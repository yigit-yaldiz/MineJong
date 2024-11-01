using UnityEngine;

namespace GridAsset
{
    [ExecuteAlways]
    public class CenterGrid : MonoBehaviour
    {
        private GridMaker _gridMaker;

        private void OnEnable()
        {
            GridMaker.PreGenerate += Check;
        }
        private void OnDisable()
        {
            GridMaker.PreGenerate -= Check;
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (!Application.isPlaying)
                Check();
        }
#endif

        public void Check()
        {
            if (!_gridMaker)
                if (!TryGetComponent(out _gridMaker))
                    return;

            transform.position = new Vector3(-_gridMaker.GridWidth / 2f, 0, -_gridMaker.GridLength / 2f);
        }
    }
}