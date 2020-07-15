using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class CameraMover : ICameraMover
{
    private bool isMoving = false;
    private float movementDuration;
    private float movementStartingTime;
    private float movementEndpointSmoothing;
    private float movementSigmoidOffset;
    private Vector3 movementStartingLocation;
    private Vector3 movementTargetLocation;

    private bool isZooming = false;
    private float zoomDuration;
    private float zoomStartingTime;
    private float zoomEndpointSmoothing;
    private float zoomSigmoidOffset;
    private float zoomStartingOrthSize;
    private float zoomTargetOrthSize;

    public void InitiateSmoothMovement(float x, float y, float time, float endpointSmoothing)
    {
        SetMovementParameters(x, y, time, endpointSmoothing);
    }

    public void InitiateSmoothZoom(float zoom, float time, float endpointSmoothing)
    {
        SetZoomParameters(zoom, time, endpointSmoothing);
    }

    public void SetPosition(float x, float y)
    {
        Camera mainCamera = GameDirector.cameraSystem.GetMainCamera();
        isMoving = false;
        mainCamera.transform.position = new Vector3(x, y, mainCamera.transform.position.z);
    }

    public void SetZoom(float zoom)
    {
        isZooming = false;
        GameDirector.cameraSystem.GetMainCamera().orthographicSize = zoom;
    }

    public void UpdateManaged()
    {
        UpdateMovement();
        UpdateZoom();
    }

    private void SetMovementParameters(float x, float y, float time, float endpointSmoothing)
    {
        Camera mainCamera = GameDirector.cameraSystem.GetMainCamera();
        isMoving = true;
        movementDuration = time;
        movementStartingTime = Time.time;
        movementEndpointSmoothing = endpointSmoothing;
        movementSigmoidOffset = (float)(1 / (1 + Mathf.Exp(-1 * endpointSmoothing)));
        movementStartingLocation = new Vector3(
            mainCamera.transform.position.x,
            mainCamera.transform.position.y,
            mainCamera.transform.position.z);
        movementTargetLocation = new Vector3(
            x,
            y,
            mainCamera.transform.position.z);
    }

    private void UpdateMovement()
    {
        if (!isMoving) { return; }
        Camera mainCamera = GameDirector.cameraSystem.GetMainCamera();
        float x = (2 * movementEndpointSmoothing * (Time.time - movementStartingTime) / movementDuration) - movementEndpointSmoothing;
        float interpolationFactor = ((1 / (1 + Mathf.Exp(x))) - movementSigmoidOffset) / (1 - 2 * movementSigmoidOffset);
        if (interpolationFactor >= 1)
        {
            isMoving = false;
            mainCamera.transform.position = movementTargetLocation;
            return;
        }
        else
        {
            Vector3 tweenPosition = Vector3.Lerp(movementStartingLocation, movementTargetLocation, interpolationFactor);
            mainCamera.transform.position = new Vector3(tweenPosition.x, tweenPosition.y, tweenPosition.z);
        }
    }

    private void SetZoomParameters(float zoom, float time, float endpointSmoothing)
    {
        isZooming = true;
        zoomDuration = time;
        zoomStartingTime = Time.time;
        zoomEndpointSmoothing = endpointSmoothing;
        zoomSigmoidOffset = (1 / (1 + Mathf.Exp(-1 * zoomEndpointSmoothing)));
        zoomStartingOrthSize = GameDirector.cameraSystem.GetMainCamera().orthographicSize;
        zoomTargetOrthSize = zoom;
    }

    private void UpdateZoom()
    {
        if (!isZooming) { return; }
        Camera mainCamera = GameDirector.cameraSystem.GetMainCamera();
        float x = (2 * zoomEndpointSmoothing * (Time.time - zoomStartingTime) / zoomDuration) - zoomEndpointSmoothing;
        float interpolationFactor = ((float)(1 / (1 + Mathf.Exp(x))) - zoomSigmoidOffset) / (1 - 2 * zoomSigmoidOffset);
        if (interpolationFactor >= 1)
        {
            isZooming = false;
            mainCamera.orthographicSize = zoomTargetOrthSize;
            return;
        }
        else
        {
            mainCamera.orthographicSize = Mathf.Lerp(zoomStartingOrthSize, zoomTargetOrthSize, interpolationFactor);
        }
    }
}
