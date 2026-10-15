using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFitter : MonoBehaviour
{
    [SerializeField] private Vector2 mapSize = new Vector2(8f, 12f);
    [SerializeField] private Vector2 mapCenter = Vector2.zero;
    [SerializeField] private float padding = 0.5f;

    private Camera cam;
    private float lastAspect;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (Mathf.Approximately(cam.aspect, lastAspect))
            return;

        lastAspect = cam.aspect;
        Fit();
    }

    private void Fit()
    {
        float halfHeight = mapSize.y / 2f + padding;
        float halfWidth = mapSize.x / 2f + padding;

        cam.orthographicSize = Mathf.Max(halfHeight, halfWidth / cam.aspect);
        transform.position = new Vector3(mapCenter.x, mapCenter.y, transform.position.z);
    }
}