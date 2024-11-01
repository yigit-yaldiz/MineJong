using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class ClickManager : MonoBehaviour
    {
        public static Action<float> OnJump;
        public static Action OnBuild;
        public static Action OnFurnace;

        public static ClickManager Instance { get; private set; }
        public float CheckDelay { get => _checkDelay; set => _checkDelay = value; }

        [Header("Check above block delay")]
        [SerializeField] float _checkDelay = 0.1f;

        [Header("Holding threshold for building")]
        [SerializeField] float _checkThreshold = 0.1f;
        
        float _clickTimer = 0;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            UILoader.OnLevelFinish += DeactivateClicks;
        }

        private void OnDisable()
        {
            UILoader.OnLevelFinish -= DeactivateClicks;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (GameManager.Instance.GameState != GameState.Collect) return;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.transform.TryGetComponent(out StaticBlock staticBlock) && staticBlock.Available)
                    {
                        staticBlock.SpawnBlock();
                    }
                    else if (hit.transform.TryGetComponent(out Block block))
                    {
                        block.JumpToAvailablePoint();
                    }
                }
            }
            else if (Input.GetMouseButton(0))
            {
                if (GameManager.Instance.GameState != GameState.Build) return;

                _clickTimer += Time.deltaTime;

                if (_clickTimer > _checkThreshold)
                {
                    OnBuild?.Invoke();
                    _clickTimer = 0;
                }
            }
        }

        void DeactivateClicks()
        {
            enabled = false;
        }
    }
}
