using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    public VirtualJoystick moveJoystick;

    public Transform playerCamera;
    public float turnSmoothing = 0.06f;

    private float h;
    private float v;
    private int currentBehaviour;
    private int defaultBehaviour;
    private int behaviourLocked;
    private Vector3 lastDirection;
    private Animator anim;
    private int hFloat;
    private int vFloat;
    private List<PlayerState> behaviours;
    private List<PlayerState> overridingBehaviours;
    private Rigidbody rBody;
    private int groundedBool;
    private Vector3 colExtents;

    public float GetH => h;
    public float GetV => v;
    public Rigidbody GetRigidBody => rBody;
    public Animator GetAnim => anim;

    void Awake()
    {
        behaviours = new List<PlayerState>();
        overridingBehaviours = new List<PlayerState>();
        anim = GetComponent<Animator>();
        hFloat = Animator.StringToHash("H");
        vFloat = Animator.StringToHash("V");


        rBody = GetComponent<Rigidbody>();
        groundedBool = Animator.StringToHash("Grounded");
        colExtents = GetComponent<Collider>().bounds.extents;
    }

    void Update()
    {
        Vector2 joystickInput = Vector2.zero;
        Vector2 keyboardInput = Vector2.zero;

        if (moveJoystick != null)
        {
            joystickInput = moveJoystick.InputVector;
        }

        if (Keyboard.current != null)
        {
            float keyX = 0f;
            float keyY = 0f;

            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) keyY += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) keyY -= 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) keyX -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) keyX += 1f;

            keyboardInput = new Vector2(keyX, keyY).normalized;
        }

        Vector2 finalInput = (joystickInput.sqrMagnitude > keyboardInput.sqrMagnitude) ? joystickInput : keyboardInput;

        h = finalInput.x;
        v = finalInput.y;

        anim.SetFloat(hFloat, h, 0.1f, Time.deltaTime);
        anim.SetFloat(vFloat, v, 0.1f, Time.deltaTime);

        anim.SetBool(groundedBool, IsGrounded());
    }

    void FixedUpdate()
    {
        bool isAnyBehaviourActive = false;
        if (behaviourLocked > 0 || overridingBehaviours.Count == 0)
        {
            foreach (PlayerState behaviour in behaviours)
            {
                if (behaviour.isActiveAndEnabled && currentBehaviour == behaviour.GetBehaviourCode())
                {
                    isAnyBehaviourActive = true;
                    behaviour.LocalFixedUpdate();
                }
            }
        }
        else
        {
            foreach (PlayerState behaviour in overridingBehaviours)
            {
                behaviour.LocalFixedUpdate();
            }
        }

        if (!isAnyBehaviourActive && overridingBehaviours.Count == 0)
        {
            rBody.useGravity = true;
            Repositioning();
        }
    }

    private void LateUpdate()
    {
        if (behaviourLocked > 0 || overridingBehaviours.Count == 0)
        {
            foreach (PlayerState behaviour in behaviours)
            {
                if (behaviour.isActiveAndEnabled && currentBehaviour == behaviour.GetBehaviourCode())
                {
                    behaviour.LocalLateUpdate();
                }
            }
        }
        else
        {
            foreach (PlayerState behaviour in overridingBehaviours)
            {
                behaviour.LocalLateUpdate();
            }
        }
    }

    public void SubscribeBehaviour(PlayerState behaviour) => behaviours.Add(behaviour);

    public void RegisterDefaultBehaviour(int behaviourCode)
    {
        defaultBehaviour = behaviourCode;
        currentBehaviour = behaviourCode;
    }

    public bool IsCurrentBehaviour(int behaviourCode) => currentBehaviour == behaviourCode;

    public bool GetTempLockStatus(int behaviourCodeIgnoreSelf = 0)
    {
        return (behaviourLocked != 0 && behaviourLocked != behaviourCodeIgnoreSelf);
    }

    public void LockTempBehaviour(int behaviourCode)
    {
        if (behaviourLocked == 0) behaviourLocked = behaviourCode;
    }

    public void UnlockTempBehaviour(int behaviourCode)
    {
        if (behaviourLocked == 0 || behaviourLocked == behaviourCode)
        {
            behaviourLocked = 0;
        }
    }

    public bool IsOverriding(PlayerState behaviour = null)
    {
        return behaviour == null ? overridingBehaviours.Count > 0 : overridingBehaviours.Contains(behaviour);
    }

    public bool IsMoving() => (h != 0) || (v != 0);
    public Vector3 GetLastDirection() => lastDirection;
    public void SetLastDirection(Vector3 direction) => lastDirection = direction;

    public void Repositioning()
    {
        if (lastDirection != Vector3.zero)
        {
            lastDirection.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(lastDirection);
            Quaternion newRotation = Quaternion.Slerp(rBody.rotation, targetRotation, turnSmoothing);
            rBody.MoveRotation(newRotation);
        }
    }

    public bool IsGrounded()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 2 * colExtents.x, Vector3.down);
        return Physics.SphereCast(ray, colExtents.x, colExtents.x + 0.2f);
    }
}