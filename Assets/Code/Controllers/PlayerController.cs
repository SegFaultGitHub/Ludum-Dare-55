using MyBox;
using UnityEngine;

namespace Code.Controllers {
    public class Player : Character {
        #region Members
        [Foldout("Player", true)]
        [SerializeField] private float m_JumpPower;
        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected Transform m_Camera;
        #endregion

        #region Getters / Setters
        private float JumpPower { get => this.m_JumpPower; }
        private Transform Camera { get => this.m_Camera; set => this.m_Camera = value; }
        #endregion

        protected override void Awake() {
            base.Awake();
            this.Camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }

        private void Update() {
            this.GatherInput();
            this.HandleInput();
            float speed = this.MovementDirection.magnitude;
            if (speed != 0) {
                float targetAngle = Mathf.Atan2(this.MovementDirection.x, this.MovementDirection.z) * Mathf.Rad2Deg
                                    + this.Camera.eulerAngles.y;
                this.MovementDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            }

            this.Animator.SetFloat(SPEED, speed * (this.Running ? this.RunningSpeed : this.WalkingSpeed) / WALK_SPEED_THRESHOLD);
        }

        #region Input
        private PlayerInputs PlayerInputs;
        private class _Input {
            public bool MoveStarted;
            public bool MoveInProgress;
            public bool MoveEnded;
            // --
            public bool JumpStarted;
            public bool JumpInProgress;
            public bool JumpEnded;
            // --
            public bool WalkingStarted;
            public bool WalkingInProgress;
            public bool WalkingEnded;
        }
        private _Input Input;

        private void OnEnable() {
            this.PlayerInputs = new PlayerInputs();
            this.PlayerInputs.Controls.Enable();
        }

        private void OnDisable() {
            this.PlayerInputs.Controls.Disable();
        }

        private void GatherInput() {
            this.Input = new _Input {
                MoveStarted = this.PlayerInputs.Controls.Move.WasPressedThisFrame(),
                MoveInProgress = this.PlayerInputs.Controls.Move.IsPressed(),
                MoveEnded = this.PlayerInputs.Controls.Move.WasReleasedThisFrame(),
                // --
                JumpStarted = this.PlayerInputs.Controls.Jump.WasPressedThisFrame(),
                JumpInProgress = this.PlayerInputs.Controls.Jump.IsPressed(),
                JumpEnded = this.PlayerInputs.Controls.Jump.WasReleasedThisFrame(),
                // --
                WalkingStarted = this.PlayerInputs.Controls.Walking.WasPressedThisFrame(),
                WalkingInProgress = this.PlayerInputs.Controls.Walking.IsPressed(),
                WalkingEnded = this.PlayerInputs.Controls.Walking.WasReleasedThisFrame()
            };
        }

        private void HandleInput() {
            if (this.Input.MoveStarted || this.Input.MoveInProgress || this.Input.MoveEnded) {
                this.Move(this.PlayerInputs.Controls.Move.ReadValue<Vector2>());
            }

            // --
            if (this.Input.JumpInProgress) {
                this.Jump();
            }

            // --
            this.Running = !this.Input.WalkingInProgress;
        }

        private void Move(Vector2 direction) {
            this.MovementDirection = new Vector3(direction.x, 0, direction.y).normalized;
        }

        private void Jump() {
            if (!this.Grounded) return;
            this.FallingSpeed = new Vector3(0, this.JumpPower, 0);
            this.Animator.SetTrigger(JUMP);
        }
        #endregion
    }
}
