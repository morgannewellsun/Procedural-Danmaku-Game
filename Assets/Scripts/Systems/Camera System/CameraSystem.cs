using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour, ICameraSystem
{
    private ICameraCreator cameraCreator;
    private ICameraMover cameraMover;

    private Camera mainCamera;

    void Awake()
    {
        cameraCreator = new CameraCreator();
        mainCamera = cameraCreator.CreateMain();
        cameraMover = new CameraMover();
    }

    void Start()
    {

    }

    void Update()
    {
        cameraMover.UpdateManaged();
    }

    public Camera GetMainCamera()
    {
        return mainCamera;
    }

    public void MoveMainCameraTo(float x, float y, float time = 0, float endpointSmoothing = 0)
    {
        if (time == 0)
        {
            cameraMover.SetPosition(x, y);
        }
        else
        {
            cameraMover.InitiateSmoothMovement(x, y, time, endpointSmoothing);
        }
    }

    public void ZoomMainCameraTo(float zoom, float time = 0, float endpointSmoothing = 0)
    {
        if (time == 0)
        {
            cameraMover.SetZoom(zoom);
        }
        else
        {
            cameraMover.InitiateSmoothZoom(zoom, time, endpointSmoothing);
        }
    }
}
