using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkeletonAnimationSystem
{
    GameObject AttachLimb(GameObject baseObject, string limbLabel);
    GameObject GetLimb(GameObject baseObject, string limbLabel);
    void InterpolateLimbRelativePositionCartesian(
        GameObject baseObject, string limbLabel, AnimationCurve xCurve, AnimationCurve yCurve, bool loop);
    void InterpolateLimbRelativePositionRadial(
        GameObject baseObject, string limbLabel, AnimationCurve rCurve, AnimationCurve thetaCurve, bool loop);
    void InterpolateLimbAbsoluteRotation(
        GameObject baseObject, string limbLabel, AnimationCurve absoluteRotationCurve, bool loop);
    void StopRelativePositionInterpolation(GameObject baseObject, string limbLabel);
    void StopAbsoluteRotationInterpolation(GameObject baseObject, string limbLabel);
    void SetLimbRelativePosition(GameObject baseObject, string limbLabel, Vector2 relativePosition);
    void SetLimbAbsoluteRotation(GameObject baseObject, string limbLabel, float absoluteRotation);
}