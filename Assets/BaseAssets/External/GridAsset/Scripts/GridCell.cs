using UnityEngine;

namespace GridAsset
{
    [DefaultExecutionOrder(-2)]
    public class GridCell : MonoBehaviour
    {
        public bool Available { get => _available; set => _available = value; }
        public int HCost { get => _hCost; set => _hCost = value; }
        public int GCost { get => _gCost; set => _gCost = value; }
        public int FCost
        {
            get
            {
                return _hCost + _gCost;
            }
        }
        public Vector3 Position
        {
            get
            {
                Vector3 pos = transform.position;
                pos.y = 0;
                return pos;
            }
        }
        public GridCell[] Neighbours { get; private set; }
        public GridCell[] NeighboursNonTraverse { get; private set; }
        public GridCoord Coords => GridMaker.Instance.CoordsOf(this);

        public GridCell Parent { get => _parent; set => _parent = value; }

        private int _hCost;
        private int _gCost;
        private GridCell _parent;

        [SerializeField] private Renderer _renderer;
        
        private bool _available = true;

        public void MaterialSet(Material material)
        {
            _renderer.sharedMaterial = material;
        }

        private void Awake()
        {
            NeighbourCheck();
        }

        public void OnGenerate()
        {
            Available = true;
            NeighbourCheck();
        }

        private void NeighbourCheck()
        {
            if (GridMaker.Instance)
            {
                Neighbours = GridMaker.Instance.GetNeighbours(this, true);
                NeighboursNonTraverse = GridMaker.Instance.GetNeighbours(this, false);
            }
        }

        public GridCell CellWithOffset(GridCoord offset)
        {
            return GridMaker.Instance.CellAtCoords(Coords + offset);
        }

        public GridCell[] GetNeighbours(bool traverse)
        {
            if (traverse)
                return Neighbours;
            else
                return NeighboursNonTraverse;
        }
    }
}