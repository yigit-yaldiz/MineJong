using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections.ObjectModel;
using System;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        RequirementUI[] _requirements;
        RequirementButton[] _requirementButtons;

        [SerializeField] TMP_Text _coinText;

        Coroutine _printCoroutine;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (GameManager.Instance.GameState == GameState.Loading) return;
            
            _requirements = RequirementPanel.Instance.RequirementUIs;
            _requirementButtons = RequirementPanel.Instance.RequirementButtons;

            PrintBlockCounts(0.5f);
        }

        public void NextButton()
        {
            LevelLoader.Instance.LoadBuildScene();
        }

        public void RestartButton()
        {
            ReadyBlockManager.Instance.PullTempReadyBlocks();
            LevelLoader.Instance.RestartScene();
            Saving.SaveWrapper.Instance.DeleteDisposable();
        }

        public void FailedButton()
        {
            RestartButton();
        }

        public void ReturnToCollectScene()
        {
            Saving.SaveWrapper.Instance.DeleteDisposable();
            LevelLoader.Instance.ReturnToLastCollectScene();
        }

        public void PrintBlockCounts(float delay = 0)
        {
            if (GameManager.Instance.GameState == GameState.Loading) return;

            if (_printCoroutine != null)
            {
                StopCoroutine(_printCoroutine);
            }

            _printCoroutine = StartCoroutine(PrintReadyBlockCounts());

            IEnumerator PrintReadyBlockCounts()
            {
                yield return new WaitForSeconds(delay);

                if (GameManager.Instance.GameState != GameState.Furnace)
                {
                    foreach (var ui in _requirements)
                    {
                        foreach (KeyValuePair<BlockType, int> item in ReadyBlockManager.Instance.ReadyBlocks)
                        {
                            if (ui.BlockType == item.Key)
                            {
                                ui.ChangeCountText(item.Value);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var ui in _requirements)
                    {
                        foreach (var item in ReadyBlockManager.Instance.ReadyBlocks)
                        {
                            if (ui.BlockType == item.Key)
                            {
                                ui.ChangeCountText(item.Value);
                            }
                        }
                    }

                    foreach (var button in _requirementButtons)
                    {
                        foreach (KeyValuePair<BlockType, int> item in ReadyBlockManager.Instance.ReadyBlocks)
                        {
                            if (button.BlockType == item.Key)
                            {
                                button.ChangeCountText(item.Value);
                            }
                        }
                    }
                }
                
            }
        }

        public void PrintCoin()
        {
            _coinText.text = CoinManager.Instance.Coin.ToString();
        }

        public void LoadFurnaceScene()
        {
            LevelLoader.Instance.LoadFurnaceScene();
        }
    }
}
