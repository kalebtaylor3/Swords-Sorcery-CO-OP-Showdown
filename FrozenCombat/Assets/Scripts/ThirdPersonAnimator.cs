using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonAnimator : ThirdPersonMotor
{
    #region Variables                

    public const float walkSpeed = 0.5f;
    public const float runningSpeed = 1f;
    public const float sprintSpeed = 1.5f;

    public Transform body;
    public Transform cameraT;


    float turnDelay = 0.2f;
    float currentTime = 0.0f;
    float mouseXSmooth;

    float keyInput;

    private string currentDirection;

    #endregion

    public virtual void UpdateAnimator()
    {
        if (animator == null || !animator.enabled) return;

        animator.SetBool(vAnimatorParameters.IsStrafing, isStrafing); ;
        animator.SetBool(vAnimatorParameters.IsSprinting, isSprinting);
        animator.SetBool(vAnimatorParameters.IsGrounded, isGrounded);
        animator.SetFloat(vAnimatorParameters.GroundDistance, groundDistance);

        keyInput = Input.GetAxis("Horizontal");


        // Get the character's forward vector relative to its local space
        Vector3 characterForward = body.InverseTransformDirection(Vector3.forward);

        // Convert the character's forward vector to the camera's local space
        Vector3 cameraForward = cameraT.TransformDirection(characterForward);

        // Calculate the angle between the character's forward vector and the camera's forward vector
        float angle = Vector3.SignedAngle(cameraForward, Vector3.forward, Vector3.up);

        // Determine the direction based on the angle value
        string direction = GetDirection(angle);

        // Log the direction
        Debug.Log("Facing: " + direction);

        if (isStrafing)
        {

            if (direction == "Backwards" && direction != "Forward")
            {
                animator.SetFloat(vAnimatorParameters.InputVertical, stopMove ? 0 : -verticalSpeed, strafeSpeed.animationSmooth, Time.deltaTime);
                animator.SetFloat(vAnimatorParameters.InputHorizontal, stopMove ? 0 : -horizontalSpeed, strafeSpeed.animationSmooth, Time.deltaTime);
            }

            //if facing right and forward is pressed set the horizontal speed to be 0

            if( direction == "Right" && direction != "Left")
            {
                animator.SetFloat(vAnimatorParameters.InputHorizontal, stopMove ? 0 : -verticalSpeed, strafeSpeed.animationSmooth, Time.deltaTime);
                animator.SetFloat(vAnimatorParameters.InputVertical, stopMove ? 0 : horizontalSpeed, strafeSpeed.animationSmooth, Time.deltaTime);

                currentDirection = "Right";
            }


            if(direction == "Left" && direction != "Right")
            {
                animator.SetFloat(vAnimatorParameters.InputHorizontal, stopMove ? 0 : verticalSpeed, strafeSpeed.animationSmooth, Time.deltaTime);
                animator.SetFloat(vAnimatorParameters.InputVertical, stopMove ? 0 : -horizontalSpeed, strafeSpeed.animationSmooth, Time.deltaTime);

                currentDirection = "Left";
            }

            if(direction == "Forward" && direction != "Backwards")
            {
                animator.SetFloat(vAnimatorParameters.InputHorizontal, stopMove ? 0 : horizontalSpeed, strafeSpeed.animationSmooth, Time.deltaTime);
                animator.SetFloat(vAnimatorParameters.InputVertical, stopMove ? 0 : verticalSpeed, strafeSpeed.animationSmooth, Time.deltaTime);

                currentDirection = "Forward";
            }
        }
        else
        {
            animator.SetFloat(vAnimatorParameters.InputVertical, stopMove ? 0 : verticalSpeed, freeSpeed.animationSmooth, Time.deltaTime);
        }

        animator.SetFloat(vAnimatorParameters.InputMagnitude, stopMove ? 0f : inputMagnitude, isStrafing ? strafeSpeed.animationSmooth : freeSpeed.animationSmooth, Time.deltaTime);
    }

    string GetDirection(float angle)
    {
        // Set the angular threshold for each direction
        float forwardThreshold = 45f;
        float backwardThreshold = 135f;
        float leftThreshold = -135f;
        float rightThreshold = -45f;

        // Determine the direction based on the angle value
        if (angle < forwardThreshold && angle >= rightThreshold)
        {
            return "Forward";
        }
        else if (angle >= forwardThreshold && angle < backwardThreshold)
        {
            return "Right"; // done
        }
        else if (angle >= backwardThreshold || angle < leftThreshold)
        {
            return "Backwards"; // done
        }
        else if (angle >= leftThreshold && angle < rightThreshold)
        {
            return "Left"; // done
        }

        return "Unknown";
    }


    public virtual void SetAnimatorMoveSpeed(vMovementSpeed speed)
    {
        Vector3 relativeInput = transform.InverseTransformDirection(moveDirection);
        verticalSpeed = relativeInput.z;
        horizontalSpeed = relativeInput.x;

        var newInput = new Vector2(verticalSpeed, horizontalSpeed);

        if (speed.walkByDefault)
            inputMagnitude = Mathf.Clamp(newInput.magnitude, 0, isSprinting ? runningSpeed : walkSpeed);
        else
            inputMagnitude = Mathf.Clamp(isSprinting ? newInput.magnitude + 0.5f : newInput.magnitude, 0, isSprinting ? sprintSpeed : runningSpeed);
    }
}

public static partial class vAnimatorParameters
{
    public static int InputHorizontal = Animator.StringToHash("InputHorizontal");
    public static int InputVertical = Animator.StringToHash("InputVertical");
    public static int InputMagnitude = Animator.StringToHash("InputMagnitude");
    public static int IsGrounded = Animator.StringToHash("IsGrounded");
    public static int IsStrafing = Animator.StringToHash("IsStrafing");
    public static int IsSprinting = Animator.StringToHash("IsSprinting");
    public static int GroundDistance = Animator.StringToHash("GroundDistance");
}
