using UnityEngine;
using DG.Tweening;
using System;

namespace GridAsset
{
    public class GridAgent : MonoBehaviour
    {
        public Action<bool> OnMove;
        public Action OnReachDestination;
        public PathStatusType PathStatus { get; private set; }
        public PathType PathType { get => _pathType; set => _pathType = value; }
        public GridCell Destination => _destinationEnd;
        public GridCell[] Path => _path;
        public Vector3[] PathPoints => _pathPoints;
        public float Speed => _speed;
        public bool IsMoving => _isMoving;
        public bool SetLookAt => _setLookAt;
        public bool AllowTraverse => _allowTraverse;
        public bool BlockCell { get => _blockCell; set => _blockCell = value; }

        [SerializeField] private PathType _pathType;
        [SerializeField] private bool _blockCell = false;
        [SerializeField] private bool _blockWhileMoving = false;
        [SerializeField] private float _speed = 5f;
        [SerializeField] private bool _allowTraverse = false;
        [SerializeField] private bool _setLookAt = true;
        [SerializeField] private bool _debug;
        private GridCell[] _path;
        private Vector3[] _pathPoints;

        private GridCell _destinationStart;
        private GridCell _destinationEnd;

        private Tween _pathTween;

        private bool _isMoving;

        private GridCell _currentCell;

        private void Start()
        {
            BlockCheck();
        }

        private void Update()
        {
            if (transform.hasChanged)
            {
                transform.hasChanged = false;

                if (_blockWhileMoving || !_isMoving)
                    BlockCheck();
            }
        }

        public void FollowPath(GridCell[] path, TweenCallback OnComplete = null)
        {
            if (transform == null)
                return;

            _pathPoints = new Vector3[path.Length + 1];
            _pathPoints[0] = transform.position;
            for (int i = 1; i < _pathPoints.Length; i++)
            {
                _pathPoints[i] = path[i - 1].Position;
            }

            _pathTween?.Kill();

            if (_setLookAt)
            {
                _pathTween = transform.DOPath(_pathPoints, _speed, _pathType)
                    .SetSpeedBased()
                    .SetEase(Ease.Linear)
                    .SetLookAt(0.01f)
                    .OnComplete(OnPathComplete);
            }
            else
            {
                _pathTween = transform.DOPath(_pathPoints, _speed, _pathType)
                    .SetSpeedBased()
                    .SetEase(Ease.Linear)
                    .OnComplete(OnPathComplete);
            }

            _pathTween.onComplete += OnComplete;
        }
        public bool SetDestination(GridCell destinationCell, TweenCallback OnComplete = null)
        {
            if (transform == null)
                return false;

            _destinationStart = GridMaker.Instance.CellAtWorld(transform.position);
            _destinationEnd = destinationCell;

            bool havePath = false;
            if (_allowTraverse)
            {
                havePath = GridPath.GetPathTraverse(_destinationStart, _destinationEnd, out _path);
            }
            else
            {
                havePath = GridPath.GetPathNonTraverse(_destinationStart, _destinationEnd, out _path);
            }
            if (havePath)
            {
                _pathPoints = new Vector3[_path.Length + 1];
                _pathPoints[0] = transform.position;
                for (int i = 1; i < _pathPoints.Length; i++)
                {
                    _pathPoints[i] = _path[i - 1].Position;
                }

                _pathTween?.Kill();

                if (_setLookAt)
                {
                    _pathTween = transform.DOPath(_pathPoints, _speed, _pathType)
                        .SetSpeedBased()
                        .SetEase(Ease.Linear)
                        .SetLookAt(0.01f)
                        .OnWaypointChange(OnGridCell)
                        .OnComplete(OnPathComplete);
                }
                else
                {
                    _pathTween = transform.DOPath(_pathPoints, _speed, _pathType)
                        .SetSpeedBased()
                        .SetEase(Ease.Linear)
                        .OnWaypointChange(OnGridCell)
                        .OnComplete(OnPathComplete);
                }

                if (_currentCell)
                    _currentCell.Available = true;

                _pathTween.onComplete += OnComplete;

                PathStatus = PathStatusType.IsValid;
                _isMoving = true;
                OnMove?.Invoke(true);

                LogInfo();
                return true;
            }
            else
            {
                PathStatus = PathStatusType.NotValid;

                LogInfo();
                return false;
            }
        }
        public void Stop()
        {
            _isMoving = false;
            BlockCheck();

            _pathTween?.Kill();
            OnMove?.Invoke(false);
        }

        private void OnGridCell(int waypointIndex)
        {
            if (_blockWhileMoving)
                BlockCheck();
        }

        private void OnPathComplete()
        {
            _isMoving = false;
            BlockCheck();

            OnMove?.Invoke(false);
            OnReachDestination?.Invoke();
        }

        private void LogInfo()
        {
            if (_debug)
            {
                switch (PathStatus)
                {
                    case PathStatusType.IsValid:
                        Debug.Log($"Agent '{gameObject.name}' moving from {_destinationStart} to {_destinationEnd}");
                        break;
                    case PathStatusType.NotValid:
                        Debug.Log($"Agent '{gameObject.name}' has no available way from {_destinationStart} to {_destinationEnd}");
                        break;
                    default:
                        break;
                }
            }
        }

        private void BlockCheck()
        {
            if (_currentCell)
                _currentCell.Available = true;

            _currentCell = GridMaker.Instance.CellAtWorld(transform.position);
            _currentCell.Available = !_blockCell;
        }
    }
}