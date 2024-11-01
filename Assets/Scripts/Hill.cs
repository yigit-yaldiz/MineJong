using System.Collections.Generic;
using UnityEngine;
using BaseAssets;

public class Hill : MonoBehaviour
{
    #region Properties
    public bool IsFinished { get => _isFinished; set => _isFinished = value; }
    public int BlockCount { get => _blockCount; set => _blockCount = value; }
    public List<BlockType> BlockTypes { get => _blockTypes; set => _blockTypes = value; }
    public Dictionary<BlockType, int> BlockCountsByType { get => _blockCountsByType; set => _blockCountsByType = value; }
    #endregion

    bool _isFinished;

    [SerializeField] int _blockCount;
    [SerializeField] List<BlockGroup> _blockGroups = new();

    Dictionary<BlockType, int> _blockCountsByType = new();

    List<BlockType> _blockTypes = new();

    private void Start()
    {
        FindBlockTypes();
        AddStaticBlocks();
    }

    void FindBlockTypes()
    {
        foreach (Transform group in transform)
        {
            if (group.TryGetComponent(out StaticBlock block))
            {
                if (!_blockTypes.Contains(block.BlockType))
                {
                    _blockTypes.Add(block.BlockType);
                }
            }
            else
            {
                foreach (Transform child in group)
                {
                    if (child.TryGetComponent(out StaticBlock childBlock))
                    {
                        if (!_blockTypes.Contains(childBlock.BlockType))
                        {
                            _blockTypes.Add(childBlock.BlockType);
                        }
                    }
                }
            }
        }
    }

    void AddStaticBlocks()
    {
        foreach (Transform group in transform)
        {
            if (group.TryGetComponent(out StaticBlock cube))
            {
                if (_blockCountsByType.ContainsKey(cube.BlockType))
                {
                    _blockCountsByType[cube.BlockType]++;
                    IncreaseCount(cube.BlockType);
                }
                else
                {
                    _blockCountsByType.Add(cube.BlockType, 1);
                    AddType(cube.BlockType);
                }
            }
            else
            {
                foreach (Transform child in group)
                {
                    if (child.TryGetComponent(out StaticBlock block))
                    {
                        if (_blockCountsByType.ContainsKey(block.BlockType))
                        {
                            _blockCountsByType[block.BlockType]++;
                            IncreaseCount(block.BlockType);
                        }
                        else
                        {
                            _blockCountsByType.Add(block.BlockType, 1);
                            AddType(block.BlockType);
                        }
                    }
                }
            }
        }
    }

    [Button]
    void CountBlocks()
    {
        int count = 0;

        foreach (Transform group in transform)
        {
            if (group.TryGetComponent(out StaticBlock cube))
            {
                count++;
            }
            else
            {
                foreach (Transform child in group)
                {
                    if (child.TryGetComponent(out StaticBlock block))
                    {
                        count++;
                    }
                }
            }

            _blockCount = count;
        }
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
}
