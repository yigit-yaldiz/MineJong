using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    public bool Occupied => _occupied;
    public Block Block => _block;

    [SerializeField] Block _block;
    [SerializeField] bool _occupied;

    private void Start()
    {
        Managers.SnapPointManager.Instance.AddSnapPoint(this);
    }

    public void Snap(Block block)
    {
        Managers.SnapPointManager.Instance.AddSnappedBlock(block);
        _block = block;
        _occupied = true;
    }

    public void MakeUnoccupied()
    {
        _block = null;
        _occupied = false;
    }
}
