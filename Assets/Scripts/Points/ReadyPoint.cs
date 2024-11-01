using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyPoint : MonoBehaviour
{
    public BlockType BlockType { get => _blockType; set => _blockType = value; }
    BlockType _blockType;
    
    void Start()
    {
        Managers.ReadyPointManager.Instance.AddReadyPoints(this);
    }

}
