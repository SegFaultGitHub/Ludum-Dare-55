using System.Collections;
using System.Linq;
using UnityEngine;

namespace Code.Utils {
    public class DestroyOnAnimationEvent : MonoBehaviour {
        public void Destroy() {
            this.StartCoroutine(this.DestroyWhenFinished());
        }

        private IEnumerator DestroyWhenFinished() {
            ParticleSystem[] particles = this.GetComponentsInChildren<ParticleSystem>();
            yield return new WaitUntil(() => particles.All(particle => particle.isStopped));
            Destroy(this.gameObject);
        }
    }
}
