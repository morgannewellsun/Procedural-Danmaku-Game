using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TesterScript : MonoBehaviour
{
    public bool step;
    public int prevLine;

    public GameObject testObject;
    public GameObject testObject2;
    public GameObject testObject3;

    void Start()
    {
        step = false;
        prevLine = 0;
    }

    void Update()
    {
        if (step)
        {
            step = false;
            prevLine += 1;
            Debug.Log($"Running line {prevLine}...");
            switch (prevLine)
            {
                case 1:
                    testObject = new GameObject("Test Object");
                    testObject2 = new GameObject("Test Object 2");
                    testObject3 = new GameObject("Test Object 3");
                    break;
                case 2:
                    GameDirector.skeletonAnimationSystem.AttachLimb(testObject, "Test Limb");
                    break;
                case 3:
                    GameObject limbObject = GameDirector.skeletonAnimationSystem.GetLimb(testObject, "Test Limb");
                    GameDirector.spriteAnimationSystem.Add(limbObject, "entities");
                    GameDirector.spriteAnimationSystem.ApplyAnimation(limbObject, "test_animation");
                    break;
                case 4:
                    GameDirector.skeletonAnimationSystem.SetLimbRelativePosition(testObject, "Test Limb", new Vector2(3, 2));
                    break;
                default:
                    Debug.Log("No more Lines");
                    break;
            }
        }
    }
}
