using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    #region Variables       

    [Header("Controller Input")]
    public string horizontalInput = "Horizontal";
    public string verticallInput = "Vertical";
    public KeyCode jumpInput = KeyCode.Space;
    public KeyCode strafeInput = KeyCode.Tab;
    public KeyCode sprintInput = KeyCode.LeftShift;

    [Header("Camera Input")]
    public string rotateCameraXInput = "Mouse X";
    public string rotateCameraYInput = "Mouse Y";

    [HideInInspector] public ThirdPersonController cc;
    [HideInInspector] public CombatController combatC;
    [HideInInspector] public vThirdPersonCamera tpCamera;
    [HideInInspector] public Camera cameraMain;
    public float rotationSpeed = 5f; // Speed at which the player rotates

    #endregion

    protected virtual void Start()
    {
        InitilizeController();
        InitializeTpCamera();
    }

    protected virtual void FixedUpdate()
    {
        cc.UpdateMotor();               // updates the ThirdPersonMotor methods
        cc.ControlLocomotionType();     // handle the controller locomotion type and movespeed
        cc.ControlRotationType();       // handle the controller rotation type
    }

    protected virtual void Update()
    {
        InputHandle();                  // update the input methods
        cc.UpdateAnimator();            // updates the Animator Parameters
        cc.RotateCharacter();
    }

    public virtual void OnAnimatorMove()
    {
        cc.ControlAnimatorRootMotion(); // handle root motion animations 
    }

    #region Basic Locomotion Inputs

    protected virtual void InitilizeController()
    {
        cc = GetComponent<ThirdPersonController>();
        combatC = GetComponent <CombatController>();

        if (cc != null)
            cc.Init();
    }

    protected virtual void InitializeTpCamera()
    {
        if (tpCamera == null)
        {
            tpCamera = FindObjectOfType<vThirdPersonCamera>();
            if (tpCamera == null)
                return;
            if (tpCamera)
            {
                tpCamera.SetMainTarget(this.transform);
                tpCamera.Init();
            }
        }
    }

    protected virtual void InputHandle()
    {
        if (!cc.lockMovement)
        {
            MoveInput();
            CameraInput();
            SprintInput();
            StrafeInput();
        }
        JumpInput();
        HandleRotation();
        CombatInput();
    }

    protected virtual void HandleRotation()
    {
        //Vector3 mousePosition = Input.mousePosition;

        //// Convert the screen position of the cursor into a world position on the camera's near plane
        //mousePosition.z = cameraMain.nearClipPlane;
        //Vector3 worldPosition = cameraMain.ScreenToWorldPoint(mousePosition);

        //// Calculate the direction from the player's position to the cursor position
        //Vector3 direction = worldPosition - transform.position;

        //// Calculate the rotation angle around the Y-axis based on the direction
        //float rotationAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        //// Apply the rotation to the player's transform
        //transform.rotation = Quaternion.Euler(0, rotationAngle, 0);
    }

    public virtual void MoveInput()
    {
        cc.input.x = Input.GetAxis(horizontalInput);
        cc.input.z = Input.GetAxis(verticallInput);

        if (cc.isStrafing)
            cc.canRotate = true;

        if (cc.input.x != 0 && !cc.isStrafing || cc.input.z != 0 && !cc.isStrafing)
        {
            cc.canRotate = false;
        }
        else
        {
            cc.canRotate = true;
        }
    }

    public virtual void CombatInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(combatC.inCombat)
                combatC.HandleSwing();
        }

        if(Input.GetKeyUp(KeyCode.C))
        {
            combatC.TriggerCombat(!combatC.inCombat);
        }

    }

    protected virtual void CameraInput()
    {
        if (!cameraMain)
        {
            if (!Camera.main) Debug.Log("Missing a Camera with the tag MainCamera, please add one.");
            else
            {
                cameraMain = Camera.main;
                cc.rotateTarget = cameraMain.transform;
            }
        }

        if (cameraMain)
        {
            cc.UpdateMoveDirection(cameraMain.transform);
        }

        if (tpCamera == null)
            return;

        var Y = Input.GetAxis(rotateCameraYInput);
        var X = Input.GetAxis(rotateCameraXInput);

        tpCamera.RotateCamera(X, Y);
    }

    protected virtual void StrafeInput()
    {
        if (Input.GetKeyDown(strafeInput))
            cc.Strafe();
    }

    protected virtual void SprintInput()
    {
        if (Input.GetKeyDown(sprintInput))
            cc.Sprint(true);
        else if (Input.GetKeyUp(sprintInput))
            cc.Sprint(false);
    }

    /// <summary>
    /// Conditions to trigger the Jump animation & behavior
    /// </summary>
    /// <returns></returns>
    protected virtual bool JumpConditions()
    {
        return cc.isGrounded && cc.GroundAngle() < cc.slopeLimit && !cc.isJumping && !cc.stopMove;
    }

    /// <summary>
    /// Input to trigger the Jump 
    /// </summary>
    protected virtual void JumpInput()
    {
        if (Input.GetKeyDown(jumpInput) && JumpConditions())
            cc.Jump();
    }



    #endregion
}
