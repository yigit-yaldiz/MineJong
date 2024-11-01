using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class ReadyPointManager : MonoBehaviour
    {
        public static ReadyPointManager Instance { get; private set; }
        public List<ReadyPoint> ReadyPoints => _readyPoints;

        [SerializeField] List<ReadyPoint> _readyPoints;

        private void Awake()
        {
            Instance = this;
        }

        public void AddReadyPoints(ReadyPoint point)
        {
            _readyPoints.Add(point);
            _readyPoints = _readyPoints.OrderBy(t => t.transform.GetSiblingIndex()).ToList();
        }
    }
}
