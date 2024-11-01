using UnityEngine;

namespace BaseAssets
{
    [DefaultExecutionOrder(-10)]
    public partial class BA : MonoBehaviour
    {
        public static BA Instance { get; private set; }


        private void Awake()
        {
            Instance = this;

            SaveSystem.Load();

            StateMachine.ChangeState(StateMachine.States.Menu);
        }
    }
}
