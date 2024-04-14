using System;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Utils {
    public class RandomGameObject : MonoBehaviour {
        #region Members
        [field: SerializeField] private List<WeightDistribution<GameObject>> m_Objects;
        [Header("X")]
        [field: SerializeField] private bool m_RandomRotateX;
        [field: SerializeField][field: ConditionalField(nameof(m_RandomRotateX))]
        private int m_AngleStepX = 1;
        [Header("Y")]
        [field: SerializeField] private bool m_RandomRotateY;
        [field: SerializeField][field: ConditionalField(nameof(m_RandomRotateY))]
        private int m_AngleStepY = 1;
        [Header("Z")]
        [field: SerializeField] private bool m_RandomRotateZ;
        [field: SerializeField][field: ConditionalField(nameof(m_RandomRotateZ))]
        private int m_AngleStepZ = 1;
        #endregion

        #region Getters / Setters
        private List<WeightDistribution<GameObject>> Objects { get => this.m_Objects; }
        private bool RandomRotateX { get => this.m_RandomRotateX; }
        private int AngleStepX { get => this.m_AngleStepX; }
        private bool RandomRotateY { get => this.m_RandomRotateY; }
        private int AngleStepY { get => this.m_AngleStepY; }
        private bool RandomRotateZ { get => this.m_RandomRotateZ; }
        private int AngleStepZ { get => this.m_AngleStepZ; }
        #endregion

        private void Awake() {
            if (this.Objects.Count == 0)
                throw new Exception($"[RandomGameObject:Awake] Objects is empty! {this.name}");

            GameObject obj = Utils.Sample(this.Objects);
            foreach (WeightDistribution<GameObject> randomGameObject in
                     this.Objects.Where(randomGameObject => randomGameObject.Obj != obj)) {
                Destroy(randomGameObject.Obj);
            }

            if (obj == null)
                throw new Exception($"[RandomGameObject:Awake] Object is null! {this.name}");
            obj.SetActive(true);

            Vector3 angles = obj.transform.localEulerAngles;
            float x = angles.x, y = angles.y, z = angles.z;
            if (this.RandomRotateX) {
                x += Random.Range(0, 360 / this.AngleStepX) * this.AngleStepX;
            }

            if (this.RandomRotateY) {
                y += Random.Range(0, 360 / this.AngleStepY) * this.AngleStepY;
            }

            if (this.RandomRotateZ) {
                z += Random.Range(0, 360 / this.AngleStepZ) * this.AngleStepZ;
            }

            obj.transform.localEulerAngles = new Vector3(x, y, z);
        }
    }
}
