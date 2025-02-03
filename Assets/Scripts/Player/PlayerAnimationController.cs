using UnityEngine;


public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] Animator _animator;
    private static readonly int IsJumping = Animator.StringToHash("isJumping");
    private static readonly int IsRiding = Animator.StringToHash("isRiding");
    private static readonly int IsDead = Animator.StringToHash("isDead");
    private static readonly int IsRunning = Animator.StringToHash("isRunning");

    public void SetIsJumping(bool isJumping)
    {
        // SetIsRunning(false);
        // SetIsRiding(false);
        _animator.SetBool(IsJumping, isJumping);
    }

    public void SetIsRiding(bool isRiding)
    {
        // SetIsRunning(false);
        // SetIsJumping(false);
        _animator.SetBool(IsRiding, isRiding);
    }

    public void SetIsRunning(bool isRunning)
    {
        // SetIsJumping(false);
        // SetIsRiding(false);
        _animator.SetBool(IsRunning, isRunning);
    }

    public void SetIsDead(bool isDead)
    {
        // SetIsJumping(false);
        // SetIsRiding(false);
        _animator.SetBool(IsDead, isDead);
    }
}