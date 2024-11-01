using BaseAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCounter : MonoBehaviour
{
    [SerializeField] int _blockCount;
    [SerializeField] List<BlockGroup> _blockGroups = new();

    private void Start()
    {
        AddBlocks();
    }

    void AddBlocks()
    {
        foreach (Transform group in transform)
        {
            if (group.TryGetComponent(out BuildPoint cube))
            {
                if (ContainsTypeCheck(cube.BlockType))
                {
                    IncreaseCount(cube.BlockType);
                }
                else
                {
                    AddType(cube.BlockType);
                }
            }
            else
            {
                foreach (Transform child in group)
                {
                    if (child.TryGetComponent(out BuildPoint block))
                    {
                        if (ContainsTypeCheck(block.BlockType))
                        {
                            IncreaseCount(block.BlockType);
                        }
                        else
                        {
                            AddType(block.BlockType);
                        }
                    }
                }
            }
        }
    }

    bool ContainsTypeCheck(BlockType type)
    {
        foreach (var item in _blockGroups)
        {
            if (item.BlockType == type)
            {
                return true;
            }
        }

        return false;
    }

    
    void AddType(BlockType type)
    {
        _blockGroups.Add(new BlockGroup(type));
    }

    void IncreaseCount(BlockType type)
    {
        foreach (BlockGroup item in _blockGroups)
        {
            if (item.BlockType == type)
            {
                item.Count++;
            }
        }
    }

    [System.Serializable]
    class BlockGroup
    {
        [HideInInspector] public string Name;
        public BlockType BlockType;
        public int Count = 0;

        public BlockGroup(BlockType blockType)
        {
            BlockType = blockType;
            Count++;
            Name = BlockType.ToString();
        }
    }

    [Button]
    void CountBlocks()
    {
        int count = 0;

        foreach (Transform group in transform)
        {
            if (group.TryGetComponent(out BuildPoint cube))
            {
                count++;
            }
            else
            {
                foreach (Transform child in group)
                {
                    if (child.TryGetComponent(out BuildPoint block))
                    {
                        count++;
                    }
                }
            }

            _blockCount = count;
        }
    }

}
