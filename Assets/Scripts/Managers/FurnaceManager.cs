using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class FurnaceManager : MonoBehaviour
    {
        public static FurnaceManager Instance { get; private set; }

        #region Properties
        public bool IsAnyFurnaceActive { get => _isAnyFurnaceActive; set => _isAnyFurnaceActive = value; }
        public List<Furnace> Furnaces  => _furnaces;
        public List<BlockType> ReadyCookableTypes => _readyCookableTypes;
        public BlockType[] CookedBlockTypes => _cookedBlockTypes;
        #endregion

        [SerializeField] List<BlockType> _readyCookableTypes = new();
        
        List<Furnace> _furnaces = new();
        bool _isAnyFurnaceActive;

        BlockType[] _cookableBlockTypes = { BlockType.Diamond, BlockType.Emerald, BlockType.Gold, BlockType.Iron, BlockType.RedStone, BlockType.Sand };
        BlockType[] _cookedBlockTypes = { BlockType.DiamondBlock, BlockType.EmeraldBlock, BlockType.GoldBlock, BlockType.IronBlock, BlockType.RedStoneBlock , BlockType.Glass};

        private void Awake()
        {
            Instance = this;

            foreach (Transform child in transform)
            {
                if (child.TryGetComponent(out Furnace furnace))
                {
                    _furnaces.Add(furnace);
                }
            }
        }

        private void Start()
        {
            FindCookableReadyBlockTypes();
        }

        public void ActivateFurnace(int siblingIndex)
        {
            _isAnyFurnaceActive = true;
            _furnaces[siblingIndex].ActivateFurnace();
            CoinManager.Instance.DecreaseCoin(100);
        }

        bool CheckIsCookable(BlockType blockType)
        {
            foreach (var type in _cookableBlockTypes)
            {
                if (blockType == type)
                {
                    return true;
                }
            }

            return false;
        }

        public void FindCookableReadyBlockTypes()
        {
            Dictionary<BlockType, int> temp = new(ReadyBlockManager.Instance.ReadyBlocks);

            foreach (var item in temp)
            {
                if (CheckIsCookable(item.Key))
                {
                    _readyCookableTypes.Add(item.Key);
                }
            }
        }

        public int GetFurnaceIndex(Furnace furnace)
        {
            return _furnaces.IndexOf(furnace);
        }

        public Furnace GetNextFurnace(int currentFurnaceIndex)
        {
            return Furnaces[currentFurnaceIndex + 1];
        }
    }
}
