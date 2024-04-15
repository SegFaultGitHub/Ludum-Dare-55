using System.Collections.Generic;
using Code.Controllers;
using Code.Levels;
using Code.PuzzleObjects;
using Code.UI;
using External.LeanTween.Framework;
using MyBox;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Environment = Code.PuzzleObjects.Environment;

namespace Code.Managers {
    public class GameplayManager : WithRaycast {
        #region Members
        [Foldout("PlatformManager", true)]
        [Separator("Read only")]
        [ReadOnly][SerializeField] private Player m_Player;
        [ReadOnly][SerializeField] private Hit<Platform> m_CurrentPlatform;
        [ReadOnly][SerializeField] private Block m_GrabbedBlock;
        [ReadOnly][SerializeField] private Vector3 m_GrabbedBlockInitialPosition;
        [ReadOnly][SerializeField] private Vector3 m_GrabbedBlockInitialAngles;

        private const float BLOCK_DEFAULT_HEIGHT = 2f;

        private const string PLATFORMS_LAYER = "Platforms";
        private const string BLOCKS_LAYER = "Blocks";
        private const string ENVIRONMENT_LAYER = "Environment";
        private const string UI_LAYER = "UI";
        private const string FALLBACK_PLANE_LAYER = "FallbackPlane";
        private LayerMask PlatformsLayerMask;
        private LayerMask BlocksLayerMask;
        private LayerMask EnvironmentLayerMask;
        private LayerMask UILayerMask;
        private LayerMask FallbackPlaneLayerMask;

        private List<Vector3> Line = new();
        private float DistanceToCamera;

        private Hit<Block> CurrentBlock;
        private Hit<Block> CurrentUIBlock;

        private Level Level;
        #endregion

        #region Getters / Setters
        private Player Player { get => this.m_Player; set => this.m_Player = value; }
        private Hit<Platform> CurrentPlatform { get => this.m_CurrentPlatform; set => this.m_CurrentPlatform = value; }
        private Block GrabbedBlock { get => this.m_GrabbedBlock; set => this.m_GrabbedBlock = value; }
        private Vector3 GrabbedBlockInitialPosition {
            get => this.m_GrabbedBlockInitialPosition;
            set => this.m_GrabbedBlockInitialPosition = value;
        }
        private Vector3 GrabbedBlockInitialAngles {
            get => this.m_GrabbedBlockInitialAngles;
            set => this.m_GrabbedBlockInitialAngles = value;
        }
        #endregion

        private void Awake() {
            this.Player = FindObjectOfType<Player>();
            this.PlatformsLayerMask = 1 << LayerMask.NameToLayer(PLATFORMS_LAYER);
            this.BlocksLayerMask = 1 << LayerMask.NameToLayer(BLOCKS_LAYER);
            this.EnvironmentLayerMask = 1 << LayerMask.NameToLayer(ENVIRONMENT_LAYER);
            this.UILayerMask = 1 << LayerMask.NameToLayer(UI_LAYER);
            this.FallbackPlaneLayerMask = 1 << LayerMask.NameToLayer(FALLBACK_PLANE_LAYER);
            this.Level = FindObjectOfType<Level>();
        }

