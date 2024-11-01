using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

namespace GridAsset
{
    [DefaultExecutionOrder(-3)]
    public class GridMaker : MonoBehaviour
    {
        public static GridMaker Instance { get; private set; }

        public static Action PreGenerate;
        public static Action OnGenerate;
        public static Action AfterGenerate;

        public int GridWidth => _gridWidth;
        public int GridLength => _gridLength;
        public int CellCount => _cells.Count;

        public GridCell LastCell => _cells[_cells.Count - 1];

        [SerializeField] private int _gridWidth = 2;
        [SerializeField] private int _gridLength = 2;
        [SerializeField] private float _gridSize = 1;
        /*[SerializeField]*/
        private bool _reverse = false;
        /*[SerializeField]*/
        private bool _centerHorizontal = false;
        /*[SerializeField]*/
        private bool _centerVertical = false;
        //[SerializeField] private bool _allowTraverse = true;

        [SerializeField] private GridCell _gridCell;

        [SerializeField] private Material[] _materials;

        private List<GridCell> _cells;

        private const string _gridHolderName = "Grid";

        private void Awake()
        {
            Instance = this;

            _cells = new List<GridCell>(GetComponentsInChildren<GridCell>());
        }
        public void GenerateGrid(int gridWidth, int gridLength)
        {
            PreGenerate?.Invoke();

            _gridWidth = gridWidth;
            _gridLength = gridLength;

            GenerateGrid();
        }
        public void GenerateGrid(int gridWidth, int gridLength, Vector3 position)
        {
            PreGenerate?.Invoke();

            transform.position = position;

            _gridWidth = gridWidth;
            _gridLength = gridLength;

            GenerateGrid();
        }
        public void GenerateGrid()
        {
            Transform gridParent = transform.Find(_gridHolderName);
            if (gridParent != null)
                DestroyImmediate(gridParent.gameObject);

            gridParent = new GameObject(_gridHolderName).transform;
            gridParent.transform.parent = transform;
            gridParent.localPosition = Vector3.zero;
            gridParent.localRotation = Quaternion.identity;

            int materialIndex = _gridWidth % 2 == 0 ? 1 : 0;
            _cells = new List<GridCell>();

            for (int y = 0; y < _gridLength; y++)
            {
                for (int x = 0; x < _gridWidth; x++)
                {
                    SpawnCell(gridParent, new GridCoord(x, y), materialIndex);
                    materialIndex++;
                }
                if (_gridWidth % 2 == 0)
                    materialIndex++;
            }

            for (int i = 0; i < _cells.Count; i++)
            {
                _cells[i].OnGenerate();
            }

            OnGenerate?.Invoke();

            StartCoroutine(Delay());

            IEnumerator Delay()
            {
                yield return null;
                AfterGenerate?.Invoke();
            }
        }
        private GridCell SpawnCell(Transform cellParent, GridCoord cellCoords, int materialIndex)
        {
            float x = cellCoords.x * _gridSize + _gridSize / 2f;
            float y = cellCoords.y * _gridSize + _gridSize / 2f;

            if (_centerHorizontal)
            {
                x -= _gridWidth / 2f * _gridSize;
            }
            if (_centerVertical)
            {
                y -= _gridLength / 2f * _gridSize;
            }

            if (_reverse)
            {
                x *= -1;
            }

#if UNITY_EDITOR
            GridCell newCell = PrefabUtility.InstantiatePrefab(_gridCell, cellParent) as GridCell;
#else
            GridCell newCell = Instantiate(_gridCell, cellParent);
#endif
            newCell.transform.localPosition = new Vector3(x, 0, y);
            newCell.transform.localScale *= _gridSize;
            newCell.MaterialSet(_materials[materialIndex % _materials.Length]);

            _cells.Add(newCell);
            GridCoord coords = CoordsOf(newCell);
            string newName = newCell.name + "(" + coords.x + "," + coords.y + ")";
            newCell.name = newName;

            return newCell;
        }

        public GridCell[] GetNeighbours(GridCell cell, bool allowTraverse)
        {
            List<GridCell> neighbours = new List<GridCell>();

            GridCoord coords = cell.Coords;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 & y == 0)
                        continue;

                    if (!allowTraverse)
                    {
                        if (x == 1 & y == 1)
                            continue;
                        if (x == 1 & y == -1)
                            continue;
                        if (x == -1 & y == 1)
                            continue;
                        if (x == -1 & y == -1)
                            continue;
                    }

                    int checkX = coords.x + x;
                    int checkY = coords.y + y;

                    if (checkX >= 1 && checkX <= _gridWidth && checkY >= 1 && checkY <= _gridLength)
                        neighbours.Add(CellAtCoords(new GridCoord(checkX, checkY)));
                }
            }
            return neighbours.ToArray();
        }

        public GridCoord CoordsOf(GridCell cell)
        {
            if (cell == null)
                return new GridCoord(0, 0);

            int index = _cells.IndexOf(cell);
            int x = index % _gridWidth + 1;
            int y = index / _gridWidth + 1;
            return new GridCoord(x, y);
        }

        public GridCell CellAtCoords(GridCoord coords)
        {
            //coords.x = Mathf.Clamp(coords.x, 1, _gridWidth);
            //coords.y = Mathf.Clamp(coords.y, 1, _gridLength);
            if (coords.x <= 0)
                return null;
            if (coords.y <= 0)
                return null;
            if (coords.x > _gridWidth)
                return null;
            if (coords.y > _gridLength)
                return null;

            int index = coords.y * _gridWidth - _gridWidth + coords.x - 1;
            if (index >= _cells.Count)
                return null;

            return _cells[index];
        }
        public GridCell CellAtWorld(Vector3 worldPosition, bool clamp = true)
        {
            GridCoord gridCoord = WorldToCoord(worldPosition, clamp);
            return CellAtCoords(gridCoord);
        }

        public GridCell GetCell(int index)
        {
            return _cells[index];
        }

        public GridCell[] GetCells()
        {
            return _cells.ToArray();
        }

        public GridCoord WorldToCoord(Vector3 worldPosition, bool clamp = true)
        {
            int x;
            int y;

            worldPosition -= transform.position;

            //if (_centerHorizontal)
            //    worldPosition.x -= _gridWidth / 2f * _gridSize;

            //if (_centerVertical)
            //    worldPosition.z -= _gridLength / 2f * _gridSize;

            //if (_reverse)
            //    worldPosition.x *= -1;

            x = Mathf.FloorToInt(worldPosition.x / _gridSize) + 1;
            y = Mathf.FloorToInt(worldPosition.z / _gridSize) + 1;

            if (clamp)
            {
                x = Mathf.Clamp(x, 1, _gridWidth);
                y = Mathf.Clamp(y, 1, _gridLength);
            }
            return new GridCoord(x, y);
        }
    }
}