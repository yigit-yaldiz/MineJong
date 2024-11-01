using BaseAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(1)]
public class RequirementButton : MonoBehaviour
{
    public BlockType BlockType { get => _blockType; set => _blockType = value; }
    public SpawnPoint SpawnPoint => _spawnPoint;

    BlockType _blockType;

    Image _image;
    TMP_Text _countText;

    SpawnPoint _spawnPoint;

    Button _button;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _countText = GetComponentInChildren<TMP_Text>();
        _spawnPoint = GetComponentInChildren<SpawnPoint>();
        _button = GetComponentInParent<Button>();
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);

        if (Managers.ReadyBlockManager.Instance.PullReadyBlockCount(_blockType) <= 0)
        {
            GetUninteractable();
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

    public void SortBlock()
    {
        if (!Managers.FurnaceManager.Instance.IsAnyFurnaceActive)
        {
            return;
        }

        Block block = SpawnBlock(_blockType.ToString());
        SendBlockToQueuePoint(block);
        Managers.ReadyBlockManager.Instance.DecreaseReadyBlock(block.GetBlockType);

        if (Managers.ReadyBlockManager.Instance.PullReadyBlockCount(_blockType) <= 0)
        {
            GetUninteractable();
        }
    }

    void SendBlockToQueuePoint(Block block)
    {
        block.GoToQueuePoint();
    }

    Block SpawnBlock(string id)
    {
        Block block = PoolManager.Instance.GetPoolObject(id).GetComponent<Block>();
        block.transform.position = _spawnPoint.transform.position;
        return block;
    }

    void GetUninteractable()
    {
        _button.interactable = false;

        Color imageColor = _image.color;
        imageColor.a = 0.5f;
        _image.color = imageColor;

        Color textColor = _countText.color;
        textColor.a = 0.5f;
        _countText.color = textColor;
    }
}