        private void Update() {
            this.GatherInputs();

            this.CurrentPlatform = this.Raycast<Platform>(this.PlatformsLayerMask);
            Hit<Block> currentBlock = this.CurrentBlock;
            this.CurrentBlock = this.Raycast<Block>(this.BlocksLayerMask);

            if (this.GrabbedBlock is not null) {
                Vector3 newPosition = this.GrabbedBlock.Model.transform.position;
                this.Line.Clear();
                if (this.CurrentPlatform is not null) {
                    newPosition = this.CurrentPlatform.RaycastHit.point
                                  + this.CurrentPlatform.RaycastHit.normal.normalized * this.CurrentPlatform.Obj.Distance;
                    this.Line = new List<Vector3> { this.CurrentPlatform.RaycastHit.point, newPosition };
                } else {
                    this.GrabbedBlock.OpenCanvas();
                    Hit<Environment> environmentHit = this.Raycast<Environment>(this.EnvironmentLayerMask);
                    Hit<FallbackPlane> fallbackHit = this.Raycast<FallbackPlane>(this.FallbackPlaneLayerMask);
                    if (environmentHit is not null) {
                        newPosition = environmentHit.RaycastHit.point + environmentHit.RaycastHit.normal.normalized * BLOCK_DEFAULT_HEIGHT;
                        this.Line = new List<Vector3> { environmentHit.RaycastHit.point, newPosition };
                    } else if (fallbackHit is not null) {
                        newPosition = fallbackHit.RaycastHit.point + fallbackHit.RaycastHit.normal.normalized * BLOCK_DEFAULT_HEIGHT;
                        this.Line = new List<Vector3> { fallbackHit.RaycastHit.point, newPosition };
                    }
                }
                this.GrabbedBlock.Model.transform.position = Vector3.Lerp(this.GrabbedBlock.Model.transform.position, newPosition, 0.25f);
            } else if (this.CurrentBlock is not null) {
                if (this.Player.CanGrab(this.CurrentBlock.Obj)) this.CurrentBlock.Obj.CloseCanvas();
                else this.CurrentBlock.Obj.OpenCanvas();
            } else if (this.CurrentBlock is null && currentBlock is not null) {
                currentBlock.Obj.CloseCanvas();
            }
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos() {
            if (this.Line.Count > 0) {
                Handles.color = Color.red;
                Handles.DrawLine(this.Line[0], this.Line[1]);
            }
        }
        #endif

        private void GrabBlock() {
            if (!this.Player.Grounded) return;
            Hit<Block> uiBlockHit = this.Raycast<Block>(this.UILayerMask);
            Hit<Block> blockHit = this.Raycast<Block>(this.BlocksLayerMask);

            if (uiBlockHit is null && blockHit is null) return;

            if (uiBlockHit is not null) {
                uiBlockHit.Obj.SetLayerRecursively(BLOCKS_LAYER);
                this.GrabbedBlock = uiBlockHit.Obj;
                this.GrabbedBlock.transform.SetParent(null);
                this.GrabbedBlock.transform.eulerAngles = new Vector3(0, 0, 0);
                this.GrabbedBlock.InitCanvas();
                this.GrabbedBlock.InUI = false;
            } else if (this.Player.CanGrab(blockHit.Obj)) {
                this.GrabbedBlock = blockHit.Obj;
            } else {
                return;
            }

            this.GrabbedBlock.Grab();
            this.Player.CanMove = false;
            this.Player.GrabBlock(this.GrabbedBlock);
            this.GrabbedBlockInitialPosition = this.GrabbedBlock.transform.position;
            this.GrabbedBlockInitialAngles = this.GrabbedBlock.transform.eulerAngles;
            // this.GrabbedBlock.Disable();
        }

        private void ReleaseBlock() {
            if (this.GrabbedBlock is null) return;

            void _Reset() {
                this.Player.CanMove = true;
                this.Line.Clear();
                // this.GrabbedBlock.Enable();
                this.GrabbedBlock = null;
            }

            bool release = this.CurrentPlatform is not null && this.Player.CanRelease(this.GrabbedBlock.Model.transform.position);

            if (this.GrabbedBlock.FromUI) {
                if (!release) {
                    this.GrabbedBlock.SetLayerRecursively(UI_LAYER);
                    this.GrabbedBlock.MoveBackToUI();
                } else {
                    this.GrabbedBlock.FromUI = false;
                    this.GrabbedBlock.InUI = false;
                    this.Level.BlockInstances.Remove(this.GrabbedBlock);
                }
            }
            this.GrabbedBlock.Release();
            this.Player.ReleaseBlock();

            if (!release) {
                LeanTween.move(this.GrabbedBlock.Model.gameObject, this.GrabbedBlockInitialPosition, .15f)
                    .setEaseOutSine()
                    .setOnComplete(_Reset);
                LeanTween.rotate(this.GrabbedBlock.Model.gameObject, this.GrabbedBlockInitialAngles, .15f)
                    .setEaseOutSine()
                    .setOnComplete(_Reset);
            } else {
                this.GrabbedBlock.transform.position = this.GrabbedBlock.Model.transform.position;
                this.GrabbedBlock.transform.eulerAngles = this.GrabbedBlock.Model.transform.eulerAngles;
                this.GrabbedBlock.Model.transform.localPosition = Vector3.zero;
                this.GrabbedBlock.Model.transform.localEulerAngles = Vector3.zero;
                this.GrabbedBlock.Platform = this.CurrentPlatform.Obj;
                _Reset();
            }

        }

        #region Input
        protected override void OnEnable() {
            base.OnEnable();

            this.InputActions.Globals.MoveBlock.started += this.MoveBlockStartedInput;
            this.InputActions.Globals.MoveBlock.canceled += this.MoveBlockCanceledInput;
        }

        protected override void OnDisable() {
            this.InputActions.Globals.MoveBlock.started -= this.MoveBlockStartedInput;
            this.InputActions.Globals.MoveBlock.canceled -= this.MoveBlockCanceledInput;

            base.OnDisable();
        }

        private void MoveBlockStartedInput(InputAction.CallbackContext _) => this.GrabBlock();
        private void MoveBlockCanceledInput(InputAction.CallbackContext _) => this.ReleaseBlock();
        #endregion
    }
}
