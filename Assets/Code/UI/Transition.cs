using System;
using External.LeanTween.Framework;
using MyBox;
using UnityEngine;

namespace Code.UI {
    public class Transition : MonoBehaviour {
        private enum TransitionType {
            Show, Hide
        }

        #region Serialized fields
        [Foldout("Transition", true)]
        [SerializeField] private GameObject[] m_Bars;
        [SerializeField] private TransitionType m_Type;
        #endregion

        #region Getters / Setters
        private GameObject[] Bars { get => this.m_Bars; }
        private TransitionType Type { get => this.m_Type; }
        #endregion

        public LTDescr Run() {
            int i = 0;
            LTDescr result = null;
            foreach (GameObject bar in this.Bars) {
                int ii = i;
                RectTransform rect = bar.GetComponent<RectTransform>();
                LTDescr desc = LeanTween.value(rect.anchoredPosition.y, 0, .25f)
                    .setOnUpdate(
                        value => {
                            rect.anchoredPosition = new Vector2(0, value);
                        }
                    )
                    .setIgnoreTimeScale(true)
                    .setDelay(i * 0.05f);
                switch (this.Type) {
                    case TransitionType.Show:
                        desc.setEaseOutQuad();
                        break;
                    case TransitionType.Hide:
                        desc.setEaseInQuad();
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }

                if (ii == this.Bars.Length - 1) result = desc;
                i++;
            }

            return result;
        }
    }
}
