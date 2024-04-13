using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Code.Camera {
    public class CameraController : MonoBehaviour {
        [field: SerializeField] private Transform Follow;
        [field: SerializeField] private Transform FocusOn;
        [field: SerializeField] private float Distance;
        [field: SerializeField] private float AngleX;
        [field: SerializeField] private float AngleY;

        [field: SerializeField] private Volume Volume;
        private DepthOfField DepthOfField;

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
