using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageTextScript : MonoBehaviour
{
    private Transform tr;
    private Transform trMove;
    private TextMeshProUGUI txt;

    public void Init()
    {
        tr = transform;
        txt = GetComponentInChildren<TextMeshProUGUI>();
        trMove = txt.transform;
        trCamera = Camera.main.transform;
        isWorld = GetComponentInParent<Canvas>().renderMode == RenderMode.WorldSpace;
    }

    private bool active;
    private float t;
    private AnimationCurve moveCurve;
    private AnimationCurve scaleCurve;
    public void StartShow(Vector3 position, string txt, Color color, AnimationCurve moveCurve, AnimationCurve scaleCurve, float speed, float spread)
    {
        gameObject.SetActive(true);

        Vector3 rand = Random.insideUnitCircle * spread;
        tr.position = position + rand;
        this.txt.text = txt;
        this.txt.color = color;

        this.moveCurve = moveCurve;
        moveCurveLength = moveCurve.keys[moveCurve.length - 1].time;

        this.scaleCurve = scaleCurve;
        scaleCurveLength = scaleCurve.keys[scaleCurve.length - 1].time;

        LookToCamera();
        MoveAndScale(0);

        this.speed = speed;

        t = 0;
        active = true;
    }

    private float moveCurveLength;
    private float scaleCurveLength;

    private float speed;

    private void Update()
    {
        if (active)
        {
            LookToCamera();

            t += Time.deltaTime * speed;
            MoveAndScale(t);
            if (t >= moveCurveLength && t >= scaleCurveLength)
            {
                active = false;
                gameObject.SetActive(false);
            }
        }
    }

    private void MoveAndScale(float t)
    {
        trMove.localPosition = Vector3.up * moveCurve.Evaluate(t);
        trMove.localScale = Vector3.one * scaleCurve.Evaluate(t);
    }

    private bool isWorld;
    private Transform trCamera;
    private void LookToCamera()
    {
        if (isWorld)
        {
            tr.rotation = Quaternion.LookRotation(tr.position - trCamera.position);
        }
    }
}
