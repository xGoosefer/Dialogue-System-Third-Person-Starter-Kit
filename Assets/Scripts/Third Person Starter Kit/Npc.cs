using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : MonoBehaviour
{
    [Tooltip("Npc waits this long before 'rolling' a random chance to change idle animations.")]
    [SerializeField]
    private float minTimeBetweenIdleAnimationChangeRolls = 3, maxTimeBetweenIdleAnimationChangeRolls = 6;

    [Tooltip("We switch the parent gameobject's animator component to use this when the game starts.")]
    [SerializeField]
    private RuntimeAnimatorController animatorController;

    private Animator animator;
    private int cycleOffsetValueAnimParam = Animator.StringToHash("CycleOffsetValue");
    private float cycleOffsetValue;
    private float timeBetweenIdleAnimationChangeRolls;

    public float MinTimeBetweenIdleAnimationChangeRolls => minTimeBetweenIdleAnimationChangeRolls;
    public float MaxTimeBetweenIdleAnimationChangeRolls => maxTimeBetweenIdleAnimationChangeRolls;

    // Start is called before the first frame update
    void Start()
    {      
        animator = GetComponentInParent<Animator>();
        animator.runtimeAnimatorController = animatorController;
        animator.SetFloat(cycleOffsetValueAnimParam, UnityEngine.Random.value);
    }

    /// <summary>
    /// Waits some time, then randomly decides on a new animation to play (or not).
    /// The goal is to have NPCs not all play the same animations and not feel too uniform or static.
    /// </summary>
    /// <returns></returns>



}
