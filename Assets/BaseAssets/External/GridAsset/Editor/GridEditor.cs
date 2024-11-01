using UnityEditor;
using UnityEngine;

namespace GridAsset
{
    [CustomEditor(typeof(GridMaker))]
    public class GridEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GridMaker gridMaker = (GridMaker)target;
            if (GUILayout.Button("Generate"))
            {
                gridMaker.GenerateGrid();
            }
        }
    }
}