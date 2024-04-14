using MyBox;
using UnityEngine;

namespace Code {
    public class Platform : MonoBehaviour {
        #region Members
        [Foldout("Platform", true)]
        [SerializeField] private protected float m_Distance;
        #endregion

        #region Getters / Setters
        public float Distance { get => this.m_Distance; }
        #endregion
    }
}
