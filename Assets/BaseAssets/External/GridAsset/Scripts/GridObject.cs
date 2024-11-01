using System;
using System.Collections.Generic;
using UnityEngine;

namespace GridAsset
{
    [DefaultExecutionOrder(-1)]
    public class GridObject : MonoBehaviour
    {
        public static Action OnGridObjetsChange;
        public static Action<GridObject> OnPositionUpdate;
        public Action OnPositionChange;
        public GridCoord Coords => _coords;
        public bool BlockHoveringCell
        {
            get
            {
                return _blockUnderneath;
            }
            set
            {
                _blockUnderneath = value;
                UpdatePosition();
            }
        }

        public GridCell[] UsingCells => _usingCells.ToArray();

        public bool IsUsingCell(GridCell cell)
        {
            return _usingCells.Contains(cell);
        }

        public GridCoord[] AdditionalDimensions => _additionalDimensions;

        [SerializeField] private bool _blockUnderneath = true;
        [SerializeField] private GridCoord[] _additionalDimensions;
        [SerializeField] private List<GridCell> _usingCells;

        private GridCoord _coords;
        private bool _firstCheck = true;

        private void Awake()
        {
            //UpdatePosition();
        }
        private void OnEnable()
        {
            OnPositionChange += UpdatePosition;
            GridMaker.OnGenerate += GenerateUpdate;

            UpdatePosition();
        }
        private void OnDisable()
        {
            OnPositionChange -= UpdatePosition;
            GridMaker.OnGenerate -= GenerateUpdate;

            UnclaimCells();
        }

        private void GenerateUpdate()
        {
            CalculateUsingCells();
        }

        private void UpdatePosition()
        {
            if (!_firstCheck)
                UnclaimCells();

            CalculateUsingCells();

            if (!_firstCheck)
            {
                OnGridObjetsChange?.Invoke();
                OnPositionUpdate?.Invoke(this);
            }

            _firstCheck = false;
        }

        private void UnclaimCells()
        {
            GridMaker.Instance.CellAtCoords(_coords).Available = true;
            for (int i = 0; i < _usingCells.Count; i++)
                _usingCells[i].Available = true;
        }
        private void CalculateUsingCells()
        {
            _coords = GridMaker.Instance.WorldToCoord(transform.position);

            _usingCells = new List<GridCell>();
            _usingCells.Add(GridMaker.Instance.CellAtCoords(_coords));

            float yRot = Mathf.RoundToInt(transform.eulerAngles.y) * Mathf.Deg2Rad;

            int x = Mathf.RoundToInt(Mathf.Sin(yRot));
            int y = Mathf.RoundToInt(Mathf.Cos(yRot));

            for (int i = 0; i < _additionalDimensions.Length; i++)
            {
                int fixedX = _additionalDimensions[i].x;
                int fixedY = _additionalDimensions[i].y;

                if (x != 0)
                {
                    int tempX = fixedX;
                    int tempY = fixedY;

                    fixedX = tempY;
                    fixedY = tempX;
                }

                if (x != 0)
                    fixedX *= x;

                if (y != 0)
                    fixedY *= y;


                _additionalDimensions[i] = new GridCoord(fixedX, fixedY);

                GridCell offsetCell = _usingCells[0].CellWithOffset(_additionalDimensions[i]);
                if (offsetCell != null)
                    _usingCells.Add(offsetCell);
            }

            for (int i = 0; i < _usingCells.Count; i++)
                if (_usingCells[i] != null)
                    _usingCells[i].Available = !_blockUnderneath;
        }
    }
}