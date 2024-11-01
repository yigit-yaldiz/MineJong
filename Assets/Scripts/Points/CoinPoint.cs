using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPoint : MonoBehaviour
{
    public static CoinPoint Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
}
