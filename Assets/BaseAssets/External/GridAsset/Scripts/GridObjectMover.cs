using System;
using UnityEngine;

namespace GridAsset
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(GridObject))]
    [RequireComponent(typeof(GridAgent))]
    public class GridObjectMover : MonoBehaviour
    {
        public static bool DontAllowClicks;

        public static Action<GridObjectMover> OnDown;
        public static Action<GridObjectMover> OnDrag;
        public static Action<GridObjectMover> OnUp;

        private Camera _cam;
        private Transform _tr;
        private GridObject _gridObject;
        private GridAgent _griadAgent;

        private float _cameraZDistance;

        private Vector3 _screenPosition;

        private GridCell _startCell;

        private bool _down;

        private GridCell[] _path;

        private GridCoord _targetCoords;

        private Vector3 _offset;

        private void Start()
        {
            _gridObject = GetComponent<GridObject>();
            _griadAgent = GetComponent<GridAgent>();
            _cam = Camera.main;
            _tr = transform;
        }

        private void OnMouseDown()
        {
            if (!enabled)
                return;

            if (DontAllowClicks)
                return;

            _startCell = GridMaker.Instance.CellAtWorld(_tr.position);
            _cameraZDistance = _cam.WorldToScreenPoint(transform.position).z;

            _screenPosition = Input.mousePosition;
            _screenPosition.z = _cameraZDistance;

            Vector3 clickPosition = _cam.ScreenToWorldPoint(_screenPosition);
            _offset = clickPosition - transform.position;

            _down = true;

            OnDown?.Invoke(this);
        }

        private void OnMouseDrag()
        {
            if (!_down)
                return;

            if (!enabled)
            {
                if (_startCell)
                    _tr.position = _startCell.Position;
                return;
            }

            if (DontAllowClicks)
                return;

            _screenPosition = Input.mousePosition;
            _screenPosition.z = _cameraZDistance;

            Vector3 clickPosition = _cam.ScreenToWorldPoint(_screenPosition);
            GridCoord clickCoords = GridMaker.Instance.WorldToCoord(clickPosition - _offset);



            if (_targetCoords != clickCoords)
            {
                _targetCoords = clickCoords;

                GridCell clickCell = GridMaker.Instance.CellAtWorld(transform.position);
                GridCell endCell = GridMaker.Instance.CellAtCoords(_targetCoords);

                if (_griadAgent.AllowTraverse)
                {
                    if (GridPath.GetPathTraverse(clickCell, endCell, out _path, _gridObject))
                    {
                        _griadAgent.FollowPath(_path);
                    }
                }
                else
                {
                    if (GridPath.GetPathNonTraverse(clickCell, endCell, out _path, _gridObject))
                    {
                        _griadAgent.FollowPath(_path);
                    }
                }

                OnDrag?.Invoke(this);
            }
        }

        private void OnMouseUp()
        {
            _down = false;

            if (!enabled)
                return;

            if (DontAllowClicks)
                return;

            GridCell cell = GridMaker.Instance.CellAtWorld(_tr.position);
            _tr.position = cell.Position;

            _gridObject.OnPositionChange?.Invoke();

            OnUp?.Invoke(this);
        }

        public void Enable()
        {
            enabled = true;
        }
        public void Disable()
        {
            enabled = false;
            if (_down)
                OnMouseUp();
        }
    }
}