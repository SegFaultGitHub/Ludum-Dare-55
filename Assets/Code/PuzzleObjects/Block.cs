using MyBox;
using UnityEngine;

namespace Code {
    public class Block : MonoBehaviour {
        #region Members
        [Foldout("Block", true)]
        [Separator("Read only")]
        [ReadOnly][SerializeField] private Platform m_Platform;
        [ReadOnly][SerializeField] private protected Rigidbody m_Rigidbody;
        [ReadOnly][SerializeField] private protected Collider m_Collider;
        #endregion

        #region Getters / Setters
        private Platform Platform { get => this.m_Platform; set => this.m_Platform = value; }
        private Rigidbody Rigidbody { get => this.m_Rigidbody; set => this.m_Rigidbody = value; }
        private Collider Collider { get => this.m_Collider; set => this.m_Collider = value; }
        #endregion

        private void Awake() {
            this.Rigidbody = this.GetComponent<Rigidbody>();
            this.Collider = this.GetComponent<Collider>();
        }

        public void Disable() {
            this.Rigidbody.isKinematic = true;
            this.Collider.enabled = false;
        }

        public void Enable() {
            this.Rigidbody.isKinematic = false;
            this.Collider.enabled = true;
        }
    }
}
