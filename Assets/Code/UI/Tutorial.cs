using System;
using External.LeanTween.Framework;
using UnityEngine;

namespace Code.UI {
    public class Tutorial : MonoBehaviour {
        public GameObject Arrow;
        private LTDescr Tween;
        public Camera Camera;

        private void Awake() {
            this.Camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UnityEngine.Camera>();
        }

        public void Open(Transform target, float angle) {
            if (this.Tween != null) {
                LeanTween.cancel(this.Tween.id);
                this.Tween = null;
            }

            this.Arrow.transform.position = this.Camera.WorldToScreenPoint(target.position);
            this.Arrow.transform.localEulerAngles = new Vector3(0, 0, angle);

            this.Arrow.SetActive(true);
            float duration = 1 - this.Arrow.transform.localScale.x;
            this.Tween = LeanTween.scale(this.Arrow, Vector3.one, duration * 0.15f)
                .setEaseOutBack()
                .setOnComplete(() => this.Tween = null);
        }

        public void Close() {
            if (this.Tween != null) {
                LeanTween.cancel(this.Tween.id);
                this.Tween = null;
            }

            float duration = this.Arrow.transform.localScale.x;
            this.Tween = LeanTween.scale(this.Arrow, Vector3.zero, duration * 0.15f)
                .setEaseInBack()
                .setOnComplete(
                    () => {
                        this.Arrow.gameObject.SetActive(false);
                        this.Tween = null;
                    }
                );
        }
    }
}
