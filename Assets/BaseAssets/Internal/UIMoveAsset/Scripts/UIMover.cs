using System;
using UnityEngine;
using UnityEngine.UI;

public class UIMover : MonoBehaviour
{
    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private AnimationCurve scaleCurve;
    [SerializeField] private Gradient colorOverTime;
    
    Transform tr;

    Image img;
    Vector3 defaultScale;

    bool active;

    public void Init()
    {
        tr = transform;
        defaultScale = tr.localScale;
        img = GetComponent<Image>();
    }

    private Vector3 start;
    private Vector3 end;
    private Vector3 mid;
    private float duration;
    private Action onReached;
    public void StartMove(Vector3 start, Vector3 end, float duration, Sprite icon, Action onReached)
    {
        img.sprite = icon;
        img.color = colorOverTime.Evaluate(0);

        tr.position = start;
        t = 0;
        this.start = start;
        this.end = end;
        this.duration = duration;
        this.onReached = onReached;
        mid = GetRadomMid(start, end);
        active = true;

        gameObject.SetActive(true);
    }

    private float r = 400;
    private Vector3 GetRadomMid(Vector3 start, Vector3 end)
    {
        Vector3 dir = (end - start).normalized;
        Vector3 right = Vector3.Cross(dir, Vector3.forward);

        Vector3 mid = Vector3.Lerp(start, end, UnityEngine.Random.Range(0f, 0.5f));
        mid += right * UnityEngine.Random.Range(-r, r);
        return mid;
    }

    private float t;
    public void Move()
    {
        if (!active)
        {
            return;
        }
        t += Time.unscaledDeltaTime / duration;
        tr.position = Utils.Curve(start, mid, end, speedCurve.Evaluate(t));
        tr.localScale = defaultScale * speedCurve.Evaluate(t);
        img.color = colorOverTime.Evaluate(t);
        if (t >= 1)
        {
            active = false;
            gameObject.SetActive(false);

            onReached?.Invoke();
        }
    }
}