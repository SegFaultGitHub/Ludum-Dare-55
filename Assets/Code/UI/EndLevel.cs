using UnityEngine;

namespace Code.UI {
    public class EndLevel : MonoBehaviour {
        public InGameMenu InGameMenu;

        public void OpenMenu() {
            this.InGameMenu.DisplayEndGameMenu();
        }
    }
}
