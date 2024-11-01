using System.Collections;
using UnityEngine;
using DG.Tweening;
using BaseAssets;
using Managers;

[System.Serializable]
public enum BlockState
{
    Idle,
    Snapped,
    Air,
    Ready
}

[System.Serializable]
public enum BlockType
{
    Grass,
    Dirt,
    Stone,
    CobbleStone,
    StoneBrick,
    EndStone,
    EndStoneBrick,
    SandStone,
    NetherRack,
    NetherQuartz,
    NetherBrick,
    Obsidian,
    GlowStone,
    Coal,
    Emerald,
    Gold,
    Iron,
    Diamond,
    LapisLazuli,
    RedStone,
    Plank,
    Mycelium,
    Granite,
    Gravel,
    Oak,
    Stairs,
    SprucePlank,
    MossyCobbleStone,
    IronBlock,
    GoldBlock,
    DiamondBlock,
    EmeraldBlock,
    RedStoneBlock,
    Sand,
    Glass,
    StoneBrickStairs,
    StoneBrickSlab,
    DarkOakPlank
}

public class Block : MonoBehaviour
{
    public Vector3 StartPosition { get;  set; }
    public SnapPoint ThePointISnapped { get; private set; }
    public BlockType GetBlockType => _blockType;
    public BlockState BlockState => _state;

    public FurnaceReadyPoint ReadyPoint { get; private set; }
    public QueuePoint QueuePoint { get; private set; }
    public Furnace Furnace { get; private set; }

    [SerializeField] BlockType _blockType;
    [SerializeField] AnimationCurve _jumpCurve;

    float _snapJumpDuration = 1f;
    const float _readyJumpDuration = 1f;

    const float _finalScalePercent = 0.4f;

    const float _targetTreshold = 0.1f;

    bool _willBeReady = false;
    
    BlockState _state;

    StaticBlock _staticBlock;

    public void Initial(StaticBlock block)
    {
        transform.localScale = Vector3.one;
        _staticBlock = block;
        _state = BlockState.Idle;
        _willBeReady = false;
    }

    Vector3 FindAvailablePoint()
    {
        SnapPoint point = null;

        int occupiedCount = 0;

        foreach (SnapPoint snapPoint in Managers.SnapPointManager.Instance.SnapPoints)
        {
            if (snapPoint.Occupied)
            {
                occupiedCount++;

                if (occupiedCount == Managers.SnapPointManager.Instance.SnapPoints.Count)
                {
                    Debug.LogWarning("There are no available points");
                    return transform.position;
                }

                continue;
            }
            else
            {
                point = snapPoint;
                ThePointISnapped = snapPoint;
                snapPoint.Snap(this);

                CheckFailed();

                break;
            }
        }

        Vector3 snapPointPos = new(point.transform.position.x, 0, point.transform.position.z);
        return snapPointPos;
    }

    public void JumpToAvailablePoint()
    {
        if (_state != BlockState.Idle) return;

        Vector3 targetPosition = FindAvailablePoint();
        bool isThereAPoint = Vector3.Distance(transform.position, targetPosition) > _targetTreshold;
        
        _state = BlockState.Air;
        gameObject.layer = LayerMask.NameToLayer("Default");

        transform.DOJump(targetPosition, jumpPower: 4, 1, _snapJumpDuration).SetEase(_jumpCurve).OnComplete(() =>
        {
            if (transform.position != StartPosition)
            {
                _state = BlockState.Snapped;
                return;
            }
            else
            {
                _state = BlockState.Idle;
                gameObject.layer = LayerMask.NameToLayer("Block");
            }
        }).OnStart(() => 
        {
            if (isThereAPoint)
            {
                Managers.ClickManager.OnJump?.Invoke(Managers.ClickManager.Instance.CheckDelay);
            }
        }); 
    }

    public void GoToReadyPoint()
    {
        Transform target = null;

        FindReadyPoint();   

        if (target == null) return;

        _state = BlockState.Air;

        _willBeReady = true;

        if (Managers.GameManager.Instance.GameState == GameState.Furnace)
        {
            _snapJumpDuration = 0;
        }

        StartCoroutine(Delay());

        IEnumerator Delay()
        {
            yield return new WaitForSeconds(_snapJumpDuration);
            
            transform.DOJump(target.position, jumpPower: 1, 1, _readyJumpDuration).SetEase(_jumpCurve).OnStart(() => 
            {
                transform.DOScale(_finalScalePercent, _readyJumpDuration);
            }) 
                .OnComplete(() =>
            {
                PoolManager.Instance.PutPoolObject(gameObject, _blockType.ToString());
                gameObject.SetActive(false);
                _snapJumpDuration = 1;
            });

            if (Managers.GameManager.Instance.GameState == GameState.Furnace) yield break;

            _staticBlock.DeactivateBlock();

            ThePointISnapped.MakeUnoccupied();
            ThePointISnapped = null;
        }

        void FindReadyPoint()
        {
            foreach (RequirementUI requirement in RequirementPanel.Instance.RequirementUIs)
            {
                if (requirement.BlockType == _blockType)
                {
                    target = requirement.ReadyPoint.transform;
                    break;
                }
            }
        }
    }

