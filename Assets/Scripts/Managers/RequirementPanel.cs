using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    [DefaultExecutionOrder(1)]
    public class RequirementPanel : MonoBehaviour
    {
        public static RequirementPanel Instance { get; private set; }
        public RequirementUI[] RequirementUIs => _requirements;
        public RequirementButton[] RequirementButtons => _requirementButtons;

        [SerializeField] BlockSpritesSO _blockSprites;
        RequirementUI[] _requirements;
        RequirementButton[] _requirementButtons;

        private void Awake()
        {
            Instance = this;
            _requirements = GetComponentsInChildren<RequirementUI>();
            _requirementButtons = GetComponentsInChildren<RequirementButton>();
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.4f);
            DefineUIBlockTypes(GameManager.Instance.GameState);
            ChangeSpritesByTypes();
        }

        void DefineUIBlockTypes(GameState gameState)
        {
            if (gameState == GameState.Collect)
            {
                List<BlockType> temp = HillManager.Instance.ActiveHill.BlockTypes;

                if (_requirements.Length > temp.Count)
                {
                    int residual = _requirements.Length - temp.Count;

                    for (int i = residual; i > 0; i--)
                    {
                        _requirements[_requirements.Length - i].transform.parent.gameObject.SetActive(false);
                    }
                }

                foreach (RequirementUI child in _requirements)
                {
                    foreach (var type in temp)
                    {
                        child.BlockType = type;
                        child.ReadyPoint.BlockType = type;
                        temp.Remove(type);
                        break;
                    }
                }
            }
            else if (gameState == GameState.Build)
            {
                List<BlockType> temp = new();

                foreach (var item in ReadyBlockManager.Instance.ReadyBlocks)
                {
                    temp.Add(item.Key);
                }

                foreach (var item in RequirementManager.Instance.CurrentBuilding.RequiredTotalBlockCounts)
                {
                    if (item.Key == BlockType.Stairs)
                    {
                        temp.Add(item.Key);
                    }
                }

                if (_requirements.Length > temp.Count)
                {
                    int residual = _requirements.Length - temp.Count;

                    for (int i = residual; i > 0; i--)
                    {
                        _requirements[_requirements.Length - i].transform.parent.gameObject.SetActive(false);
                    }
                }

                foreach (RequirementUI requirement in _requirements)
                {
                    foreach (var type in temp)
                    {
                        requirement.BlockType = type;

                        if (type == BlockType.Stairs)
                        {
                            requirement.transform.parent.gameObject.SetActive(false);
                        }

                        temp.Remove(type);

                        break;
                    }
                }
            }
            else if (gameState == GameState.Furnace)
            {
                List<BlockType> tempUI = new();
                List<BlockType> tempButton = new();

                foreach (var item in FurnaceManager.Instance.ReadyCookableTypes)
                {
                    tempButton.Add(item);

                    foreach (var type in FurnaceManager.Instance.CookedBlockTypes)
                    {
                        if (item.ToString() + "Block" == type.ToString())
                        {
                            tempUI.Add(type);
                        }
                    }
                }

                if (_requirements.Length > tempUI.Count)
                {
                    int residual = _requirements.Length - tempUI.Count;

                    for (int i = residual; i > 0; i--)
                    {
                        _requirements[_requirements.Length - i].transform.parent.gameObject.SetActive(false);
                    }
                }

                if (_requirementButtons.Length > tempButton.Count)
                {
                    int residual = _requirementButtons.Length - tempButton.Count;

                    for (int i = residual; i > 0; i--)
                    {
                        _requirementButtons[_requirementButtons.Length - i].transform.parent.gameObject.SetActive(false);
                    }
                }

                foreach (RequirementUI requirement in _requirements)
                {
                    foreach (var type in tempUI)
                    {
                        requirement.BlockType = type;
                        tempUI.Remove(type);
                        break;
                    }
                }

                foreach (RequirementButton item in _requirementButtons)
                {
                    foreach (var type in tempButton)
                    {
                        item.BlockType = type;
                        tempButton.Remove(type);
                        break;
                    }
                }
            }
        }

        void ChangeSpritesByTypes()
        {
            foreach (RequirementUI child in _requirements)
            {
                Sprite sprite = PullTheImage(child.BlockType.ToString());
                child.ChangeImage(sprite);
            }

            foreach (RequirementButton child in _requirementButtons)
            {
                Sprite sprite = PullTheImage(child.BlockType.ToString());
                child.ChangeImage(sprite);
            }
        }

        Sprite PullTheImage(string name)
        {
            Sprite target = null;

            foreach (Sprite item in _blockSprites.BlockSprites)
            {
                if (name == item.name)
                {
                    target = item;
                }
            }

            return target;
        }
    }
}
