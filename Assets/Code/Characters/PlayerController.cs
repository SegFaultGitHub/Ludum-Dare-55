using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Characters {
    public class PlayerController : CharacterController {
        private Transform Camera;
        [field: SerializeField] private float JumpPower;

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

            this.Animator.SetFloat(SPEED, speed * (this.Running ? this.RunningSpeed : this.WalkingSpeed) / WalkSpeedThreshold);
        }

        #region Input
        private List<Action> Interactions;

        private PlayerInputs PlayerInputs;
        private class _Input {
            public bool MoveStarted;
            public bool MoveInProgress;
            public bool MoveEnded;
            // --
            public bool InteractPressed;
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
                InteractPressed = this.PlayerInputs.Controls.Interact.WasPressedThisFrame(),
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
            if (this.Input.InteractPressed) {
                this.Interact();
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

        private void Interact() {
            if (this.Interactions.Count == 0)
                return;

            foreach (Action action in this.Interactions) {
                action.Invoke();
            }
        }

        private void Jump() {
            if (!this.Grounded) return;
            this.FallingSpeed = new Vector3(0, this.JumpPower, 0);
            this.Animator.SetTrigger(JUMP);
        }
        #endregion
    }
}
