using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FurnaceReadyBlocks : MonoBehaviour
{
    Furnace _myFurnace;
    BoxCollider _boxCollider;

    private void Awake()
    {
        _myFurnace = GetComponentInParent<Furnace>();
        _boxCollider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider == _boxCollider)
                {
                    _myFurnace.SendReadyBlocks();
                }
            }
        }
    }
}
