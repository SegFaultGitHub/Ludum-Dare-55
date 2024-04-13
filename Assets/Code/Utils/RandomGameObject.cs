using System;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Utils {
    public class RandomGameObject : MonoBehaviour {
        [field: SerializeField] private List<WeightDistribution<GameObject>> Objects;
        [Header("X")]
        [field: SerializeField] private bool RandomRotateX;
        [field: SerializeField, ConditionalField(nameof(RandomRotateX))]
        private int AngleStepX = 1;
        [Header("Y")]
        [field: SerializeField] private bool RandomRotateY;
        [field: SerializeField, ConditionalField(nameof(RandomRotateY))]
        private int AngleStepY = 1;
        [Header("Z")]
        [field: SerializeField] private bool RandomRotateZ;
        [field: SerializeField, ConditionalField(nameof(RandomRotateZ))]
        private int AngleStepZ = 1;

        private void Awake() {
            if (this.Objects.Count == 0)
                throw new Exception($"[RandomGameObject:Awake] Objects is empty! {this.name}");

            GameObject obj = Utils.Sample(this.Objects);
            foreach (WeightDistribution<GameObject> randomGameObject in this.Objects.Where(randomGameObject => randomGameObject.Obj != obj))
                Destroy(randomGameObject.Obj);
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
