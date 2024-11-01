#if UNITY_EDITOR
using UnityEngine;

public class Shortcuts : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Q))
        {
            GameManager.Instance.Lose();
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.W))
        {
            GameManager.Instance.Reload();
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.E))
        {
            GameManager.Instance.Win();
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.C))
        {
            ValueManager.Instance.UpdateValue(Value.Type.Money, 1000);
        }
    }
}
#endif
