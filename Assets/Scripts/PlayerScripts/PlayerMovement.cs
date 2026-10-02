using UnityEngine;

public class PlayerMovement : PlayerState
{
    public float moveSpeed = 4.0f;
    public float speedDampTime = 0.15f;

    private Vector3 moveDirection;

    void Start()
    {
        behaviourManager.SubscribeBehaviour(this);
        behaviourManager.RegisterDefaultBehaviour(this.behaviourCode);
    }

    public override void LocalFixedUpdate()
    {
        MovementManagement(behaviourManager.GetH, behaviourManager.GetV);
    }

    void MovementManagement(float horizontal, float vertical)
    {
        if (behaviourManager.IsGrounded())
        {
            behaviourManager.GetRigidBody.useGravity = true;
        }

        moveDirection = CalculateDirection(horizontal, vertical);

        Vector2 input = new Vector2(horizontal, vertical);
        float inputMagnitude = Mathf.Clamp01(input.magnitude);

        if (inputMagnitude > 0.05f && moveDirection.sqrMagnitude > 0.01f)
        {
            Vector3 flatDirection = new Vector3(moveDirection.x, 0f, moveDirection.z).normalized;
            if (flatDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(flatDirection, Vector3.up);
                Quaternion smoothedRotation = Quaternion.Slerp(
                    behaviourManager.GetRigidBody.rotation,
                    targetRotation,
                    behaviourManager.turnSmoothing * 25f * Time.fixedDeltaTime
                );
                behaviourManager.GetRigidBody.MoveRotation(smoothedRotation);
                behaviourManager.SetLastDirection(flatDirection);
            }
        }

        float targetSpeed = inputMagnitude * moveSpeed;

        Vector3 currentVel = behaviourManager.GetRigidBody.linearVelocity;
        Vector3 targetVelocity = moveDirection * targetSpeed;
        targetVelocity.y = currentVel.y;

        behaviourManager.GetRigidBody.linearVelocity = Vector3.Lerp(currentVel, targetVelocity, 15f * Time.fixedDeltaTime);
        behaviourManager.GetAnim.SetFloat(speedFloat, inputMagnitude, speedDampTime, Time.fixedDeltaTime);
    }

    private Vector3 CalculateDirection(float horizontal, float vertical)
    {
        if (behaviourManager.playerCamera == null) return Vector3.zero;

        Vector3 camForward = behaviourManager.playerCamera.forward;
        Vector3 camRight = behaviourManager.playerCamera.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * vertical + camRight * horizontal);
        return moveDir.sqrMagnitude > 1f ? moveDir.normalized : moveDir;
    }
}