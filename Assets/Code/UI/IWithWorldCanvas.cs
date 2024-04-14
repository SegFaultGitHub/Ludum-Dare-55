using UnityEngine;

namespace Code.UI {
    public interface IWithWorldCanvas {
        public void RotateCanvas(Canvas canvas) {
            canvas.transform.eulerAngles = canvas.worldCamera.transform.eulerAngles;
        }
    }
}
