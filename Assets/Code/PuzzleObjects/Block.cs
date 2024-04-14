using Code.Controllers;
using Code.UI;
using External.LeanTween.Framework;
using MyBox;
using Unity.VisualScripting;
using UnityEngine;

namespace Code.PuzzleObjects {
    public class Block : MonoBehaviour, IWithWorldCanvas {
        #region Members
        [Foldout("Block", true)]
        [SerializeField] private GameObject m_Model;
        [SerializeField] private Material m_TransparentMaterial;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private Player m_Player;
        [ReadOnly][SerializeField] private Canvas m_Canvas;
        [ReadOnly][SerializeField] private Rigidbody m_Rigidbody;
        [ReadOnly][SerializeField] private GameObject m_Phantom;
        [ReadOnly][SerializeField] private Platform m_Platform;

        public bool Grabbed;
        private LTDescr Tween;
        private bool CanvasOpened;
        public Transform UIParent;
        public bool FromUI = false;
        public bool InUI = false;
        #endregion

        #region Getters / Setters
        public GameObject Model { get => this.m_Model; }
        private Material TransparentMaterial { get => this.m_TransparentMaterial; }
        private Player Player { get => this.m_Player; set => this.m_Player = value; }
        private Canvas Canvas { get => this.m_Canvas; set => this.m_Canvas = value; }
        public Rigidbody Rigidbody { get => this.m_Rigidbody; set => this.m_Rigidbody = value; }
        private GameObject Phantom { get => this.m_Phantom; set => this.m_Phantom = value; }
        public Platform Platform { get => this.m_Platform; set => this.m_Platform = value; }
        #endregion

        private void Awake() {
            this.Canvas = this.GetComponentInChildren<Canvas>();
            this.Rigidbody = this.GetComponent<Rigidbody>();
            this.Player = FindObjectOfType<Player>();
            this.InitCanvas();
            this.CanvasOpened = false;
        }

        public void InitCanvas() {
            this.Canvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UnityEngine.Camera>();
            this.Canvas.gameObject.SetActive(false);
            this.Canvas.transform.localScale *= 0;
        }

        public void MoveBackToUI() {
            if (!this.FromUI) return;

            this.InUI = true;
            this.CloseCanvas();
            this.Canvas.gameObject.SetActive(false);
            this.transform.SetParent(this.UIParent);
        }

        private void Update() {
            if (!this.InUI) {
                this.Canvas.gameObject.layer = LayerMask.NameToLayer("UI");
                this.Canvas.transform.position = this.Model.transform.position + new Vector3(0, 3, 0);
                ((IWithWorldCanvas)this).RotateCanvas(this.Canvas);
            }

            if (this.Grabbed) {
                if (this.Player.CanRelease(this.Model.transform.position)) this.CloseCanvas();
                else this.OpenCanvas();
            }
        }

        public void Grab() {
            this.CloseCanvas();
            this.Rigidbody.isKinematic = true;
            if (this.Phantom is not null) Destroy(this.Phantom);
            this.Phantom = Instantiate(this.Model);
            Transform t = this.transform;
            this.Phantom.transform.position = t.position;
            this.Phantom.transform.eulerAngles = t.eulerAngles;
            this.Phantom.transform.localScale = this.Model.transform.localScale;
            this.Phantom.GetComponentInChildren<MeshRenderer>().material = this.TransparentMaterial;

            this.Grabbed = true;
        }

        public void Release() {
            this.CloseCanvas();
            if (!this.FromUI)
                this.Rigidbody.isKinematic = false;
            LeanTween.scale(this.Phantom, Vector3.zero, .3f).setEaseInBack().setDestroyOnComplete(true);
            this.Grabbed = false;
        }

        public void OpenCanvas() {
            if (this.CanvasOpened) return;
            Debug.Log("OPEN");
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
            Debug.Log("CLOSE");
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
    }
}
