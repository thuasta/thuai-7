using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    public const float MinWalkDistance = 1f;
    public const float DeadTime = 2f;
    public const float AttackTime = 0.5f;
    public const float UseMedicineTime = 1f;
    public const float UseGrenadeTime = 1f;
    private Animator _animator;
    private void Start()
    {
        TryGetAnimator();
    }
    private void TryGetAnimator()
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
    }
    public void SetAnimatorSpeed(float speed)
    {
        if (speed > 0)
        {
            TryGetAnimator();
            _animator.speed = speed;
        }
    }
    /// <summary>
    /// Judge whether to play the walk animation
    /// </summary>
    /// <param name="originalPosition"></param>
    /// <param name="newPosition"></param>
    public void WalkAnimationPlayer(Vector3 originalPosition, Vector3 newPosition)
    {
        TryGetAnimator();


        if (Vector3.Distance(originalPosition, newPosition) > MinWalkDistance * Record.RecordInfo.FrameTime)
            _animator.SetBool("IsWalking", true);
        else
            _animator.SetBool("IsWalking", false);
    }
    /// <summary>
    /// Play the dead animation
    /// </summary>
    private void SetNotDead()
    {
        TryGetAnimator();

        _animator.SetBool("IsDead", false);
    }
    public IEnumerator DeadAnimationPlayer()
    {
        TryGetAnimator();

        _animator.SetBool("IsDead", true);
        yield return new WaitForSeconds(DeadTime);

        SetNotDead();
    }
    private void SetAttackingFalse()
    {
        TryGetAnimator();

        _animator.SetBool("IsAttacking", false);
    }

    public void AttackAnimationPlayer(bool isContinuous, bool isAttackStart)
    {
        TryGetAnimator();
        if (isAttackStart)
            _animator.SetBool("IsAttacking", true);
        else
            _animator.SetBool("IsAttacking", false);

        if (isContinuous != true)
        {
            Invoke(nameof(SetAttackingFalse), AttackTime);
        }
    }

    private void SetNotUseMedicine()
    {
        TryGetAnimator();
        
        _animator.SetBool("IsUsingMedicine", false);
    }

    public void UseMedicineAnimationPlayer()
    {
        TryGetAnimator();

        _animator.SetBool("IsUsingMedicine", true);
        Invoke(nameof(SetNotUseMedicine), UseMedicineTime);
    }
}
