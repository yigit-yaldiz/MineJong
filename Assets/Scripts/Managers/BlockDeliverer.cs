using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseAssets;

namespace Managers
{
    [DefaultExecutionOrder(1)]
    public class BlockDeliverer : MonoBehaviour
    {
        [SerializeField] BlocksSO _blockSO;
        [SerializeField] Building _currentBuilding;
        [SerializeField] Hill _currentHill;

        public static BlockDeliverer Instance { get; private set; }

        BlockType[] _cookedBlockTypes = { BlockType.GoldBlock, BlockType.IronBlock, BlockType.DiamondBlock, BlockType.EmeraldBlock, BlockType.RedStoneBlock };

        private void Awake()
        {
            Instance = this;
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.3f);

            if (GameManager.Instance.GameState == GameState.Collect)
            {
                SpawnHillBlocks();
            }
            else if (GameManager.Instance.GameState == GameState.Build)
            {
                _currentBuilding = RequirementManager.Instance.CurrentBuilding;

                if (_currentBuilding.IsBuild)
                {
                    UILoader.Instance.ActivateCompletedPanel();
                    yield break;
                }

                SpawnRequiredBlocks();
            }
            else if (GameManager.Instance.GameState == GameState.Furnace)
            {
                SpawnFurnaceBlocks();
            }
        }

        void SpawnHillBlocks()
        {
            _currentHill = HillManager.Instance.ActiveHill;

            foreach (var item in _currentHill.BlockCountsByType)
            {
                PoolManager.Instance.CreatePool(PullBlock(item.Key), 6, item.Key.ToString());
            }
        }

        public void SpawnRequiredBlocks()
        {
            foreach (KeyValuePair<BlockType, int> item in ReadyBlockManager.Instance.ReadyBlocks)
            {
                if (item.Value > 10)
                {
                    PoolManager.Instance.CreatePool(PullBlock(item.Key), 10, item.Key.ToString());
                }
                else
                {
                    PoolManager.Instance.CreatePool(PullBlock(item.Key), item.Value, item.Key.ToString());
                }
            }

            if (_currentBuilding.RequiredTotalBlockCounts.ContainsKey(BlockType.Stairs))
            {
                for (int i = 0; i < _blockSO.Blocks.Count; i++)
                {
                    if (_currentBuilding.RequiredTotalBlockCounts[BlockType.Stairs] > 10)
                    {
                        PoolManager.Instance.CreatePool(PullBlock(BlockType.Stairs), 10, BlockType.Stairs.ToString());
                    }
                    else
                    {
                        int count = _currentBuilding.RequiredTotalBlockCounts[BlockType.Stairs];
                        PoolManager.Instance.CreatePool(PullBlock(BlockType.Stairs), count, BlockType.Stairs.ToString());
                    }
                }
            }
        }

        void SpawnFurnaceBlocks()
        {
            foreach (BlockType type in FurnaceManager.Instance.ReadyCookableTypes)
            {
                PoolManager.Instance.CreatePool(PullBlock(type), 10, type.ToString());
                PoolManager.Instance.CreatePool(PullBlock(type.ToString() + "Block"), 18, type.ToString() + "Block");
            }
        }

        public void CreatePool(BlockType type, int count)
        {
            for (int i = 0; i < _blockSO.Blocks.Count; i++)
            {
                PoolManager.Instance.CreatePool(PullBlock(type), count, BlockType.Stairs.ToString()); 
            }
        }

        GameObject PullBlock(BlockType type)
        {
            GameObject target = null;

            for (int i = 0; i < _blockSO.Blocks.Count; i++)
            {
                if (type == _blockSO.Blocks[i].GetBlockType)
                {
                    target = _blockSO.Blocks[i].gameObject;
                }
            }

            return target;
        }

        GameObject PullBlock(string id)
        {
            GameObject target = null;

            foreach (var block in _blockSO.Blocks)
            {
                if (id == block.GetBlockType.ToString())
                {
                    target = block.gameObject;
                }
            }

            return target;
        }
    }
}
