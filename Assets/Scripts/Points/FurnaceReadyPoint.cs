using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnaceReadyPoint : MonoBehaviour
{
    public bool Occupied => _occupied;

    [SerializeField] bool _occupied;

    public void MakeOccupied(bool occupied)
    {
        _occupied = occupied;
    }
}
