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
                    break;
                case 2:
                    GameDirector.spriteAnimationSystem.Add(testObject, "entities");
                    break;
                case 3:
                    GameDirector.spriteAnimationSystem.ApplyAnimation(testObject, "test_animation");
                    break;
                case 4:
                    GameDirector.spriteAnimationSystem.SetAnimationSpeed(testObject, 5);
                    break;
                default:
                    Debug.Log("No more Lines");
                    break;
            }
        }
    }
}
