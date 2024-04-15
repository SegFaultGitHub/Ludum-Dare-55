using UnityEngine;

namespace Code.UI {
    public class OnScreenArrowTrigger : MonoBehaviour {
        public Transform Target;
        public float Angle;

        private Tutorial Tutorial;

        private void Start() => this.Tutorial = FindObjectOfType<Tutorial>();

        public void Open() => this.Tutorial.Open(this.Target, this.Angle);
        public void Close() => this.Tutorial.Close();
    }
}
