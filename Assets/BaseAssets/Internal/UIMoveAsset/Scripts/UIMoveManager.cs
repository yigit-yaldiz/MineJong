using System;
using System.Collections;
using UnityEngine;

public class UIMoveManager : MonoBehaviour
{
    public static UIMoveManager Instance { get; private set; }
    public UIMover prefObject;
    public Canvas canvas;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PoolObjects();
    }

    public int count;
    private UIMover[] movers;
    private void PoolObjects()
    {
        movers = new UIMover[count];
        for (int i = 0; i < count; i++)
        {
            movers[i] = Instantiate(prefObject, canvas.transform);
            movers[i].gameObject.SetActive(false);
            movers[i].Init();
            movers[i].transform.localScale = Vector3.one;
        }
    }

    private int index;
    public void StartMove(Vector3 start, Vector3 end, float duration, float delay = 0, Sprite icon = null, Action onReached = null)
    {
        if (delay == 0)
        {
            movers[index % count].StartMove(start, end, duration, icon, onReached);
            index++;
        }
        else
        {
            StartCoroutine(MoveDelay(delay, start, end, duration, icon, onReached));
        }
    }

    IEnumerator MoveDelay(float delay, Vector3 start, Vector3 end, float duration, Sprite icon, Action onReached)
    {
        yield return new WaitForSecondsRealtime(delay);
        movers[index % count].StartMove(start, end, duration, icon, onReached);
        index++;
    }

    private void Update()
    {
        for (int i = 0; i < movers.Length; i++)
        {
            movers[i].Move();
        }
    }

    public int[] SplitValue(int count, int value)
    {
        int[] values = new int[count];
        for (int i = 0; i < value; i++)
        {
            values[i % count]++;
        }
        return values;
    }
}
