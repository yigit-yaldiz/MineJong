using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIndicatorExample : MonoBehaviour
{
    public bool world;

    private float t;
    public float seconds;
    void Update()
    {
        t += Time.deltaTime;
        if (t >= seconds)
        {
            if (world)
            {
                DamageIndicator.scr.Show(transform.position, "10", DamageIndicator.ColorType.Red);
            }
            else
            {
                DamageIndicator.scr.Show(Camera.main.WorldToScreenPoint(transform.position), "10", DamageIndicator.ColorType.Green);
            }
            t = 0;
        }
    }
}
