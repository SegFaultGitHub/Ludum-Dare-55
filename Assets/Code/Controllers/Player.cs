using System;
using Code.PuzzleObjects;
using Code.UI;
using External.Extensions;
using External.LeanTween.Framework;
using MyBox;
using TMPro;
using UnityEngine;

namespace Code.Controllers {
    public class Player : Character, IWithWorldCanvas {
        #region Members
        [Foldout("Player", true)]
        [SerializeField] private float m_JumpPower;
        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected Transform m_Camera;
        [ReadOnly][SerializeField] private bool m_CanMove;
        [ReadOnly][SerializeField] private Block m_GrabbedBlock;
        [ReadOnly][SerializeField] private float m_GrabbedPlayerAngle;
        [ReadOnly][SerializeField] private Vector3 m_GrabbedBlockAngles;
        #endregion

        #region Getters / Setters
        private float JumpPower { get => this.m_JumpPower; }
        private Transform Camera { get => this.m_Camera; set => this.m_Camera = value; }
        public bool CanMove { get => this.m_CanMove; set => this.m_CanMove = value; }
        private Block GrabbedBlock { get => this.m_GrabbedBlock; set => this.m_GrabbedBlock = value; }
        private float GrabbedPlayerAngle { get => this.m_GrabbedPlayerAngle; set => this.m_GrabbedPlayerAngle = value; }
        private Vector3 GrabbedBlockAngles { get => this.m_GrabbedBlockAngles; set => this.m_GrabbedBlockAngles = value; }
        #endregion

        public float MaxGrabDistance = 15;
        private static readonly int GRABBING = Animator.StringToHash("Grabbing");
        public Camera CameraController;
        public FullTransition Transition;
        public Transform CanvasTransition;

        public Canvas Canvas;
        public TMP_Text Text;
        private LTDescr Tween;
        private bool CanvasOpened;

        protected override void Awake() {
            base.Awake();
            this.Canvas = this.GetComponentInChildren<Canvas>();
            this.CameraController = FindObjectOfType<Camera>();
            this.CanMove = true;
            this.Camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
            this.Canvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UnityEngine.Camera>();
            this.Canvas.gameObject.SetActive(false);
            this.Canvas.transform.localScale *= 0;
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.transform.position, this.MaxGrabDistance);
        }
        #endif

        protected override void Update() {
            base.Update();
            this.GatherInput();

            this.Canvas.transform.position = this.transform.position + new Vector3(-4.5f, 5.5f, 2f);
            ((IWithWorldCanvas)this).RotateCanvas(this.Canvas);

            if (this.CanMove) {
                this.HandleInput();
                float speed = this.MovementDirection.magnitude;
                if (speed != 0) {
                    float targetAngle = Mathf.Atan2(this.MovementDirection.x, this.MovementDirection.z) * Mathf.Rad2Deg
                                        + this.Camera.eulerAngles.y;
                    this.MovementDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
                }

                this.Animator.SetFloat(SPEED, speed * (this.Running ? this.RunningSpeed : this.WalkingSpeed) / WALK_SPEED_THRESHOLD);
            } else {
                this.Animator.SetFloat(SPEED, 0);
                this.MovementDirection = Vector3.zero;
                if (this.GrabbedBlock is not null) {
                    this.transform.LookAt(this.GrabbedBlock.Model.transform);
                    this.transform.eulerAngles = new Vector3(0, this.transform.eulerAngles.y, 0);

                    this.GrabbedBlock.Model.transform.eulerAngles = this.GrabbedBlockAngles;
                    this.GrabbedBlock.Model.transform.Rotate(
                        Vector3.up,
                        this.transform.eulerAngles.y - this.GrabbedPlayerAngle,
                        Space.World
                    );
                }
            }
        }

        public bool CanGrab(Block block) {
            return (block.transform.position - this.transform.position).magnitude <= this.MaxGrabDistance;
        }

        public bool CanRelease(Vector3 position) {
            if (!this.Grounded) return false;
            return (this.transform.position - position).magnitude <= this.MaxGrabDistance;
        }

        public void GrabBlock(Block block) {
            this.Animator.SetBool(GRABBING, true);
            this.GrabbedBlock = block;
            this.transform.LookAt(this.GrabbedBlock.Model.transform);
            this.GrabbedPlayerAngle = this.transform.eulerAngles.y;
            this.GrabbedBlockAngles = this.GrabbedBlock.transform.eulerAngles;
        }

        public void ReleaseBlock() {
            this.Animator.SetBool(GRABBING, false);
            this.GrabbedBlock = null;
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.layer == LayerMask.NameToLayer("WaterPlane")) {
                this.CanMove = false;
                this.CameraController.Enabled = false;
                Instantiate(this.Transition, this.CanvasTransition)
                    .Run(
                        0.15f,
                        action2: () => {
                            this.CameraController.Enabled = true;
                            this.SetPosition(this.LastGroundedPosition);
                        },
                        action4: () => {
                            this.CanMove = true;
                        }
                    );
            } else if (other.gameObject.layer == LayerMask.NameToLayer("SpeechBubbleTrigger")) {
                SpeechBubbleTrigger trigger = other.gameObject.GetComponent<SpeechBubbleTrigger>();
                this.OpenCanvas(trigger.Text);

                OnScreenArrowTrigger arrowTrigger = other.gameObject.GetComponent<OnScreenArrowTrigger>();
                arrowTrigger?.Open();
            } else if (other.gameObject.layer == LayerMask.NameToLayer("EndLevel")) {
                EndLevel endLevel = other.gameObject.GetComponent<EndLevel>();
                this.CanMove = false;
                endLevel.OpenMenu();
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.gameObject.layer == LayerMask.NameToLayer("SpeechBubbleTrigger")) {
                SpeechBubbleTrigger trigger = other.gameObject.GetComponent<SpeechBubbleTrigger>();
                if (trigger.DestroyAfterTrigger) Destroy(trigger.gameObject);
                this.CloseCanvas();

                OnScreenArrowTrigger arrowTrigger = other.gameObject.GetComponent<OnScreenArrowTrigger>();
                arrowTrigger?.Close();
            }
        }

        public void OpenCanvas(string text) {
            if (this.CanvasOpened) return;
            this.Text.text = text;
            this.CanvasOpened = true;
            if (this.Tween != null) {
                LeanTween.cancel(this.Tween.id);
                this.Tween = null;
            }

            this.Canvas.gameObject.SetActive(true);
            float duration = 1 - this.Canvas.transform.localScale.x;
            this.Tween = LeanTween.scale(this.Canvas.gameObject, Vector3.one, duration * 0.15f)
                .setEaseOutBack()
                .setOnComplete(() => this.Tween = null);
        }

        public void CloseCanvas() {
            if (!this.CanvasOpened) return;
            this.CanvasOpened = false;
            if (!this.Canvas.gameObject.activeSelf)
                return;

            if (this.Tween != null) {
                LeanTween.cancel(this.Tween.id);
                this.Tween = null;
            }

            float duration = this.Canvas.transform.localScale.x;
            this.Tween = LeanTween.scale(this.Canvas.gameObject, Vector3.zero, duration * 0.15f)
                .setEaseInBack()
                .setOnComplete(
                    () => {
                        this.Canvas.gameObject.SetActive(false);
                        this.Tween = null;
                    }
                );
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
