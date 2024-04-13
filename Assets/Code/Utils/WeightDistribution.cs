using System;
using UnityEngine;

namespace Code.Utils {
    [Serializable]
    public struct WeightDistribution<T> {
        #region Members
        [SerializeField] private float m_Weight;
        [SerializeField] private T m_Obj;
        #endregion

        #region Getters / Setters
        public float Weight { get => this.m_Weight; }
        public T Obj { get => this.m_Obj; }
        #endregion
    }
}
