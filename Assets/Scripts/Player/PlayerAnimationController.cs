using UnityEngine;


public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] Animator _animator;
    [SerializeField] AudioSource audioSource;  
    [SerializeField] AudioClip jumpSound, rideSound, gameOverSound;  
    private static readonly int IsJumping = Animator.StringToHash("isJumping");
    private static readonly int IsRiding = Animator.StringToHash("isRiding");
    private static readonly int IsDead = Animator.StringToHash("isDead");
    private static readonly int IsRunning = Animator.StringToHash("isRunning");

    public void SetIsJumping(bool isJumping)
    {
        _animator.SetBool(IsJumping, isJumping);
        if (isJumping && jumpSound != null)
            audioSource.PlayOneShot(jumpSound);
    }

    public void SetIsRiding(bool isRiding)
    {
        _animator.SetBool(IsRiding, isRiding);
        if (isRiding && rideSound != null)
            audioSource.PlayOneShot(rideSound);
    }

    public void SetIsRunning(bool isRunning)
    {
        _animator.SetBool(IsRunning, isRunning);
    }

    public void SetIsDead(bool isDead)
    {
        _animator.SetBool(IsDead, isDead);
        if(isDead && gameOverSound != null)
            audioSource.PlayOneShot(gameOverSound);
    }
}