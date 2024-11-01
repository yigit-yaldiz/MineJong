using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class BuildPoint : MonoBehaviour
{
    public BlockType BlockType => _type;

    [SerializeField] BlockType _type;

    private void Awake()
    {
        ChangeMeshCondition(false);
        Debug.Log("Closed");
    }

    public void ChangeMeshCondition(bool condition)
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = condition;

        if (transform.childCount != 0)
        {
            MeshRenderer[] childMeshes = GetComponentsInChildren<MeshRenderer>();

            foreach (var item in childMeshes)
            {
                item.enabled = condition;
            }
        }
    }
}
