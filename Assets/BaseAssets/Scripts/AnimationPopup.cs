using UnityEngine;

public class AnimationPopup : MonoBehaviour
{
    private Transform tr;

    private void Awake()
    {
        tr = transform;
        currentScale = transform.localScale;
    }

    private void OnEnable()
    {
        StartPopup();
    }

    private bool reverse;
    public void StartPopup(bool reverse = false)
    {
        SetCurve();

        this.reverse = reverse;
        t = !reverse ? 0 : scaleCurve.keys[scaleCurve.keys.Length - 1].time;
        startAnimation = true;
        ResetScale();
    }

    private void ResetScale()
    {
        tr.localScale = currentScale * scaleCurve.Evaluate(t);
    }

    private Vector3 currentScale;
    private bool startAnimation;
    private void LateUpdate()
    {
        PopupAnimation();
    }

    private float t;

    public bool ownCurve;
    [BaseAssets.ShowIf(nameof(ownCurve))]
    public AnimationCurve scaleCurve;
    private void SetCurve()
    {
        if (ownCurve)
            return;

        if (scaleCurve == null)
            scaleCurve = new AnimationCurve();

        int keyCount = scaleCurve.keys.Length;
        for (int i = 0; i < keyCount; i++)
        {
            scaleCurve.RemoveKey(0);
        }

        scaleCurve.AddKey(0, 0);
        scaleCurve.AddKey(0.5f, 1.1f);
        scaleCurve.AddKey(1f, 1f);
    }

    private void PopupAnimation()
    {
        if (startAnimation)
        {
            t += Time.deltaTime / 0.25f * (!reverse ? 1f : -1f);
            tr.localScale = currentScale * scaleCurve.Evaluate(t);
            if (!reverse ? t >= scaleCurve.keys[scaleCurve.keys.Length - 1].time : t <= 0)
            {
                startAnimation = false;
            }
        }
    }
}
