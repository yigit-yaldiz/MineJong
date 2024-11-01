using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saving;
using TMPro;

namespace Managers
{
    public class SnapPointManager : MonoBehaviour
    {
        public static SnapPointManager Instance { get; private set; }
        public List<SnapPoint> SnapPoints => _snapPoints;
        public List<Block> SnappedBlocks => _snappedBlocks;

        [SerializeField] List<Block> _snappedBlocks = new();

        [SerializeField] TMP_Text _savedText;

        List<SnapPoint> _snapPoints = new();
        Dictionary<BlockType, List<Block>> _snappedBlocksByType = new();

        Coroutine _saveCoroutine;

        private void Awake()
        {
            Instance = this;
        }

        public void AddSnapPoint(SnapPoint snapPoint)
        {
            _snapPoints.Add(snapPoint);
            _snapPoints = _snapPoints.OrderBy(t => t.transform.GetSiblingIndex()).ToList();
        }

        public void AddSnappedBlock(Block block)
        {
            _snappedBlocks.Add(block);

            if (_snappedBlocksByType.ContainsKey(block.GetBlockType))
            {
                _snappedBlocksByType[block.GetBlockType].Add(block);
            }
            else
            {
                List<Block> value = new();
                value.Add(block);
                _snappedBlocksByType.Add(block.GetBlockType, value);
            }

            CheckSameBlocks(count: 3);
        }

        void CheckSameBlocks(int count)
        {
            Dictionary<BlockType, List<Block>> temp = new(_snappedBlocksByType);

            foreach (KeyValuePair<BlockType, List<Block>> kvp in temp)
            {
                if (kvp.Value.Count == count)
                {
                    List<Block> readyBlocks = new(_snappedBlocksByType[kvp.Key]);

                    foreach (Block block in readyBlocks)
                    {
                        block.GoToReadyPoint();
                        _snappedBlocks.Remove(block);
                    }

                    if (_saveCoroutine != null)
                    {
                        StopCoroutine(_saveCoroutine);
                    }

                    _saveCoroutine = StartCoroutine(SaveWithDelay());
                    
                    //mine blocks may not be added to ready blocks
                    ReadyBlockManager.Instance.AddReadyBlock(readyBlocks[0]);
                    GameManager.Instance.IncreaseReadyBlockCount();
                    _snappedBlocksByType.Remove(kvp.Key);
                }
            }

            IEnumerator SaveWithDelay()
            {
                yield return new WaitForSeconds(2.25f);
                SaveWrapper.Instance.Save();
                _savedText.gameObject.SetActive(true);
                yield return new WaitForSeconds(1f);
                _savedText.gameObject.SetActive(false);
            }
        }
    }
}
