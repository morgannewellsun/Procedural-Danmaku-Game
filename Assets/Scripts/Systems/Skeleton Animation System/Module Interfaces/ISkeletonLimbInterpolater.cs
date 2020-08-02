using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkeletonLimbInterpolater
{
    void Add(GameObject baseObject, GameObject limbObject);
    void InterpolateLimbRelativePositionCartesian(
        GameObject baseObject, GameObject limbObject, AnimationCurve xCurve, AnimationCurve yCurve, bool loop);
    void InterpolateLimbRelativePositionRadial(
        GameObject baseObject, GameObject limbObject, AnimationCurve rCurve, AnimationCurve thetaCurve, bool loop);
    void InterpolateLimbAbsoluteRotation(
        GameObject baseObject, GameObject limbObject, AnimationCurve absoluteRotationCurve, bool loop);
    void StopRelativePositionInterpolation(GameObject baseObject, GameObject limbObject);
    void StopAbsoluteRotationInterpolation(GameObject baseObject, GameObject limbObject);
    void SetLimbRelativePosition(GameObject baseObject, GameObject limbObject, Vector2 relativePosition);
    void SetLimbAbsoluteRotation(GameObject baseObject, GameObject limbObject, float absoluteRotation);
    void UpdateManaged();
}
