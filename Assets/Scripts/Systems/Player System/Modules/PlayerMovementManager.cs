using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovementManager : IPlayerMovementManager
{
    private const string movementLimbLabel = "Movement Limb";
    private GameObject movementLimbObject;

    private const float speedMultiplierUnfocused = 1f;
    private const float speedMultiplierFocused = 0.5f;

    private Rect movementBounds = new Rect(-5, -5, 10, 10);

    int prevHorizontalMovement = 0;
    int prevVerticalMovement = 0;
    int currHorizontalMovement = 0;
    int currVerticalMovement = 0;

    public void CreateLimbObject()
    {
        movementLimbObject = GameDirector.skeletonAnimationSystem.AttachLimb(
            GameDirector.playerSystem.GetPlayerBaseObject(), movementLimbLabel);
        GameDirector.spriteAnimationSystem.Add(movementLimbObject, "entities");
    }

    public void UpdateMovement()
    {
        ParseDirectionalMovementInputs();
        ApplyMovementInputsToPlayerPosition();
        AnimateMovementLimbObject();
    }

    private void ParseDirectionalMovementInputs()
    {
        prevHorizontalMovement = currHorizontalMovement;
        prevVerticalMovement = currVerticalMovement;
        currHorizontalMovement = (Input.GetButton("right") ? 1 : 0) - (Input.GetButton("left") ? 1 : 0);
        currVerticalMovement = (Input.GetButton("up") ? 1 : 0) - (Input.GetButton("down") ? 1 : 0);
    }

    private void ApplyMovementInputsToPlayerPosition()
    {
        Vector2 movementDelta = new Vector3(currHorizontalMovement, currVerticalMovement);
        if (currHorizontalMovement != 0 & currVerticalMovement != 0)
        {
            movementDelta *= 0.70710678118f; // sqrt(1/2)
        }
        movementDelta *= Time.deltaTime;
        movementDelta *= (Input.GetButton("focus") ? speedMultiplierFocused : speedMultiplierUnfocused);
        GameDirector.playerSystem.GetPlayerBaseObject().transform.position = new Vector3(
            Mathf.Clamp(
                GameDirector.playerSystem.GetPlayerBaseObject().transform.position.x + movementDelta.x,
                movementBounds.xMin,
                movementBounds.xMax),
            Mathf.Clamp(
                GameDirector.playerSystem.GetPlayerBaseObject().transform.position.y + movementDelta.y,
                movementBounds.yMin,
                movementBounds.yMax),
            GameDirector.playerSystem.GetPlayerBaseObject().transform.position.z);
    }

    private void AnimateMovementLimbObject()
    {
        if ((currHorizontalMovement != prevHorizontalMovement) 
            || (currHorizontalMovement == 0 && currVerticalMovement != prevVerticalMovement))
        {
            ResetMovementLimbObjectAnimation();
        }
        else if (Input.GetButtonUp("focus") || Input.GetButtonDown("focus"))
        {
            SetMovementLimbObjectAnimationSpeed();
        }
    }

    private void ResetMovementLimbObjectAnimation()
    {
        string animationName;
        if (currHorizontalMovement == 1)
        {
            animationName = "sokoban_player_right_walking";
        }
        else if (currHorizontalMovement == -1)
        {
            animationName = "sokoban_player_left_walking";
        }
        else if (currVerticalMovement == 1)
        {
            animationName = "sokoban_player_up_walking";
        }
        else if (currVerticalMovement == -1)
        {
            animationName = "sokoban_player_down_walking";
        }
        else
        {
            if (prevHorizontalMovement == 1)
            {
                animationName = "sokoban_player_right_standing";
            }
            else if (prevHorizontalMovement == -1)
            {
                animationName = "sokoban_player_left_standing";
            }
            else if (prevVerticalMovement == 1)
            {
                animationName = "sokoban_player_up_standing";
            }
            else // if (prevVerticalMovement == -1 || prevVerticalMovement == 0)
            {
                animationName = "sokoban_player_down_standing";
            }
        }
        float speedMultiplier = Input.GetButton("focus") ? speedMultiplierFocused : speedMultiplierUnfocused;
        GameDirector.spriteAnimationSystem.ApplyAnimation(movementLimbObject, animationName, speedMultiplier);
    }

    private void SetMovementLimbObjectAnimationSpeed()
    {
        float speedMultiplier = Input.GetButton("focus") ? speedMultiplierFocused : speedMultiplierUnfocused;
        GameDirector.spriteAnimationSystem.SetAnimationSpeed(movementLimbObject, speedMultiplier);
    }
}
