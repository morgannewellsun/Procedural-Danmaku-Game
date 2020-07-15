using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICameraSystem
{
    Camera GetMainCamera();
    void MoveMainCameraTo(float x, float y,  float time = 0, float endpointSmoothing = 0);
    void ZoomMainCameraTo(float zoom, float time = 0, float endpointSmoothing = 0);
}
