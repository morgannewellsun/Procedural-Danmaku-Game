using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class SkeletonLimbInterpolater : ISkeletonLimbInterpolater
{
    private enum PositionInterpolationCurveType
    {
        Cartesian,
        Radial,
    }

    private Dictionary<GameObject, int> limbObjectIndices = new Dictionary<GameObject, int>();
    private Queue<int> deletedIndices = new Queue<int>();

    private List<GameObject> baseObjects = new List<GameObject>();

    private List<bool> positionInterpolationActive = new List<bool>();
    private List<Tuple<AnimationCurve, AnimationCurve>> positionInterpolationCurves = new List<Tuple<AnimationCurve, AnimationCurve>>();
    private List<PositionInterpolationCurveType> positionInterpolationCurveTypes = new List<PositionInterpolationCurveType>();
    private List<bool> positionInterpolationCurveLoops = new List<bool>();
    private List<float> positionInterpolationCurveDurations = new List<float>();
    private List<float> positionInterpolationEndTimes = new List<float>();

    private List<bool> rotationInterpolationActive = new List<bool>();
    private List<AnimationCurve> rotationInterpolationCurves = new List<AnimationCurve>();
    private List<bool> rotationInterpolationCurveLoops = new List<bool>();
    private List<float> rotationInterpolationCurveDurations = new List<float>();
    private List<float> rotationInterpolationEndTimes = new List<float>();

    public void InterpolateLimbAbsoluteRotation(
        GameObject baseObject, GameObject limbObject, AnimationCurve absoluteRotationCurve, bool loop)
    {
        int index = GetOrAddLimbObjectIndex(limbObject, baseObject: baseObject);
        rotationInterpolationActive[index] = true;
        rotationInterpolationCurves[index] = absoluteRotationCurve;
        rotationInterpolationCurveLoops[index] = loop;
        float duration = GetLastKeyframeInAnimationCurve(absoluteRotationCurve).time;
        rotationInterpolationCurveDurations[index] = duration;
        rotationInterpolationEndTimes[index] = Time.time + duration;
    }

    public void InterpolateLimbRelativePositionCartesian(
        GameObject baseObject, GameObject limbObject, AnimationCurve xCurve, AnimationCurve yCurve, bool loop)
    {
        int index = GetOrAddLimbObjectIndex(limbObject, baseObject: baseObject);
        positionInterpolationActive[index] = true;
        MatchAnimationCurveDurations(xCurve, yCurve, out float duration);
        positionInterpolationCurves[index] = new Tuple<AnimationCurve, AnimationCurve>(xCurve, yCurve);
        positionInterpolationCurveTypes[index] = PositionInterpolationCurveType.Cartesian;
        positionInterpolationCurveLoops[index] = loop;
        positionInterpolationCurveDurations[index] = duration;
        positionInterpolationEndTimes[index] = Time.time + duration;
    }

    public void InterpolateLimbRelativePositionRadial(
        GameObject baseObject, GameObject limbObject, AnimationCurve rCurve, AnimationCurve thetaCurve, bool loop)
    {
        int index = GetOrAddLimbObjectIndex(limbObject, baseObject: baseObject);
        positionInterpolationActive[index] = true;
        MatchAnimationCurveDurations(rCurve, thetaCurve, out float duration);
        positionInterpolationCurves[index] = new Tuple<AnimationCurve, AnimationCurve>(rCurve, thetaCurve);
        positionInterpolationCurveTypes[index] = PositionInterpolationCurveType.Radial;
        positionInterpolationCurveLoops[index] = loop;
        positionInterpolationCurveDurations[index] = duration;
        positionInterpolationEndTimes[index] = Time.time + duration;
    }

    public void SetLimbAbsoluteRotation(GameObject baseObject, GameObject limbObject, float absoluteRotation)
    {
        StopAbsoluteRotationInterpolation(baseObject, limbObject);
        limbObject.transform.rotation = Quaternion.Euler(0, 0, absoluteRotation);
    }

    public void SetLimbRelativePosition(GameObject baseObject, GameObject limbObject, Vector2 relativePosition)
    {
        StopRelativePositionInterpolation(baseObject, limbObject);
        limbObject.transform.position = new Vector3(
            baseObject.transform.position.x + relativePosition.x,
            baseObject.transform.position.y + relativePosition.y,
            limbObject.transform.position.z);
    }

    public void StopAbsoluteRotationInterpolation(GameObject baseObject, GameObject limbObject)
    {
        int index = GetOrAddLimbObjectIndex(limbObject);
        rotationInterpolationActive[index] = false;
    }

    public void StopRelativePositionInterpolation(GameObject baseObject, GameObject limbObject)
    {
        int index = GetOrAddLimbObjectIndex(limbObject);
        positionInterpolationActive[index] = false;
    }

    public void UpdateManaged()
    {
        CullDestroyedBaseAndLimbObjects();
        foreach (KeyValuePair<GameObject, int> entry in limbObjectIndices)
        {
            int index = entry.Value;
            if (positionInterpolationActive[index])
            {
                UpdateLimbObjectPositionInterpolation(entry.Key, index);
            }
            if (rotationInterpolationActive[index])
            {
                UpdateLimbObjectRotationInterpolation(entry.Key, index);
            }
        }
    }

    private int GetOrAddLimbObjectIndex(GameObject limbObject, GameObject baseObject = null)
    {
        int index;
        if (limbObjectIndices.TryGetValue(limbObject, out index))
        {
            return index;
        }
        else if (limbObject == null)
        {
            throw new Exception("Limb object must not be null or destroyed.");
        }
        else if (baseObject == null)
        {
            throw new Exception("Base object must not be null or destroyed.");
        }
        else
        {
            try
            {
                index = deletedIndices.Dequeue();
                limbObjectIndices.Add(limbObject, index);
                baseObjects[index] = baseObject;
                positionInterpolationActive[index] = false;
                rotationInterpolationActive[index] = false;
            }
            catch (InvalidOperationException)
            {
                index = limbObjectIndices.Count;
                limbObjectIndices.Add(limbObject, index);
                baseObjects.Add(baseObject);
                positionInterpolationActive.Add(false);
                positionInterpolationCurves.Add(null);
                positionInterpolationCurveTypes.Add(PositionInterpolationCurveType.Cartesian);
                positionInterpolationCurveLoops.Add(false);
                positionInterpolationEndTimes.Add(0f);
                rotationInterpolationActive.Add(false);
                rotationInterpolationCurves.Add(null);
                rotationInterpolationCurveLoops.Add(false);
                rotationInterpolationEndTimes.Add(0f);
            }
            return index;
        }
    }

    private Keyframe GetLastKeyframeInAnimationCurve(AnimationCurve curve)
    {
        Keyframe lastFrame = curve[0];
        for (int i = 1; i < curve.length; i++)
        {
            if (curve[i].time > lastFrame.time)
            {
                lastFrame = curve[i];
            }
        }
        return lastFrame;
    }

    private void MatchAnimationCurveDurations(AnimationCurve curveOne, AnimationCurve curveTwo, out float matchedDuration)
    {
        Keyframe curveOneLastKeyframe = GetLastKeyframeInAnimationCurve(curveOne);
        Keyframe curveTwoLastKeyframe = GetLastKeyframeInAnimationCurve(curveTwo);
        if (curveOneLastKeyframe.time == curveTwoLastKeyframe.time)
        {
            matchedDuration = curveOneLastKeyframe.time;
        }
        else if (curveOneLastKeyframe.time > curveTwoLastKeyframe.time)
        {
            curveTwo.AddKey(new Keyframe(curveOneLastKeyframe.time, curveTwoLastKeyframe.value));
            matchedDuration = curveOneLastKeyframe.time;
        }
        else //if (curveOneLastKeyframe.time < curveTwoLastKeyframe.time)
        {
            curveOne.AddKey(new Keyframe(curveTwoLastKeyframe.time, curveOneLastKeyframe.value));
            matchedDuration = curveTwoLastKeyframe.time;
        }
    }

    private void CullDestroyedBaseAndLimbObjects()
    {
        throw new NotImplementedException();
    }

    private void UpdateLimbObjectPositionInterpolation(GameObject limbObject, int index)
    {
        throw new NotImplementedException();
    }

    private void UpdateLimbObjectRotationInterpolation(GameObject limbObject, int index)
    {
        throw new NotImplementedException();
    }
}
