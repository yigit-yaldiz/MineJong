using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RequirementUI : MonoBehaviour
{
    public BlockType BlockType { get => _blockType; set => _blockType = value; }
    public ReadyPoint ReadyPoint { get => _readyPoint; set => _readyPoint = value; }
    public SpawnPoint SpawnPoint  => _spawnPoint;

    BlockType _blockType;

    Image _image;
    TMP_Text _countText;

    ReadyPoint _readyPoint;
    SpawnPoint _spawnPoint;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _countText = GetComponentInChildren<TMP_Text>();
        _readyPoint = GetComponentInChildren<ReadyPoint>();

        if (_readyPoint == null)
        {
            _spawnPoint = GetComponentInChildren<SpawnPoint>();
        }
    }

    public void ChangeImage(Sprite sprite)
    {
        _image.sprite = sprite;
    }

    public void ChangeCountText(int count)
    {
        _countText.text = count.ToString();
    }
}
