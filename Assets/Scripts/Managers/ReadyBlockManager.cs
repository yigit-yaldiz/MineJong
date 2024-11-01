using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniqueGames.Saving;
using BaseAssets;

namespace Managers
{
    [RequireComponent(typeof(SaveableEntity))]
    public class ReadyBlockManager : MonoBehaviour, ISaveable
    {
        public static ReadyBlockManager Instance { get; private set; }
        public Dictionary<BlockType, int> ReadyBlocks { get => _readyBlocks; set => _readyBlocks = value; }

        [SerializeField] List<BlockGroups> _serializableReadyBlocks = new();

        Dictionary<BlockType, int> _readyBlocks = new();
        Dictionary<BlockType, int> _tempReadyBlocks = new();

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            SerializeReadyBlocks();
        }

        [Button]
        void DebugReadyBlocks()
        {
            if (_readyBlocks.Count == 0)
            {
                Debug.LogWarning("ReadyBlocks is empty");
                return;
            }

            foreach (KeyValuePair<BlockType, int> item in _readyBlocks)
            {
                Debug.Log(item.Key + " " + item.Value);
            }
        }

        public void AddReadyBlock(Block block)
        {
            if (_readyBlocks.ContainsKey(block.GetBlockType))
            {
                _readyBlocks[block.GetBlockType]++;

                foreach (var item in _serializableReadyBlocks)
                {
                    if (item.Type == block.GetBlockType)
                    {
                        item.Count++;
                    }
                }
            }
            else
            {
                List<Block> value = new();
                value.Add(block);
                _readyBlocks.Add(block.GetBlockType, 1);

                _serializableReadyBlocks.Add(new(block.GetBlockType, 1));
            }

            if (GameManager.Instance.GameState == GameState.Furnace)
            {
                UIManager.Instance.PrintBlockCounts(delay: 1);
                return;
            }

            UIManager.Instance.PrintBlockCounts(delay: 2);
        }

        public void AddReadyBlockType(BlockType blockType)
        {
            if (!_readyBlocks.ContainsKey(blockType))
            {
                _readyBlocks.Add(blockType, 0);
            }
        }

        public void DecreaseReadyBlock(BlockType type)
        {
            if (_readyBlocks.ContainsKey(type))
            {
                if (type == BlockType.Stairs)
                {
                    type = BlockType.Plank;
                }

                if (_readyBlocks[type] > 0)
                {
                    _readyBlocks[type]--;

                    foreach (var item in _serializableReadyBlocks)
                    {
                        if (item.Type == type)
                        {
                            item.Count--;
                        }
                    }
                }
            }

            if (GameManager.Instance.GameState == GameState.Furnace)
            {
                UIManager.Instance.PrintBlockCounts();
            }
        }

        public void ClearReadyBlocks()
        {
            _readyBlocks.Clear();
            Debug.LogWarning("Ready Blocks Cleared");
        }

        public void FillTempBlocks()
        {
            _tempReadyBlocks = new();

            foreach (var item in _readyBlocks)
            {
                if (_tempReadyBlocks.ContainsKey(item.Key))
                {
                    _tempReadyBlocks[item.Key] = item.Value;
                }
                else
                {
                    _tempReadyBlocks.Add(item.Key, item.Value);
                }
            }
        }

        public void PullTempReadyBlocks()
        {
            if (_tempReadyBlocks == null || _tempReadyBlocks.Count == 0)
            {
                Debug.LogWarning("Temp blocks is null or empty");
                ClearReadyBlocks();
                return;
            }
            
            Dictionary<BlockType, int> temp = new(_tempReadyBlocks);

            foreach (var item in temp)
            {
                if (_readyBlocks.ContainsKey(item.Key))
                {
                    _readyBlocks[item.Key] = item.Value;
                }
                else
                {
                    _readyBlocks.Add(item.Key, item.Value);
                }   
            }
        }

        public int PullReadyBlockCount(BlockType blockType)
        {
            return _readyBlocks[blockType];
        }

        void SerializeReadyBlocks()
        {
            foreach (KeyValuePair<BlockType, int> kvp in _readyBlocks)
            {  
                _serializableReadyBlocks.Add(new(kvp.Key, kvp.Value));
            }
        }

        [System.Serializable]
        class BlockGroups
        {
            [HideInInspector] public string Name;
            public BlockType Type;
            public int Count;

            public BlockGroups(BlockType type, int rate)
            {
                Type = type;
                Name = Type.ToString();
                Count += rate;
            }
        }

        [System.Serializable]
        struct SaveData
        {
            public Dictionary<BlockType, int> ReadyBlocks;
            public Dictionary<BlockType, int> TempReadyBlocks;
        }

        public object CaptureState()
        {
            SaveData saveData = new SaveData();
            saveData.ReadyBlocks = _readyBlocks;
            saveData.TempReadyBlocks = _tempReadyBlocks;

            return saveData;
        }

        public void RestoreState(object state)
        {
            SaveData saveData = (SaveData)state;
            _readyBlocks = saveData.ReadyBlocks;
            _tempReadyBlocks = saveData.TempReadyBlocks;
        }
    }
}
