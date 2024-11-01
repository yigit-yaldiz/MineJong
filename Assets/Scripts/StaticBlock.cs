using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseAssets;
using HighlightPlus;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(HighlightEffect))]
[System.Serializable]
public class StaticBlock : MonoBehaviour
{
    public BlockType BlockType { get => _type; set => _type = value; }
    public bool Available => _available;

    [SerializeField] BlockType _type;
    [SerializeField] LayerMask _blockLayerMask;

    bool _available = true;

    Coroutine _checkCoroutine;

    float _checkSphereRadius = 0.35f;
    Vector3 _capsuleStartPos;
    Vector3 _capsuleEndPos;

    HighlightEffect _effect;

    BlockState _state;

    HillDataHolder _hillDataHolder;

    private void Awake()
    {
        _hillDataHolder = GetComponentInParent<HillDataHolder>();
        _effect = GetComponent<HighlightEffect>();
    }

    private void OnEnable()
    {
        Managers.ClickManager.OnJump += CheckTheBlockAbove;
    }

    private void OnDisable()
    {
        Managers.ClickManager.OnJump -= CheckTheBlockAbove;
    }

    void Start()
    {
        _capsuleStartPos = transform.position + (Vector3.up / 1.1f);
        _capsuleEndPos = transform.position + Vector3.up * 4;
        CheckTheBlockAbove();
    }

    public void SpawnBlock()
    {
        GameObject blockGameObject = PoolManager.Instance.GetPoolObject(_type.ToString());
        Block block = blockGameObject.GetComponent<Block>();
        
        block.Initial(this);
        blockGameObject.transform.position = transform.position;
        block.StartPosition = transform.position;
        block.JumpToAvailablePoint();
        ChangeMeshCondition(false);
        _state = BlockState.Snapped;
    }

    public void ChangeMeshCondition(bool condition)
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = condition;

        MeshCollider collider = GetComponent<MeshCollider>();
        collider.enabled = false;

        if (transform.childCount != 0)
        {
            MeshRenderer[] childMeshs = GetComponentsInChildren<MeshRenderer>();

            foreach (var item in childMeshs)
            {
                item.enabled = condition;
            }
        }
    }

    void CheckTheBlockAbove(float waitTime = 0)
    {
        if (_state == BlockState.Ready) return;

        if (_checkCoroutine != null)
        {
            StopCoroutine(_checkCoroutine);
        }

        _checkCoroutine = StartCoroutine(Delay(waitTime));

        IEnumerator Delay(float delay)
        {
            yield return new WaitForSeconds(delay);

            bool condition = Physics.CheckCapsule(_capsuleStartPos, _capsuleEndPos, _checkSphereRadius, _blockLayerMask);

            if (condition)
            {
                ChangeAvailableCondition(false);
            }
            else
            {
                ChangeAvailableCondition(true);
            }
        }
    }

    public void ChangeAvailableCondition(bool available)
    {
        if (available)
        {
            _available = true;
            _effect.highlighted = false;
        }
        else
        {
            _available = false;
            _effect.highlighted = true;
        }
    }

    public void DeactivateBlock(bool isItLoad = false)
    {
        if (!isItLoad)
        { 
            _hillDataHolder.AddClosedStaticBlock(this);
        }
        gameObject.SetActive(false);
    }

    //private void OnDrawGizmos()
    //{
    //    _capsuleStartPos = transform.position + (Vector3.up / 1.1f);
    //    _capsuleEndPos = transform.position + Vector3.up * 4;

    //    Gizmos.color = Color.cyan;
    //    Gizmos.DrawLine(_capsuleStartPos, _capsuleEndPos);
    //    Gizmos.DrawWireSphere(_capsuleStartPos, _checkSphereRadius);
    //}
}
