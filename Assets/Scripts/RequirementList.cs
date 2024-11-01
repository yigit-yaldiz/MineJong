using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using BaseAssets;
using UniqueGames.Saving;
using TMPro;

[RequireComponent(typeof(SaveableEntity))]
public class RequirementList : MonoBehaviour, ISaveable
{
    public Dictionary<BlockType, int> RequiredBlockCounts  => _requiredBlockCounts;
    public bool IsBuild => _isBuild;

    [SerializeField] AnimationCurve _jumpCurve;
    [SerializeField] List<BlockGroup> _requiredBlocks = new();
    
    Dictionary<BlockType, int> _requiredBlockCounts = new();
    
    Dictionary<int, BlockType> _nonSavedBlocks = new();
    Dictionary<BlockType, List<int>> _savedBlocks = new(); 

    BuildPoint[] _buildPoints;

    bool _isBuild;
    bool _isSaved;

    private void Awake()
    {
        _buildPoints = GetComponentsInChildren<BuildPoint>();
    }

    private void OnEnable()
    {
        Managers.ClickManager.OnBuild += Build;
    }

    private void OnDisable()
    {
        Managers.ClickManager.OnBuild -= Build;
    }

    IEnumerator Start()
    {
        AddSerializableBlocks();
        ActivateSavedBlocks();
        
        yield return new WaitForSeconds(0.3f);

        CalculateRequiredBlockCount();
        DetectIndexsOfBlocks();
    }

    public void ActivateSavedBlocks()
    { 
        if (_savedBlocks.Count == 0)
        {
            Debug.Log("Saved blocks is empty", transform.parent);
            return;
        }

        foreach (var item in _savedBlocks)
        {
            foreach (var siblingIndex in item.Value)
            {
                _buildPoints[siblingIndex].ChangeMeshCondition(true);
            }
        }
    }

    void CalculateRequiredBlockCount()
    {
        if (_isBuild) return;

        foreach (BuildPoint point in _buildPoints)
        {
            if (_requiredBlockCounts.ContainsKey(point.BlockType))
            {
                _requiredBlockCounts[point.BlockType]++;
            }
            else
            {
                _requiredBlockCounts.Add(point.BlockType, 1);
            }
        }
    }

    [Button]
    void PrintRequiredProducts()
    {
        foreach (KeyValuePair<BlockType, int> item in _requiredBlockCounts)
        {
            Debug.Log(item.Key + " " + item.Value);
        }
    }

    void DetectIndexsOfBlocks()
    {
        if (_nonSavedBlocks.Count != 0) return;

        foreach (Transform child in transform)
        {
            BuildPoint point = child.GetComponent<BuildPoint>();
            _nonSavedBlocks.Add(child.GetSiblingIndex(), point.BlockType);
        }
    }

    void DecreaseBlockCount(BlockType type)
    {
        if (_requiredBlockCounts[type] <= 0) return;

        _requiredBlockCounts[type]--;

        DecreaseReadyBlockCount(type);

        CheckCompleted();
    }

    void DecreaseReadyBlockCount(BlockType type)
    {
        if (type == BlockType.Stairs)
        {
            type = BlockType.Plank;
        }

        Managers.ReadyBlockManager.Instance.DecreaseReadyBlock(type);
        Managers.UIManager.Instance.PrintBlockCounts();
    }

    void CheckCompleted()
    {
        Building building = GetComponentInParent<Building>();

        if (_nonSavedBlocks.Count == 0)
        {
            _isBuild = true;
            Managers.RequirementManager.Instance.SetCurrentList();
            building.CheckFinished();
        }
    }

    void AddSaveableBlocks(BlockType type, int siblingIndex)
    {
        if (_savedBlocks.ContainsKey(type))
        {
            _savedBlocks[type].Add(siblingIndex);
        }
        else
        {
            List<int> temp = new();
            temp.Add(siblingIndex);
            _savedBlocks.Add(type, temp);
        }
    }

