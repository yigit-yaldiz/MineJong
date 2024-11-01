using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Blocks", menuName = "Blocks")]
public class BlocksSO : ScriptableObject
{
    public List<Block> Blocks = new();
}
