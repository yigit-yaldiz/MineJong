using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniqueGames.Saving;

[RequireComponent(typeof(SaveableEntity))]
public class Building : MonoBehaviour, ISaveable
{
    public bool IsBuild => _isBuild;
    public Dictionary<BlockType, int> RequiredTotalBlockCounts => _requiredTotalBlockCounts;

    public List<RequirementList> Lists { get => _lists; set => _lists = value; }

    List<RequirementList> _lists = new();

    bool _isBuild;

    Dictionary<BlockType, int> _requiredTotalBlockCounts = new();

    private void Awake()
    {
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out RequirementList list))
            {
                _lists.Add(list);
            }
        }
    }

    private void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out RequirementList list))
            {
                if (list.IsBuild)
                {
                    list.enabled = false;
                }
            }
        }

        CalculateRequiredBlocks();
    }

    void CalculateRequiredBlocks()
    {
        foreach (Transform group in transform)
        {
            if (group.TryGetComponent(out BuildPoint cube))
            {
                if (_requiredTotalBlockCounts.ContainsKey(cube.BlockType))
                {
                    _requiredTotalBlockCounts[cube.BlockType]++;
                }
                else
                {
                    _requiredTotalBlockCounts.Add(cube.BlockType, 1);
                }
            }
            else
            {
                foreach (Transform child in group)
                {
                    if (child.TryGetComponent(out BuildPoint block))
                    {
                        if (_requiredTotalBlockCounts.ContainsKey(block.BlockType))
                        {
                            _requiredTotalBlockCounts[block.BlockType]++;
                        }
                        else
                        {
                            _requiredTotalBlockCounts.Add(block.BlockType, 1);
                        }
                    }
                }
            }
        }
    }

    public void CheckFinished()
    {
        int completedCount = 0;

        foreach (var list in _lists)
        {
            if (list.IsBuild)
            {
                completedCount++;

                if (completedCount == _lists.Count)
                {
                    _isBuild = true;
                    //Managers.ReadyBlockManager.Instance.ClearReadyBlocks();
                    //Managers.UILoader.Instance.ActivateCompletedPanel();
                    //Saving.SaveWrapper.Instance.Save();
                }

                continue;
            }
        }
    }

    [System.Serializable]
    struct SaveData
    {
        public bool IsBuild;
    }

    public object CaptureState()
    {
        SaveData saveData = new();
        saveData.IsBuild = _isBuild;

        return saveData;
    }

    public void RestoreState(object state)
    {
        SaveData saveData = (SaveData)state;
        _isBuild = saveData.IsBuild;
    }
}
