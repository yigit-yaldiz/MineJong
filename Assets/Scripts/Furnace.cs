using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightPlus;
using UnityEngine.UI;
using BaseAssets;
using Managers;
using Saving;

public class Furnace : MonoBehaviour
{
    #region Properties
    public bool Occupied  => _occupied;
    public bool IsActive => _isActive;
    public List<QueuePoint> QueuePoints => _queuePoints;
    public List<FurnaceReadyPoint> ReadyPoints => _readyPoints;
    public bool AllReadyPointsOccupied { get => _allReadyPointsOccupied; set => _allReadyPointsOccupied = value; }
    #endregion

    [SerializeField] Button _button;
    [SerializeField] GameObject _front;
    [SerializeField] Material _onMaterial;

    #region Components
    Image _progressBar;
    Material _defaultMaterial;
    MeshRenderer _frontRenderer;
    HighlightEffect _effect;
    #endregion

    FurnacesDataHolder _dataHolder;
    
    List<QueuePoint> _queuePoints = new();
    List<FurnaceReadyPoint> _readyPoints = new();
    
    [SerializeField] List<Block> _cookedBlocks = new();
    Queue<Block> _cookableBlocks = new(); //this blocks on the queue points

    bool _occupied;
    bool _isActive;
    bool _allReadyPointsOccupied;

    const float _cookTime = 3f;
    const float _updateInterval = 0.05f;

    private void Awake()
    {
        _frontRenderer = _front.transform.GetComponent<MeshRenderer>();
        _effect = GetComponent<HighlightEffect>();
        _progressBar = GetComponentInChildren<Image>();
        _dataHolder = GetComponentInParent<FurnacesDataHolder>();

        foreach (Transform child in transform.GetChild(0))
        {
            if (child.TryGetComponent(out QueuePoint point))
            {
                _queuePoints.Add(point);
            }
        }

        foreach (Transform child in transform.GetChild(1))
        {
            if (child.TryGetComponent(out FurnaceReadyPoint point))
            {
                _readyPoints.Add(point);
            }
        }

        _defaultMaterial = _frontRenderer.material;
    }

    public void Cook(BlockType type)
    {
        if (!_isActive) return;

        StartCoroutine(CookWithDelay());

        IEnumerator CookWithDelay()
        {
            float elapsedTime = 0f;
            _occupied = true;
            _frontRenderer.material = _onMaterial;

            while (elapsedTime < _cookTime)
            {
                yield return new WaitForSeconds(_updateInterval);
                elapsedTime += _updateInterval;

                _progressBar.fillAmount = Mathf.Clamp01(elapsedTime / _cookTime);
                UpdateProgressBar(_progressBar.fillAmount);
            }

            UpdateProgressBar(1f);
            TakeItOut(type);

            _frontRenderer.material = _defaultMaterial;
            _occupied = false;

            UpdateProgressBar(0);
            CookFirstObject();
        }

        void UpdateProgressBar(float fillAmount)
        {
            if (_progressBar != null)
            {
                _progressBar.fillAmount = fillAmount;
            }
        }
    }

    public void ActivateFurnace()
    {
        _button.gameObject.SetActive(false);
        _isActive = true;
        FurnaceManager.Instance.IsAnyFurnaceActive = true;
        _effect.highlighted = false;

        _dataHolder.AddActivatedFurnace(this);
    }

    public void EnqueueBlock(Block block)
    {
        _cookableBlocks.Enqueue(block);
    }

    public void CookFirstObject()
    {
        if (_occupied) return;
        if(_cookableBlocks.Count == 0) return;

        CheckReadyPoints();
        
        if (_allReadyPointsOccupied)
        {
            Debug.LogWarning("All ready points are occupied");
            return;
        }

        Block block = _cookableBlocks.Dequeue();
        block.GoToFurnace(this);
    }

    void TakeItOut(BlockType type)
    {
        Block block = PoolManager.Instance.GetPoolObject(type.ToString() + "Block").GetComponent<Block>();
        block.transform.position = transform.position;
        block.GoToFurnaceReadyPoint(this);
        block.ReadyPoint.MakeOccupied(true);

        _cookedBlocks.Add(block);
    }

    public void MakeOccuppied(bool occupied)
    {
        _occupied = occupied;
    }

    void CheckReadyPoints()
    {
        int occupiedCount = 0;

        foreach (var readyPoint in _readyPoints)
        {
            if (readyPoint.Occupied)
            {
                occupiedCount++;

                if (occupiedCount == _readyPoints.Count)
                {
                    _allReadyPointsOccupied = true;
                }
            }
        }
    }

    public void SendReadyBlocks()
    {
        if (_cookedBlocks.Count == 0)
        {
            Debug.LogWarning("Cooked blocks are empty");
            return;
        }

        _cookedBlocks[0].GoToReadyPoint();
        ReadyBlockManager.Instance.AddReadyBlock(_cookedBlocks[0]);
        _cookedBlocks.RemoveAt(0);

        if (_cookedBlocks.Count == 0)
        {
            SaveWrapper.Instance.Save();
        }
    }
}
