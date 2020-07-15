using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICameraMover
{
    void InitiateSmoothMovement(float x, float y, float time, float endpointSmoothing);
    void InitiateSmoothZoom(float zoom, float time, float endpointSmoothing);
    void SetPosition(float x, float y);
    void SetZoom(float zoom);
    void UpdateManaged();
}
