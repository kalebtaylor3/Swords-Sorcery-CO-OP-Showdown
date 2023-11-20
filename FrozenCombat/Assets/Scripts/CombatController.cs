using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[RequireComponent(typeof(ThirdPersonController))]
public class CombatController : MonoBehaviour
{
    public ThirdPersonController cc;
    [HideInInspector] public bool inCombat = false;
    float combatCooldownTimer = 0f;
    float combatCooldownDuration = 15f;
    float swingCooldownTimer = 0f;
    float swingCoolDownDuration = 1.2f;
    public AnimationClip swing;

    public RuntimeAnimatorController normalAnimations;
    public RuntimeAnimatorController combatAnimations;
    public Animator currentAnimator;


    public GameObject sword;


    public virtual void HandleSwing()
    {
        if (swingCooldownTimer <= 0f)
        {
            cc.canRotate = true;
            cc.animator.SetTrigger("Swing");
            swingCooldownTimer = swing.length;
        }

    }

    public void DrawSword()
    {
        sword.SetActive(true);
    }

    public void PutAwaySword()
    {
        sword.SetActive(false);
    }

    void Start()
    {
        sword.SetActive(false);
    }

    void Update()
    {

        if (swingCooldownTimer > 0f)
        {
            LockMovement(true);
            swingCooldownTimer -= Time.deltaTime;
            if (swingCooldownTimer <= 0f)
            {
                cc.animator.ResetTrigger("Swing");
                LockMovement(false);
            }
        }

    }

    public void TriggerCombat(bool value)
    {
        cc.Sprint(!value);
        cc.canRun = !value;
        cc.Strafe();
        cc.canRotate = !value;
        inCombat = value;
        //inCombat = true;
        if(value == true)
            currentAnimator.runtimeAnimatorController = combatAnimations;
        else
            currentAnimator.runtimeAnimatorController = normalAnimations;

        currentAnimator.SetTrigger("Combat");
    }

    void LockMovement(bool value)
    {
        cc.lockMovement = value;
        //cc.verticalVelocity = 0;
        cc.horizontalSpeed = 0;
        cc.verticalSpeed = 0;
        cc.input.z = 0;
        cc.input.x = 0;
        //cc._rigidbody.velocity = new Vector3(0,0,0);
    }
}
