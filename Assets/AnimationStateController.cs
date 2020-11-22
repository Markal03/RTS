using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetWalking(bool isWalking)
    {
        //If unit is not walking and we want to start walking
        if ((!animator.GetBool("IsWalking") && isWalking)
            ||
        //OR if unit is walking and we want to stop
            (animator.GetBool("IsWalking") && !isWalking))
        {
            animator.SetBool("IsWalking", isWalking);
        }
    }

    public void SetAttacking(bool isAttacking)
    {
        //If unit is not attacking and we want to start attacking
        if ((!animator.GetBool("IsAttacking") && isAttacking)
            ||
        //OR if unit is attacking and we want to stop
            (animator.GetBool("IsAttacking") && !isAttacking))
        {
            animator.SetBool("IsAttacking", isAttacking);
        }
    }

    public void SetDying(bool isDying)
    {
        animator.SetBool("IsDying", isDying);
    }

}
