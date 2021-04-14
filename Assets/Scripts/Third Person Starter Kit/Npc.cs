using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : MonoBehaviour
{
    [Tooltip("Npc waits this long before 'rolling' a random chance to change idle animations.")]
    [SerializeField]
    private float minTimeBetweenIdleAnimationChangeRolls = 2, maxTimeBetweenIdleAnimationChangeRolls = 5;

    [Tooltip("We switch the parent gameobject's animator component to use this when the game starts.")]
    [SerializeField]
    private RuntimeAnimatorController animatorController;

    private Animator animator;
    private int cycleOffsetValueAnimParam = Animator.StringToHash("CycleOffsetValue");
    private int randomAnimationValueAnimParam = Animator.StringToHash("RandomAnimationValue");
    private float cycleOffsetValue;
    private bool shouldUpdateIdleAnimations = true;
    private int maxRoll, minRoll;
    private float timeBetweenIdleAnimationChangeRolls;

    // Start is called before the first frame update
    void Start()
    {
        timeBetweenIdleAnimationChangeRolls = 
            UnityEngine.Random.Range(minTimeBetweenIdleAnimationChangeRolls, maxTimeBetweenIdleAnimationChangeRolls);
        animator = GetComponentInParent<Animator>();
        animator.runtimeAnimatorController = animatorController;
        animator.SetFloat(cycleOffsetValueAnimParam, UnityEngine.Random.value);
        StartCoroutine(ChangeIdleAnimationCoroutine());
    }

    /// <summary>
    /// Waits some time, then randomly decides on a new animation to play (or not).
    /// The goal is to have NPCs not all play the same animations and not feel too uniform or static.
    /// </summary>
    /// <returns></returns>
   private IEnumerator ChangeIdleAnimationCoroutine()
    {
        while (shouldUpdateIdleAnimations)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                yield return new WaitForSeconds(timeBetweenIdleAnimationChangeRolls);
                animator.SetInteger(randomAnimationValueAnimParam, MakeAnimationChangeRoll());
            }
            else
            {
                animator.SetInteger(randomAnimationValueAnimParam, 0);
                yield return null;
            }        
        }
    }

    private int MakeAnimationChangeRoll()
    {
        return UnityEngine.Random.Range(1, 11);
    }
}
