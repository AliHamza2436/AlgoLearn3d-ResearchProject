using UnityEngine;

public abstract class PlayerState : MonoBehaviour
{
    protected int speedFloat;
    protected PlayerInputController behaviourManager;
    protected int behaviourCode;

    void Awake()
    {
        behaviourManager = GetComponent<PlayerInputController>();
        speedFloat = Animator.StringToHash("Speed");
        behaviourCode = this.GetType().GetHashCode();
    }

    public virtual void LocalFixedUpdate() { }
    public virtual void LocalLateUpdate() { }
    public virtual void OnOverride() { }

    public int GetBehaviourCode() => behaviourCode;
}