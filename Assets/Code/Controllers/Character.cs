using System;
using MyBox;
using UnityEngine;

namespace Code.Controllers {
    public class Character : MonoBehaviour {
        #region Members
        [Foldout("Character", true)]
        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected Animator m_Animator;
        [ReadOnly][SerializeField] private protected CharacterController m_Controller;
        #endregion

        #region Getters / Setters
        protected Animator Animator { get => this.m_Animator; private set => this.m_Animator = value; }
        private CharacterController Controller { get => this.m_Controller; set => this.m_Controller = value; }
        #endregion

        private protected const float WALK_SPEED_THRESHOLD = 0.1f;

        protected virtual void Awake() {
            this.Controller = this.GetComponent<CharacterController>();
            this.Animator = this.GetComponentInChildren<Animator>();
        }

        private void Update() {
            if (this.TargetPosition != null) {
                this.MovementDirection = this.TargetPosition.Value - this.transform.position;
                if (this.MovementDirection.magnitude <= (this.Running ? this.RunningSpeed : this.WalkingSpeed)) {
                    this.MovementDirection *= 0;
                    this.SetPosition(this.TargetPosition.Value);
                    this.TargetPosition = null;
                }
            }
            if (this.TargetAngle != null) {
                float diff = (this.transform.eulerAngles.y + 360) % 360 - (this.TargetAngle.Value + 360) % 360;
                if (Math.Abs(diff) < 3) {
                    this.transform.rotation = Quaternion.Euler(0, this.TargetAngle.Value, 0);
                    this.TargetAngle = null;
                }
            }
        }

        private void FixedUpdate() {
            if (this.MovementDirection.sqrMagnitude != 0) {
                float yValue = this.MovementDirection.y;
                float targetAngle = Mathf.Atan2(this.MovementDirection.x, this.MovementDirection.z) * Mathf.Rad2Deg;
                float angle = Mathf.SmoothDampAngle(
                    this.transform.eulerAngles.y,
                    targetAngle,
                    ref this.TurnSmoothVelocity,
                    TURN_SMOOTH_TIME
                );
                this.transform.rotation = Quaternion.Euler(0, angle, 0);

                Vector3 movementDirection = (Quaternion.Euler(0, targetAngle, 0) * Vector3.forward).normalized;
                movementDirection.y = yValue;
                this.Controller.Move((this.Running ? this.RunningSpeed : this.WalkingSpeed) * movementDirection);
            } else if (this.TargetAngle != null) {
                float angle = Mathf.SmoothDampAngle(
                    this.transform.eulerAngles.y,
                    this.TargetAngle.Value,
                    ref this.TurnSmoothVelocity,
                    TURN_SMOOTH_TIME
                );
                this.transform.rotation = Quaternion.Euler(0, angle, 0);
            }

            CollisionFlags flags = this.Controller.Move(this.FallingSpeed);

            switch (flags) {
                case CollisionFlags.Below:
                    this.FallingSpeed = GRAVITY;
                    this.Animator.SetBool(FALLING, false);
                    this.Grounded = true;
                    break;
                case CollisionFlags.Above:
                    this.FallingSpeed = GRAVITY;
                    this.Grounded = false;
                    break;
                case CollisionFlags.None:
                case CollisionFlags.Sides:
                default:
                    this.FallingSpeed += GRAVITY;
                    if (this.Grounded) this.FallingSince = Time.realtimeSinceStartupAsDouble;
                    if (Time.realtimeSinceStartupAsDouble - this.FallingSince > 0.1) this.Animator.SetBool(FALLING, true);
                    this.Grounded = false;
                    break;
            }
        }

        public void SetPosition(Vector3 position) {
            this.Controller.enabled = false;
            this.transform.position = position;
            this.Controller.enabled = true;
        }

        public void LookTowards(Transform target) {
            Quaternion q = Quaternion.LookRotation(target.position - this.transform.position, Vector3.up);
            this.TargetAngle = q.eulerAngles.y;
        }

        #region Movement
        private const float TURN_SMOOTH_TIME = 0.1f;
        private static readonly Vector3 GRAVITY = new(0, -0.02f, 0);
        protected bool Running = false;
        [field: SerializeField] public float WalkingSpeed { get; protected set; }
        [field: SerializeField] public float RunningSpeed { get; protected set; }
        protected Vector3 FallingSpeed;
        protected bool Grounded = false;
        private double FallingSince = 0;
        [field: SerializeField] public Vector3 MovementDirection { get; protected set; }
        private float TurnSmoothVelocity;
        private Vector3? TargetPosition { get; set; }
        private float? TargetAngle { get; set; }

        protected static readonly int SPEED = Animator.StringToHash("Speed");
        protected static readonly int JUMP = Animator.StringToHash("Jump");
        private static readonly int FALLING = Animator.StringToHash("Falling");
        #endregion
    }
}
