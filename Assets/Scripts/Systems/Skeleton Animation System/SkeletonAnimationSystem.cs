using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonAnimationSystem : MonoBehaviour, ISkeletonAnimationSystem
{
    private ISkeletonLimbManager skeletonLimbManager;
    private ISkeletonLimbInterpolater skeletonLimbInterpolater;

    void Awake()
    {
        skeletonLimbManager = new SkeletonLimbManager();
        skeletonLimbInterpolater = new SkeletonLimbInterpolater();
    }

    void Start()
    {

    }

    void Update()
    {
        skeletonLimbManager.UpdateManaged();
        skeletonLimbInterpolater.UpdateManaged();
    }

    public GameObject AttachLimb(GameObject baseObject, string limbLabel)
    {
        return skeletonLimbManager.AttachLimb(baseObject, limbLabel);
    }

    public GameObject GetLimb(GameObject baseObject, string limbLabel)
    {
        return skeletonLimbManager.GetLimb(baseObject, limbLabel);
    }

    public void InterpolateLimbAbsoluteRotation(
        GameObject baseObject, string limbLabel, AnimationCurve absoluteRotationCurve, bool loop)
    {
        GameObject limbObject = skeletonLimbManager.GetLimb(baseObject, limbLabel);
        skeletonLimbInterpolater.InterpolateLimbAbsoluteRotation(baseObject, limbObject, absoluteRotationCurve, loop);
    }

    public void InterpolateLimbRelativePositionCartesian(
        GameObject baseObject, string limbLabel, AnimationCurve xCurve, AnimationCurve yCurve, bool loop)
    {
        GameObject limbObject = skeletonLimbManager.GetLimb(baseObject, limbLabel);
        skeletonLimbInterpolater.InterpolateLimbRelativePositionCartesian(baseObject, limbObject, xCurve, yCurve, loop);
    }

    public void InterpolateLimbRelativePositionRadial(
        GameObject baseObject, string limbLabel, AnimationCurve rCurve, AnimationCurve thetaCurve, bool loop)
    {
        GameObject limbObject = skeletonLimbManager.GetLimb(baseObject, limbLabel);
        skeletonLimbInterpolater.InterpolateLimbRelativePositionRadial(baseObject, limbObject, rCurve, thetaCurve, loop);
    }

    public void SetLimbAbsoluteRotation(GameObject baseObject, string limbLabel, float absoluteRotation)
    {
        GameObject limbObject = skeletonLimbManager.GetLimb(baseObject, limbLabel);
        skeletonLimbInterpolater.SetLimbAbsoluteRotation(baseObject, limbObject, absoluteRotation);
    }

    public void SetLimbRelativePosition(GameObject baseObject, string limbLabel, Vector2 relativePosition)
    {
        GameObject limbObject = skeletonLimbManager.GetLimb(baseObject, limbLabel);
        skeletonLimbInterpolater.SetLimbRelativePosition(baseObject, limbObject, relativePosition);
    }

    public void StopAbsoluteRotationInterpolation(GameObject baseObject, string limbLabel)
    {
        GameObject limbObject = skeletonLimbManager.GetLimb(baseObject, limbLabel);
        skeletonLimbInterpolater.StopAbsoluteRotationInterpolation(baseObject, limbObject);
    }

    public void StopRelativePositionInterpolation(GameObject baseObject, string limbLabel)
    {
        GameObject limbObject = skeletonLimbManager.GetLimb(baseObject, limbLabel);
        skeletonLimbInterpolater.StopRelativePositionInterpolation(baseObject, limbObject);
    }
}
