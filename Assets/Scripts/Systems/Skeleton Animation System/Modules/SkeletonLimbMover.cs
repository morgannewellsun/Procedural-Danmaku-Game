using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SkeletonLimbMover : ISkeletonLimbMover
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
    private List<Tuple<AnimationCurve, AnimationCurve>> positionInterpolationCurves = 
        new List<Tuple<AnimationCurve, AnimationCurve>>();
    private List<PositionInterpolationCurveType> positionInterpolationCurveTypes = 
        new List<PositionInterpolationCurveType>();
    private List<bool> positionInterpolationCurveLoops = new List<bool>();
    private List<float> positionInterpolationCurveDurations = new List<float>();
    private List<float> positionInterpolationStartTimes = new List<float>();
    private List<float> positionInterpolationEndTimes = new List<float>();

    private List<Vector2> positionFixedOffsets = new List<Vector2>();

    private List<bool> rotationInterpolationActive = new List<bool>();
    private List<AnimationCurve> rotationInterpolationCurves = new List<AnimationCurve>();
    private List<bool> rotationInterpolationCurveLoops = new List<bool>();
    private List<float> rotationInterpolationCurveDurations = new List<float>();
    private List<float> rotationInterpolationStartTimes = new List<float>();
    private List<float> rotationInterpolationEndTimes = new List<float>();

    public void Add(GameObject baseObject, GameObject limbObject)
    {
        GetOrAddLimbObjectIndex(baseObject, limbObject);
    }

    public void InterpolateLimbAbsoluteRotation(
        GameObject baseObject, GameObject limbObject, AnimationCurve absoluteRotationCurve, bool loop)
    {
        int index = GetOrAddLimbObjectIndex(baseObject, limbObject);
        rotationInterpolationActive[index] = true;
        rotationInterpolationCurves[index] = absoluteRotationCurve;
        rotationInterpolationCurveLoops[index] = loop;
        float duration = GetLastKeyframeInAnimationCurve(absoluteRotationCurve).time;
        rotationInterpolationCurveDurations[index] = duration;
        rotationInterpolationStartTimes[index] = Time.time;
        rotationInterpolationEndTimes[index] = Time.time + duration;
    }

    public void InterpolateLimbRelativePositionCartesian(
        GameObject baseObject, GameObject limbObject, AnimationCurve xCurve, AnimationCurve yCurve, bool loop)
    {
        int index = GetOrAddLimbObjectIndex(baseObject, limbObject);
        positionInterpolationActive[index] = true;
        float duration = Mathf.Max(GetLastKeyframeInAnimationCurve(xCurve).time, GetLastKeyframeInAnimationCurve(yCurve).time);
        positionInterpolationCurves[index] = new Tuple<AnimationCurve, AnimationCurve>(xCurve, yCurve);
        positionInterpolationCurveTypes[index] = PositionInterpolationCurveType.Cartesian;
        positionInterpolationCurveLoops[index] = loop;
        positionInterpolationCurveDurations[index] = duration;
        positionInterpolationStartTimes[index] = Time.time;
        positionInterpolationEndTimes[index] = Time.time + duration;
        positionFixedOffsets[index] = Vector2.zero;
    }

    public void InterpolateLimbRelativePositionRadial(
        GameObject baseObject, GameObject limbObject, AnimationCurve rCurve, AnimationCurve thetaCurve, bool loop)
    {
        int index = GetOrAddLimbObjectIndex(baseObject, limbObject);
        positionInterpolationActive[index] = true;
        float duration = Mathf.Max(GetLastKeyframeInAnimationCurve(rCurve).time, GetLastKeyframeInAnimationCurve(thetaCurve).time);
        positionInterpolationCurves[index] = new Tuple<AnimationCurve, AnimationCurve>(rCurve, thetaCurve);
        positionInterpolationCurveTypes[index] = PositionInterpolationCurveType.Radial;
        positionInterpolationCurveLoops[index] = loop;
        positionInterpolationCurveDurations[index] = duration;
        positionInterpolationStartTimes[index] = Time.time;
        positionInterpolationEndTimes[index] = Time.time + duration;
        positionFixedOffsets[index] = Vector2.zero;
    }

    public void SetLimbAbsoluteRotation(GameObject baseObject, GameObject limbObject, float absoluteRotation)
    {
        StopAbsoluteRotationInterpolation(baseObject, limbObject);
        limbObject.transform.rotation = Quaternion.Euler(0, 0, absoluteRotation);
    }

    public void SetLimbRelativePosition(GameObject baseObject, GameObject limbObject, Vector2 relativePosition)
    {
        StopRelativePositionInterpolation(baseObject, limbObject);
        int index = GetOrAddLimbObjectIndex(baseObject, limbObject);
        positionFixedOffsets[index] = relativePosition;
    }

    public void StopAbsoluteRotationInterpolation(GameObject baseObject, GameObject limbObject)
    {
        int index = GetOrAddLimbObjectIndex(baseObject, limbObject);
        rotationInterpolationActive[index] = false;
    }

    public void StopRelativePositionInterpolation(GameObject baseObject, GameObject limbObject)
    {
        int index = GetOrAddLimbObjectIndex(baseObject, limbObject);
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
                if (positionInterpolationCurveTypes[index] == PositionInterpolationCurveType.Cartesian)
                {
                    UpdateLimbObjectPositionInterpolationCartesian(entry.Key, index);
                } else //if (positionInterpolationCurveTypes[index] == PositionInterpolationCurveType.Radial)
                {
                    UpdateLimbObjectPositionInterpolationRadial(entry.Key, index);
                }
            }
            else
            {
                UpdateLimbObjectPositionUsingFixedOffset(entry.Key, index);
            }
            if (rotationInterpolationActive[index])
            {
                UpdateLimbObjectRotationInterpolation(entry.Key, index);
            }
        }
    }

    private int GetOrAddLimbObjectIndex(GameObject baseObject, GameObject limbObject)
    {
        Debug.Log("called");
        int index;
        if (limbObjectIndices.TryGetValue(limbObject, out index))
        {
            return index;
        }
        else if (limbObject == null)
        {
            throw new Exception("Added limb object must not be null or destroyed.");
        }
        else if (baseObject == null)
        {
            throw new Exception("Base object of added limb must not be null or destroyed.");
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
                positionInterpolationStartTimes.Add(0f);
                positionInterpolationEndTimes.Add(0f);
                positionFixedOffsets.Add(Vector2.zero);
                rotationInterpolationActive.Add(false);
                rotationInterpolationCurves.Add(null);
                rotationInterpolationCurveLoops.Add(false);
                rotationInterpolationStartTimes.Add(0f);
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

    private void CullDestroyedBaseAndLimbObjects()
    {
        List<GameObject> deletedLimbObjects = new List<GameObject>();
        List<int> deletedIndicesThisFrame = new List<int>();
        foreach (KeyValuePair<GameObject, int> entry in limbObjectIndices)
        {
            if (entry.Key == null | baseObjects[entry.Value] == null)
            {
                deletedLimbObjects.Add(entry.Key);
                deletedIndicesThisFrame.Add(entry.Value);
            }
        }
        foreach (GameObject deletedLimbObject in deletedLimbObjects)
        {
            limbObjectIndices.Remove(deletedLimbObject);
        }
        foreach (int deletedIndex in deletedIndicesThisFrame)
        {
            deletedIndices.Enqueue(deletedIndex);
        }
    }

    private void UpdateLimbObjectPositionInterpolationCartesian(GameObject limbObject, int index)
    {
        if (Time.time > positionInterpolationEndTimes[index])
        {
            if (positionInterpolationCurveLoops[index])
            {
                positionInterpolationStartTimes[index] = Time.time;
                positionInterpolationEndTimes[index] = Time.time + positionInterpolationCurveDurations[index];
            }
            else
            {
                positionInterpolationActive[index] = false;
                return;
            }
        }
        float timeSinceCurveStart = Time.time - positionInterpolationStartTimes[index];
        float xCurveEvaluated = positionInterpolationCurves[index].Item1.Evaluate(timeSinceCurveStart);
        float yCurveEvaluated = positionInterpolationCurves[index].Item2.Evaluate(timeSinceCurveStart);
        Transform baseTransform = baseObjects[index].transform;
        limbObject.transform.position = new Vector3(
            baseTransform.position.x + xCurveEvaluated,
            baseTransform.position.y + yCurveEvaluated,
            limbObject.transform.position.z);
    }

    private void UpdateLimbObjectPositionInterpolationRadial(GameObject limbObject, int index)
    {
        if (Time.time > positionInterpolationEndTimes[index])
        {
            if (positionInterpolationCurveLoops[index])
            {
                positionInterpolationStartTimes[index] = Time.time;
                positionInterpolationEndTimes[index] = Time.time + positionInterpolationCurveDurations[index];
            }
            else
            {
                positionInterpolationActive[index] = false;
                return;
            }
        }
        float timeSinceCurveStart = Time.time - positionInterpolationStartTimes[index];
        float rCurveEvaluated = positionInterpolationCurves[index].Item1.Evaluate(timeSinceCurveStart);
        float thetaCurveEvaluated = positionInterpolationCurves[index].Item2.Evaluate(timeSinceCurveStart);
        float xOffset = rCurveEvaluated * Mathf.Cos(thetaCurveEvaluated * Mathf.Deg2Rad);
        float yOffset = rCurveEvaluated * Mathf.Sin(thetaCurveEvaluated * Mathf.Deg2Rad);
        Transform baseTransform = baseObjects[index].transform;
        limbObject.transform.position = new Vector3(
            baseTransform.position.x + xOffset,
            baseTransform.position.y + yOffset,
            limbObject.transform.position.z);
    }

    private void UpdateLimbObjectRotationInterpolation(GameObject limbObject, int index)
    {
        if (Time.time > rotationInterpolationEndTimes[index])
        {
            if (rotationInterpolationCurveLoops[index])
            {
                rotationInterpolationStartTimes[index] = Time.time;
                rotationInterpolationEndTimes[index] = Time.time + rotationInterpolationCurveDurations[index];
            }
            else
            {
                rotationInterpolationActive[index] = false;
                return;
            }
        }
        float timeSinceCurveStart = Time.time - rotationInterpolationStartTimes[index];
        float rotationInterpolationCurveEvaluated = rotationInterpolationCurves[index].Evaluate(timeSinceCurveStart);
        limbObject.transform.rotation = Quaternion.Euler(0, 0, rotationInterpolationCurveEvaluated);
    }

    private void UpdateLimbObjectPositionUsingFixedOffset(GameObject limbObject, int index)
    {
        Transform baseTransform = baseObjects[index].transform;
        limbObject.transform.position = new Vector3(
            baseTransform.position.x + positionFixedOffsets[index].x,
            baseTransform.position.y + positionFixedOffsets[index].y,
            limbObject.transform.position.z);
    }
}
