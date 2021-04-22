using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : MonoBehaviour
{
    [Tooltip("If true, Npc will periodically play random animation variations " +
        "if any are set up in the AnimatorController.")]
    [SerializeField]
    private bool useIdleFlourishAnimations = true;

    [Tooltip("Npc waits this long before 'rolling' a random chance to play idle flourishes.")]
    [SerializeField]
    private float minTimeBetweenIdleAnimationChangeRolls = 6, maxTimeBetweenIdleAnimationChangeRolls = 12;

    [Tooltip("We switch the parent gameobject's animator component to use this when the game starts.")]
    [SerializeField]
    private RuntimeAnimatorController animatorController;

    private Animator animator;
    private int cycleOffsetValueAnimParam = Animator.StringToHash("CycleOffsetValue");

    public float MinTimeBetweenIdleAnimationChangeRolls => minTimeBetweenIdleAnimationChangeRolls;
    public float MaxTimeBetweenIdleAnimationChangeRolls => maxTimeBetweenIdleAnimationChangeRolls;
    public bool UseIdleAnimationFlourishes => useIdleFlourishAnimations;
    // Start is called before the first frame update
    void Start()
    {           
        animator = GetComponentInParent<Animator>();
        if (animator != null)
        {
            animator.runtimeAnimatorController = animatorController;

            if (animator.parameterCount == 0)
                useIdleFlourishAnimations = false;

            animator.SetFloat(cycleOffsetValueAnimParam, UnityEngine.Random.value);
        }
    }
}
