using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class RequirementManager : MonoBehaviour
    {
        public static RequirementManager Instance { get; private set; }
        public Building CurrentBuilding { get => _currentBuilding; set => _currentBuilding = value; }
        public RequirementList CurrentList { get => _currentList; set => _currentList = value; }

        [SerializeField] Building _currentBuilding;
        [SerializeField] RequirementList _currentList;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            FindCurrentBuilding();
        }

        void FindCurrentBuilding()
        {
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent(out Building building))
                {
                    if (building.IsBuild)
                    {
                        foreach (var list in building.Lists)
                        {
                            list.ActivateSavedBlocks();
                        }
                        Debug.Log(building + " is built. Moving to the next one");
                        continue;
                    }
                    else
                    {
                        building.gameObject.SetActive(true);
                        _currentBuilding = building;
                        SetCurrentList();
                        break;
                    }
                }
            }
        }

        public void SetCurrentList()
        {
            foreach (Transform item in _currentBuilding.transform)
            {
                if (item.TryGetComponent(out RequirementList list))
                {
                    if (list.IsBuild)
                    {
                        if (list.transform.GetSiblingIndex() != list.transform.parent.childCount - 1)
                        {
                            list.enabled = false;
                        }
                        continue;
                    }
                    else
                    {
                        list.gameObject.SetActive(true);
                        _currentList = list;
                        break;
                    }
                }
            }
        }

        public bool CheckNextListCompletion(RequirementList list)
        {
            int index = list.transform.GetSiblingIndex();

            bool isThereList = false;
            
            if (index >= 0 && index != list.transform.parent.childCount - 1)
            {
                RequirementList nextList = _currentBuilding.Lists[index + 1];

                if (nextList.IsBuild)
                {
                    CheckNextListCompletion(_currentBuilding.Lists[index + 1]);
                }
                else
                {
                    isThereList = true;
                    Debug.Log("The next list is not built yet.");
                }
            }

            return isThereList;
        }
    }
}
