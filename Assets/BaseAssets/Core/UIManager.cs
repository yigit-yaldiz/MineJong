using System.Collections.Generic;
using UnityEngine;

namespace BaseAssets
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        private List<PanelTypeHolder> panels = new List<PanelTypeHolder>();

        private void Awake()
        {
            Instance = this;

            panels = new List<PanelTypeHolder>(GetComponentsInChildren<PanelTypeHolder>(true));
        }

        public void ChangePanel(PanelType panelType)
        {
            for (int i = 0; i < panels.Count; i++)
            {
                panels[i].gameObject.SetActive((panelType & panels[i].PanelType) == panelType);
            }
        }
    }
}
