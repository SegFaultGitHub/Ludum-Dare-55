using System;
using External.Extensions;
using MyBox;
using UnityEngine;

namespace Code.UI {
    public class FullTransition : MonoBehaviour {
        #region Serialized fields
        [Foldout("FullTransition", true)]
        [SerializeField] private protected Transition m_Show;
        [SerializeField] private protected Transition m_Hide;
        #endregion

        #region Getters / Setters
        private Transition Show { get => this.m_Show; }
        private Transition Hide { get => this.m_Hide; }
        #endregion

        /***
         * Timeline:
         *  - start hide transition
         *  - action1
         *  - end hide transition
         *  - action2
         *  - wait
         *  - start show transition
         *  - action3
         *  - end show transition
         *  - action4
         */
        public void Run(float waitTime, Action action1 = null, Action action2 = null, Action action3 = null, Action action4 = null) {
            Transition hide = Instantiate(this.Hide, this.transform);
            action1?.Invoke();
            hide.Run()
                .setOnComplete(
                    () => {
                        action2?.Invoke();
                        this.InRealSeconds(
                            waitTime,
                            () => {
                                Destroy(hide.gameObject);
                                Transition show = Instantiate(this.Show, this.transform);
                                action3?.Invoke();
                                show.Run()
                                    .setOnComplete(
                                        () => {
                                            action4?.Invoke();
                                            Destroy(show.gameObject);
                                        }
                                    );
                            }
                        );
                    }
                );
        }

        public void TransitionHide(Action action) {
            Transition hide = Instantiate(this.Hide, this.transform);
            hide.Run().setOnComplete(action);
        }
    }
}
