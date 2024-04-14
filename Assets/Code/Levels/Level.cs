using System;
using System.Collections.Generic;
using System.Linq;
using Code.PuzzleObjects;
using MyBox;
using UnityEngine;

namespace Code.Levels {
    public class Level : MonoBehaviour {
        #region Members
        public List<Block> Blocks;
        public Transform Scaler;
        public Transform Parent;
        public List<Block> BlockInstances = new();
        public float RotateSpeed;
        #endregion

        private void Start() {
            UnityEngine.Camera camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UnityEngine.Camera>();
            const int space = 4;
            float offsetX = -(this.Blocks.Count - 1f) / 2f * space;
            foreach (Block obj in this.Blocks.Select(block => Instantiate(block, this.Parent))) {
                obj.SetLayerRecursively(this.Parent.gameObject.layer);
                obj.Rigidbody.isKinematic = true;
                obj.FromUI = true;
                obj.InUI = true;
                obj.UIParent = this.Parent;
                obj.transform.localPosition = new Vector3(offsetX, 0, 0);
                obj.transform.LookAt(camera.transform);
                this.BlockInstances.Add(obj);
                offsetX += space;
            }
        }

        private void Update() {
            Vector3 scale = this.Scaler.parent.localScale;
            this.Scaler.localScale = new Vector3(1f / scale.x, 1f / scale.y, 1f / scale.z);
            this.Parent.localScale = Vector3.one;
        }

        private void FixedUpdate() {
            foreach (Block instance in this.BlockInstances.Where(instance => !instance.Grabbed)) {
                instance.transform.Rotate(this.Parent.up, this.RotateSpeed);
            }
        }
    }
}
