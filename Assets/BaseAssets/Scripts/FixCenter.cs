using UnityEngine;

public class FixCenter : MonoBehaviour
{
    [BaseAssets.Button]
    private void Fix()
    {
        Vector3 offset = Vector3.zero;
        transform.localPosition = Vector3.zero;
        if (transform.parent)
            offset = transform.parent.localPosition;
        if (TryGetComponent(out Renderer rndr))
        {
            transform.localPosition = Vector3.zero - rndr.bounds.center + offset;
        }
        else
        {
            Debug.Log("Need renderer");
        }
    }

    public Vector3 centerPos;

    [BaseAssets.Button]
    private void CenterTop()
    {
        Vector3 offset = Vector3.zero;
        transform.localPosition = Vector3.zero;
        if (transform.parent)
            offset = transform.parent.localPosition;
        if (TryGetComponent(out Renderer rndr))
        {
            transform.localPosition = centerPos - rndr.bounds.center + offset - (Vector3.up * rndr.bounds.size.y / 2f);
        }
        else
        {
            Debug.Log("Need renderer");
        }
    }
    [BaseAssets.Button]
    private void CenterBottom()
    {
        Vector3 offset = Vector3.zero;
        transform.localPosition = Vector3.zero;
        if (transform.parent)
            offset = transform.parent.localPosition;
        if (TryGetComponent(out Renderer rndr))
        {
            transform.localPosition = centerPos - rndr.bounds.center + offset + (Vector3.up * rndr.bounds.size.y / 2f);
        }
        else
        {
            Debug.Log("Need renderer");
        }
    }
}
