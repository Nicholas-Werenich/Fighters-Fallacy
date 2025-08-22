using UnityEngine;

public class Tracking : MonoBehaviour
{
    //Script for tracking child to camera while maintaining y offset from parent
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }
    private void Update()
    {
        TrackingCamera();
    }

    private void TrackingCamera()
    {
        transform.position = new Vector2(mainCamera.transform.position.x, mainCamera.transform.position.y + transform.parent.position.y);
    }
}
