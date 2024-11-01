using DG.Tweening;
using TMPro;
using UnityEngine;

namespace IndicatorAsset
{
    public class IndicatorText : MonoBehaviour
    {
        private TextMeshProUGUI textMesh;

        internal void Init()
        {
            textMesh = GetComponentInChildren<TextMeshProUGUI>();
            gameObject.SetActive(false);
        }
        public void SetPosition(Vector3 position)
        {
            transform.localPosition = position;
        }
        public void SetText(string text)
        {
            textMesh.text = text;
        }
        public void SetColor(Color color)
        {
            textMesh.color = color;
        }
        public void SetSize(int textSize)
        {
            textMesh.fontSize = textSize;
        }
        public void Play(float showDuration, IndicatorAnimationTypes animationType)
        {
            gameObject.SetActive(true);
            transform.localScale = Vector3.zero;

            switch (animationType)
            {
                case IndicatorAnimationTypes.Popup:
                    transform.DOScale(1.2f, 0.2f).OnComplete(delegate
                    {
                        transform.DOScale(1f, 0.05f);
                    });

                    transform.DOScale(Vector3.zero, 0.25f).SetDelay(0.25f + showDuration).OnComplete(AnimationEnd);
                    break;
                case IndicatorAnimationTypes.Jump:
                    break;
                case IndicatorAnimationTypes.Floating:
                    Color targetColor = textMesh.color;
                    targetColor.a = 0;
                    textMesh.DOColor(targetColor, showDuration - 0.25f).SetDelay(0.25f);

                    transform.DOScale(1, 0.25f);
                    transform.DOLocalMoveY(transform.localPosition.y + 200, showDuration).SetDelay(0.1f).OnComplete(AnimationEnd);
                    break;
                case IndicatorAnimationTypes.Fade:
                    break;
                default:
                    break;
            }
        }

        private void AnimationEnd()
        {
            gameObject.SetActive(false);
            Indicator.Instance.PutIndicatorText(this);
        }
    }
}