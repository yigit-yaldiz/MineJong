using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace BaseAssets
{
    [DisallowMultipleComponent]
    public class ButtonClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [Tooltip("After one time click, it will not be usable again")]
        public bool singleClick;
        public bool interactable = true;
        public bool animate = true;
        public bool feel = true;

        [Tooltip("Function will call with hold instead of click")]
        public bool callOnHold;
        public UnityEvent onClick;
        public UnityEvent onRelease;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!interactable)
                return;

            if (callOnHold == true)
                return;

            if (singleClick && _clickedOnce)
                return;

            CallFunction();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!interactable)
                return;

            if (!firstDown)
            {
                currentScale = transform.localScale;
                firstDown = true;
            }
            startAnimate = animate;
            animateUp = false;

            if (callOnHold == false)
                return;

            if (singleClick && _clickedOnce)
                return;

            CallFunction();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            animateUp = true;
            onRelease?.Invoke();
        }

        private bool _clickedOnce;
        private void CallFunction()
        {
            if (singleClick)
                _clickedOnce = true;

            onClick.Invoke();

            if (feel)
            {
                BA.PlayHaptic(BA.HapticTypes.MediumImpact);
                SoundManager.Instance.PlaySound(SoundManager.SoundEffect.ButtonClick);
            }
        }

        private void LateUpdate()
        {
            if (!startAnimate)
            {
                return;
            }
            if (!animateUp)
            {
                tr.localScale = Vector3.MoveTowards(tr.localScale, currentScale * 0.875f, Time.unscaledDeltaTime * 3);
            }
            else
            {
                tr.localScale = Vector3.MoveTowards(tr.localScale, currentScale, Time.unscaledDeltaTime * 3);
                if (tr.localScale == currentScale)
                {
                    animateUp = false;
                    startAnimate = false;
                }
            }
        }

        private bool startAnimate;
        private bool animateUp;
        private Transform tr;
        private Vector3 currentScale;
        private void Start()
        {
            tr = transform;
        }
        private bool firstDown;
        private void OnEnable()
        {
            firstDown = false;
        }
    }
}

