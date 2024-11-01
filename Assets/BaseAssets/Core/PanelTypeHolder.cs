using System;
using UnityEngine;

namespace BaseAssets
{
    [Flags]
    public enum PanelType
    {
        None = 0,
        Menu = 1,
        Game = 2,
        Win = 4,
        Lose = 8
    }

    public class PanelTypeHolder : MonoBehaviour
    {
        public PanelType PanelType;
    }
}
