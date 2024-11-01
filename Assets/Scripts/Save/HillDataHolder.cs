using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniqueGames.Saving;
using System.Linq;

[RequireComponent(typeof(SaveableEntity))]
public class HillDataHolder : MonoBehaviour, ISaveable
{
    HashSet<int> _closedStaticBlocks = new();
    
    public void AddClosedStaticBlock(StaticBlock block)
    {
        _closedStaticBlocks.Add(block.transform.GetSiblingIndex());
    }

    [System.Serializable]
    struct SaveData
    {
        public int[] ClosedStaticBlocks;
    }

    public object CaptureState()
    {
        SaveData saveData = new();
        saveData.ClosedStaticBlocks = _closedStaticBlocks.ToArray();

        return saveData;
    }

    public void RestoreState(object state)
    {
        SaveData saveData = (SaveData)state;
        _closedStaticBlocks = new(saveData.ClosedStaticBlocks);

        foreach (int blockIndex in _closedStaticBlocks)
        {
            transform.GetChild(blockIndex).GetComponent<StaticBlock>().DeactivateBlock(true);
        }
    }
}
