using Code.Controllers;
using UnityEngine;

namespace Code.UI {
    public class SpeechBubbleTrigger : MonoBehaviour {
        public string Text;
        public Player Player;
        public bool DestroyAfterTrigger;

        private void Awake() {
            this.Player = FindObjectOfType<Player>();
        }
    }
}
