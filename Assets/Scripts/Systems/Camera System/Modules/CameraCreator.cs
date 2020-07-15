using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCreator : ICameraCreator
{
    private readonly float initialPositionX = 0;
    private readonly float initialPositionY = 0;
    private readonly float cameraHeight = -1;
    private readonly float initialZoom = 10;

    public Camera CreateMain()
    {
        GameObject mainCameraCarrier = new GameObject("Main Camera");
        mainCameraCarrier.tag = "MainCamera";
        mainCameraCarrier.transform.position = new Vector3(
            initialPositionX,
            initialPositionY,
            cameraHeight);
        Camera mainCamera = mainCameraCarrier.AddComponent<Camera>();
        mainCamera.orthographic = true;
        mainCamera.orthographicSize = initialZoom;
        return mainCamera;
    }
}