    public void GoToQueuePoint()
    {
        Transform target = null;

        FindQueuePoint();

        if (target == null)
        {
            PoolManager.Instance.PutPoolObject(gameObject, _blockType.ToString());
            gameObject.SetActive(false);
            return;
        }

        _state = BlockState.Air;

        transform.DOScale(0.3f, 0);

        transform.DOJump(target.position, jumpPower: 2, 1, _readyJumpDuration).SetEase(_jumpCurve).OnStart(() => 
        {
            transform.DOScale(1, _readyJumpDuration);
        })
        .OnComplete(() =>
        {
            Furnace.EnqueueBlock(this);
            Furnace.CookFirstObject();
        });

        void FindQueuePoint()
        {
            Furnace availableFurnace = FindAvailableFurnace().GetComponent<Furnace>();

            int occupiedCount = 0;

            foreach (QueuePoint queuePoint in availableFurnace.QueuePoints)
            {
                if (queuePoint.Occupied)
                {
                    occupiedCount++;

                    if (occupiedCount == availableFurnace.QueuePoints.Count)
                    {
                        break;
                    }

                    continue;
                }
                else
                {
                    Furnace = availableFurnace;
                    target = queuePoint.transform;
                    QueuePoint = queuePoint;
                    queuePoint.MakeOccupied(true);
                    break;
                }
            }
        }
    }

    public void GoToFurnace(Furnace furnace)
    {
        _state = BlockState.Air;

        furnace.MakeOccuppied(true);

        transform.DOJump(furnace.transform.position, jumpPower: 2, 1, _readyJumpDuration).SetEase(_jumpCurve).OnComplete(() =>
        {
            furnace.Cook(_blockType);

            PoolManager.Instance.PutPoolObject(gameObject, _blockType.ToString());
            QueuePoint.MakeOccupied(false);
            gameObject.SetActive(false);
        });
    }

    Transform FindAvailableFurnace()
    {
        Transform target = null;
        Furnace availableFurnace = FurnaceManager.Instance.Furnaces[0];
        int index = FurnaceManager.Instance.GetFurnaceIndex(availableFurnace);

        int occupiedCount = 0;

        foreach (QueuePoint queuePoint in availableFurnace.QueuePoints)
        {
            if (queuePoint.Occupied)
            {
                occupiedCount++;

                if (occupiedCount == availableFurnace.QueuePoints.Count)
                {
                    target = FurnaceManager.Instance.GetNextFurnace(index).transform;
                    return target;
                }

                continue;
            }
            else
            {
                target = availableFurnace.transform;
                break;
            }
        }

        return target;
    }

    public void GoToFurnaceReadyPoint(Furnace furnace)
    {
        Transform target = null;

        FindReadyPoint();

        if (target == null) return;

        _state = BlockState.Air;

        transform.DOScale(0.3f, 0);

        transform.DOJump(target.position, jumpPower: 3, 1, _readyJumpDuration).SetEase(_jumpCurve).OnStart(() =>
        {
            transform.DOScale(1, _readyJumpDuration);
        });

        void FindReadyPoint()
        {
            foreach (FurnaceReadyPoint readyPoint in furnace.ReadyPoints)
            {
                if (readyPoint.Occupied)
                {
                    continue;
                }
                else
                {
                    ReadyPoint = readyPoint;
                    target = readyPoint.transform;
                    readyPoint.MakeOccupied(true);
                    break;
                }
            }
        }
    }

    public void CheckFailed()
    {
        StartCoroutine(Delay());

        IEnumerator Delay()
        {
            int willNotReadyCount = 0;

            foreach (Block block in Managers.SnapPointManager.Instance.SnappedBlocks)
            {
                if (!block._willBeReady)
                {
                    willNotReadyCount++;

                    if (willNotReadyCount == Managers.SnapPointManager.Instance.SnapPoints.Count)
                    {
                        yield return new WaitForSeconds(1f);
                        Managers.UILoader.Instance.ActiveFailedPanel();
                    }

                    continue;
                }
            }
            
        }
    }
}

