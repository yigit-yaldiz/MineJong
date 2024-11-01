using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public static MainCamera Instance { get; private set; }

    public Camera Cam => cam;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        Instance = this;
    }
}