    void Build()
    {
        float jumpDuration = 0.4f;
        Vector3 startScale = Vector3.one * 0.2f;
        Dictionary<int, BlockType> temp = new(_nonSavedBlocks);
        Ease scaleEase = Ease.InCirc;

        foreach (KeyValuePair<int, BlockType> item in temp)
        {
            if (item.Value != BlockType.Stairs)
            {
                if (!Managers.ReadyBlockManager.Instance.ReadyBlocks.ContainsKey(item.Value) || Managers.ReadyBlockManager.Instance.ReadyBlocks[item.Value] <= 0)
                {
                    continue;
                }
            }

            Transform target = transform.GetChild(item.Key);

            string blockTypeID = item.Value.ToString();
            GameObject goingToJump = PoolManager.Instance.GetPoolObject(blockTypeID);
            Block block = goingToJump.GetComponent<Block>();

            CheckStartRotation(block, target);

            goingToJump.transform.localScale = startScale;

            goingToJump.transform.position = SetSpawnPoint(blockTypeID).position; //changeStartPoint
            Vector3 startPos = goingToJump.transform.position;

            //jump to buildPoint
            goingToJump.transform.DOJump(target.position, jumpPower: 4, numJumps: 1, jumpDuration).SetEase(_jumpCurve).OnStart(() =>
            {
                if (block.GetBlockType == BlockType.Stairs)
                {
                    goingToJump.transform.DOScale(new Vector3(0.5f, 0.25f, 0.25f), jumpDuration);
                }
                else
                {
                    goingToJump.transform.DOScale(0.45f, jumpDuration);
                }
            }) 
            .OnComplete(() =>
            {
                float scaleDuration = jumpDuration / 4;

                if (block.GetBlockType == BlockType.Stairs)
                {
                    goingToJump.transform.DOScale(new Vector3(1f, 0.5f, 0.5f), scaleDuration).SetEase(scaleEase).OnComplete(() =>
                    {
                        _buildPoints[item.Key].ChangeMeshCondition(true);
                        goingToJump.transform.localScale = startScale;

                        PoolManager.Instance.PutPoolObject(goingToJump, blockTypeID);
                        goingToJump.SetActive(false);
                        goingToJump.transform.position = startPos;
                    });
                }
                else
                {
                    goingToJump.transform.DOScale(1f, scaleDuration).SetEase(scaleEase).OnComplete(() => 
                    {
                        _buildPoints[item.Key].ChangeMeshCondition(true);
                        goingToJump.transform.localScale = startScale;

                        PoolManager.Instance.PutPoolObject(goingToJump, blockTypeID);
                        goingToJump.SetActive(false);
                        goingToJump.transform.position = startPos;
                    });
                }
            });

            AddSaveableBlocks(item.Value, item.Key);
            _nonSavedBlocks.Remove(item.Key);
            DecreaseBlockCount(item.Value);
            return; 
        }

        int completedCount = 0;

        if (Managers.RequirementManager.Instance.CheckNextListCompletion(this))
        {
            Managers.UILoader.Instance.ActivateCompletedPanel();
            Managers.ReadyBlockManager.Instance.FillTempBlocks();
            
            if (!_isSaved)
            {
                Saving.SaveWrapper.Instance.Save();
                _isSaved = true;
            }   
        }
        else
        {
            SellBlock(completedCount);
        }
    }

    void SellBlock(int count)
    {
        float jumpDuration = 0.6f;

        foreach (var item in Managers.ReadyBlockManager.Instance.ReadyBlocks)
        {
            if (item.Value <= 0)
            {
                count++;

                if (count == Managers.ReadyBlockManager.Instance.ReadyBlocks.Count)
                {
                    Managers.UILoader.Instance.ActivateCompletedPanel();
                    Managers.ReadyBlockManager.Instance.ClearReadyBlocks();
                    Saving.SaveWrapper.Instance.Save();
                    return;
                }

                continue;
            }

            string blockTypeID = item.Key.ToString();
            Transform target = CoinPoint.Instance.transform;
            
            GameObject toSell = PoolManager.Instance.GetPoolObject(blockTypeID);
            Block block = toSell.GetComponent<Block>();

            toSell.transform.position = SetSpawnPoint(blockTypeID).position;
            Vector3 startPos = toSell.transform.position;

            if (block.GetBlockType == BlockType.Stairs)
            {
                toSell.transform.localScale = new(1, 0.5f, 0.5f);
            }
            else
            {
                toSell.transform.localScale = Vector3.one;
            }

            toSell.transform.DOJump(target.position, 2, 1, jumpDuration).SetEase(_jumpCurve).OnStart(() => 
            {
                toSell.transform.DOScale(0.2f, jumpDuration);
            }).OnComplete(()=> 
            {
                Managers.CoinManager.Instance.IncreaseCoin(increaseRate: 10);

                DecreaseReadyBlockCount(item.Key);
                
                PoolManager.Instance.PutPoolObject(toSell, blockTypeID);
                toSell.SetActive(false);
                toSell.transform.position = startPos;
            });

            break;
        }
    }

    Transform SetSpawnPoint(string id)
    {
        Transform target = null;

        foreach (var item in Managers.RequirementPanel.Instance.RequirementUIs)
        {
            if (item.BlockType.ToString() == id)
            {
                target = item.SpawnPoint.transform;
            }
            else if (item.BlockType == BlockType.Stairs)
            {
                foreach (var requirement in Managers.RequirementPanel.Instance.RequirementUIs)
                {
                    if (requirement.BlockType == BlockType.Plank)
                    {
                        target = requirement.SpawnPoint.transform;
                    }
                }
            }
        }

        return target;
    }

    void CheckStartRotation(Block block, Transform target)
    {
        if (block.GetBlockType == BlockType.Stairs)
        {
            block.transform.rotation = target.rotation;
        }
    }

    void AddSerializableBlocks()
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
        foreach (var item in _requiredBlocks)
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
        _requiredBlocks.Add(new BlockGroup(type));
    }

    void IncreaseCount(BlockType type)
    {
        foreach (BlockGroup item in _requiredBlocks)
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

    [System.Serializable]
    struct SaveData
    {
        public Dictionary<BlockType, int> RequiredBlockCounts;
        public Dictionary<int, BlockType> NonSavedBlocks;
        public Dictionary<BlockType, List<int>> SavedBlocks;
        public bool IsBuild;
    }

    public object CaptureState()
    {
        SaveData saveData = new();
        saveData.RequiredBlockCounts = _requiredBlockCounts;
        saveData.NonSavedBlocks = _nonSavedBlocks;
        saveData.SavedBlocks = _savedBlocks;
        saveData.IsBuild = _isBuild;

        return saveData;
    }

    public void RestoreState(object state)
    {
        SaveData saveData = (SaveData)state;
        _requiredBlockCounts = saveData.RequiredBlockCounts;
        _nonSavedBlocks = saveData.NonSavedBlocks;
        _savedBlocks = saveData.SavedBlocks;
        _isBuild = saveData.IsBuild;
    }
}
