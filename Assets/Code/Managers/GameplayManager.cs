using Code.PuzzleObjects;
using Code.UI;
using MyBox;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Managers {
    public class GameplayManager : WithRaycast {
        #region Members
        [Foldout("PlatformManager", true)]
        [Separator("Read only")]
        [ReadOnly][SerializeField] private Hit<Platform> m_CurrentPlatform;
        [ReadOnly][SerializeField] private Block m_GrabbedBlock;
        [ReadOnly][SerializeField] private Vector3 m_GrabbedBlockInitialPosition;

        private const float BLOCK_DEFAULT_HEIGHT = 2f;

        private const string PLATFORMS_LAYER = "Platforms";
        private const string BLOCKS_LAYER = "Blocks";
        private const string ENVIRONMENT_LAYER = "Environment";
        private LayerMask PlatformsLayerMask;
        private LayerMask BlocksLayerMask;
        private LayerMask EnvironmentLayerMask;
        #endregion

        #region Getters / Setters
        private Hit<Platform> CurrentPlatform { get => this.m_CurrentPlatform; set => this.m_CurrentPlatform = value; }
        private Block GrabbedBlock { get => this.m_GrabbedBlock; set => this.m_GrabbedBlock = value; }
        private Vector3 GrabbedBlockInitialPosition {
            get => this.m_GrabbedBlockInitialPosition;
            set => this.m_GrabbedBlockInitialPosition = value;
        }
        #endregion

        private void Awake() {
            this.PlatformsLayerMask = 1 << LayerMask.NameToLayer(PLATFORMS_LAYER);
            this.BlocksLayerMask = 1 << LayerMask.NameToLayer(BLOCKS_LAYER);
            this.EnvironmentLayerMask = 1 << LayerMask.NameToLayer(ENVIRONMENT_LAYER);
        }

        private void Update() {
            this.GatherInputs();

            Hit<Platform> platformHit = this.Raycast<Platform>(this.PlatformsLayerMask);
            this.CurrentPlatform = platformHit;

            if (this.GrabbedBlock is not null) {
                Hit<Environment> environmentHit = this.Raycast<Environment>(this.EnvironmentLayerMask);
                Vector3 newPosition = Vector3.zero;
                if (environmentHit is not null)
                    newPosition = environmentHit.RaycastHit.point + environmentHit.Obj.transform.up * BLOCK_DEFAULT_HEIGHT;
                else if (this.CurrentPlatform is not null)
                    newPosition = this.CurrentPlatform.RaycastHit.point + this.CurrentPlatform.Obj.transform.up * this.CurrentPlatform.Obj.Distance;
                this.GrabbedBlock.transform.position = Vector3.Lerp(this.GrabbedBlock.transform.position, newPosition, 0.5f);
            }
        }

        private void GrabBlock() {
            Hit<Block> blockHit = this.Raycast<Block>(this.BlocksLayerMask);
            if (blockHit is null) return;

            this.GrabbedBlock = blockHit.Obj;
            this.GrabbedBlockInitialPosition = this.GrabbedBlock.transform.position;
            this.GrabbedBlock.Disable();
        }

        private void ReleaseBlock() {
            if (this.GrabbedBlock is null) return;
            if (this.CurrentPlatform is null) {
                this.GrabbedBlock.transform.position = this.GrabbedBlockInitialPosition;
            }

            this.GrabbedBlock.Enable();
            this.GrabbedBlock = null;
        }

        #region Input
        protected override void OnEnable() {
            base.OnEnable();

            this.InputActions.Globals.MoveBlock.started += this.MoveBlockStartedInput;
            this.InputActions.Globals.MoveBlock.canceled += this.MoveBlockCanceledInput;
        }

        protected override void OnDisable() {
            base.OnDisable();
        }

        private void MoveBlockStartedInput(InputAction.CallbackContext _) => this.GrabBlock();
        private void MoveBlockCanceledInput(InputAction.CallbackContext _) => this.ReleaseBlock();
        #endregion
    }
}
