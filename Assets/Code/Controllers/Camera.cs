using MyBox;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Code.Controllers {
    public class Camera : MonoBehaviour {
        #region Members
        [Foldout("Camera", true)]
        [SerializeField] private protected Transform m_Follow;
        [SerializeField] private protected Transform m_FocusOn;
        [SerializeField] private protected float m_Distance;
        [SerializeField] private protected float m_AngleX;
        [SerializeField] private protected float m_AngleY;
        [SerializeField] private protected Volume m_Volume;

        private DepthOfField DepthOfField;
        #endregion

        #region Getters / Setters
        private Transform Follow { get => this.m_Follow; }
        private Transform FocusOn { get => this.m_FocusOn; }
        private float Distance { get => this.m_Distance; }
        private float AngleX { get => this.m_AngleX; }
        private float AngleY { get => this.m_AngleY; }
        private Volume Volume { get => this.m_Volume; }
        #endregion

        private void Start() {
            this.Volume.sharedProfile.TryGet(out this.DepthOfField);
        }

        private void Update() {
            Vector3 offset = Quaternion.Euler(this.AngleX, this.AngleY, 0) * Vector3.forward;
            this.transform.position = this.Follow.position + offset * this.Distance;
            this.transform.LookAt(this.Follow);
            if (this.DepthOfField != null) {
                this.DepthOfField.focusDistance.value = Vector3.Distance(this.transform.position, this.FocusOn.position);
            }
        }
    }
}
