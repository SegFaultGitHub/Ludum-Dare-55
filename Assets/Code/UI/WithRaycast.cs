using System;
using MyBox;
using UnityEngine;

namespace Code.UI {
    public abstract class WithRaycast : MonoBehaviour {
        #region Members
        [Foldout("WithRaycast")]
        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected UnityEngine.Camera m_Camera;
        [ReadOnly][SerializeField] private protected Vector2 m_MousePosition;
        [ReadOnly][SerializeField] private protected Vector2 m_MousePositionDelta;

        private static readonly RaycastHit[] RAYCAST_HITS = new RaycastHit[10];
        #endregion

        #region Getters / Setters
        private UnityEngine.Camera Camera { get => this.m_Camera; set => this.m_Camera = value; }
        private Vector2 MousePosition { get => this.m_MousePosition; set => this.m_MousePosition = value; }
        private Vector2 MousePositionDelta { get => this.m_MousePositionDelta; set => this.m_MousePositionDelta = value; }
        #endregion


        protected virtual void Start() {
            this.Camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UnityEngine.Camera>();
        }

        protected Hit<T> Raycast<T>(LayerMask layer) {
            for (int i = 0; i < RAYCAST_HITS.Length; i++) RAYCAST_HITS[i] = default;
            Physics.RaycastNonAlloc(this.Camera.ScreenPointToRay(this.MousePosition), RAYCAST_HITS, 1000, layer);

            float minDistance = float.MaxValue;
            RaycastHit? result = null;
            foreach (RaycastHit hit in RAYCAST_HITS) {
                if (hit.collider is null) break;
                if (hit.collider.GetComponentInParent<T>() == null) continue;
                if (minDistance < hit.distance) continue;

                minDistance = hit.distance;
                result = hit;
            }

            if (result == null) return null;
            return new Hit<T> {
                RaycastHit = result.Value,
                Obj = result.Value.collider.GetComponentInParent<T>()
            };
        }

        [Serializable]
        protected class Hit<T> {
            #region Members
            [SerializeField] private T m_Obj;

            public RaycastHit RaycastHit { get; set; }
            #endregion

            #region Getters / Setters
            public T Obj { get => this.m_Obj; set => this.m_Obj = value; }
            #endregion
        }

        #region Input
        protected InputActions InputActions;

        protected virtual void OnEnable() {
            this.InputActions = new InputActions();
            this.InputActions.Globals.Enable();
        }

        protected virtual void OnDisable() {
            this.InputActions.Globals.Disable();
        }

        protected virtual void GatherInputs() {
            this.MousePositionDelta = this.MousePosition;
            this.MousePosition = this.InputActions.Globals.MousePosition.ReadValue<Vector2>();
            this.MousePositionDelta = this.MousePosition - this.MousePositionDelta;
        }
        #endregion
    }
}
