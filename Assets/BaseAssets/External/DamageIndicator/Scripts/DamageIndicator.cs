using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    public static DamageIndicator scr;

    [SerializeField]
    private uint count = 10;
    [SerializeField]
    private GameObject prefText;

    [Space(16)]
    [SerializeField]
    private float speed;
    [SerializeField]
    private float spread = 0.5f;
    [SerializeField]
    private AnimationCurve moveCurve, scaleCurve;

    #region UnityFunctions
    private void Awake()
    {
        scr = this;
    }

    void Start()
    {
        SpawnIndicators();
    }

    private void OnValidate()
    {
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i].n = colors[i].colorType.ToString();
        }
    }
    #endregion

    private DamageTextScript[] damageTexts;
    private void SpawnIndicators()
    {
        damageTexts = new DamageTextScript[count];
        for (int i = 0; i < count; i++)
        {
            GameObject o = Instantiate(prefText);
            damageTexts[i] = o.GetComponent<DamageTextScript>();
            damageTexts[i].transform.SetParent(transform, false);
            damageTexts[i].Init();
            o.SetActive(false);
        }
    }

    private int index;
    public void Show(Vector3 position, string txt, ColorType color)
    {
        int i = index % (int)count;
        damageTexts[i].StartShow(position, txt, GetColor(color).color, moveCurve, scaleCurve, speed, spread);
        index++;
    }

    #region Color
    public enum ColorType { White, Red, Orange, Yellow, Green, Blue, Purple }

    [Space(16)]
    [SerializeField]
    private ColorClass[] colors;

    [System.Serializable]
    public class ColorClass
    {
        [HideInInspector]
        public string n;
        public ColorType colorType;
        public Color color;
    }

    private ColorClass GetColor(ColorType colorType)
    {
        for (int i = 0; i < colors.Length; i++)
        {
            if (colors[i].colorType == colorType)
            {
                return colors[i];
            }
        }
        return null;
    }
    #endregion
}
