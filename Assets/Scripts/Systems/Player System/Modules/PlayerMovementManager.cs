using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovementManager : IPlayerMovementManager
{
    private Rect movementBounds = new Rect(-5, -5, 10, 10);

    private float speedMultiplierUnfocused = 1f;
    private float speedMultiplierFocused = 0.5f;

    public void UpdateMovement()
    {
        ApplyMovementInputsToPlayerPosition();
        BoundPlayerPosition();
        SetPlayerSpriteAnimationOnMovementInputChange();
    }

    private void ApplyMovementInputsToPlayerPosition()
    {
        int horizontalMovement = (Input.GetButton("right") ? 1 : 0) - (Input.GetButton("left") ? 1 : 0);
        int verticalMovement = (Input.GetButton("up") ? 1 : 0) - (Input.GetButton("down") ? 1 : 0);
        Vector3 movementDelta = new Vector3(horizontalMovement, verticalMovement, 0);
        if (horizontalMovement != 0 & verticalMovement != 0)
        {
            movementDelta *= 0.70710678118f; // sqrt(1/2)
        }
        movementDelta *= Time.deltaTime;
        movementDelta *= (Input.GetButton("focus") ? speedMultiplierFocused : speedMultiplierUnfocused);
        GameDirector.playerSystem.GetPlayerBaseObject().transform.position += movementDelta;
    }

    private void BoundPlayerPosition()
    {
        GameDirector.playerSystem.GetPlayerBaseObject().transform.position = new Vector3(
            Mathf.Clamp(
                GameDirector.playerSystem.GetPlayerBaseObject().transform.position.x,
                movementBounds.xMin,
                movementBounds.xMax),
            Mathf.Clamp(
                GameDirector.playerSystem.GetPlayerBaseObject().transform.position.y,
                movementBounds.yMin,
                movementBounds.yMax),
            GameDirector.playerSystem.GetPlayerBaseObject().transform.position.z);
    }

    private void SetPlayerSpriteAnimationOnMovementInputChange()
    {
        // TODO: waiting for ability to hot-change sprite animation sequence speed.
    }
}
