using System;
using UnityEngine;

public class NPC_Animator : MonoBehaviour
{
    enum AnimationComponent { Animator, Animation }

    [SerializeField]
    AnimationComponent componentType;
    [SerializeField]
    AnimationClip idleClip;
    [SerializeField]
    AnimationClip walkClip;
    [SerializeField]
    string speedParam;

    Animator animator;
    new Animation animation;

    Action idle;
    Action walk;

    void Awake()
    {
        switch (componentType)
        {
            case AnimationComponent.Animator:
                animator = GetComponent<Animator>();
                idle = Idle_Animator;
                walk = Walk_Animator;
                break;
            case AnimationComponent.Animation:
                animation = GetComponent<Animation>();
                idle = Idle_Animation;
                walk = Walk_Animation;
                break;
            default: break;
        }
    }

    public void Idle()
    {
        idle();
    }

    public void Walk()
    {
        walk();
    }

    void Idle_Animator()
    {
        animator.SetInteger(speedParam, 0);
    }

    void Walk_Animator()
    {
        animator.SetInteger(speedParam, 4);
    }

    void Idle_Animation()
    {
        animation.clip = idleClip;
        animation.Play();
    }

    void Walk_Animation()
    {
        animation.clip = walkClip;
        animation.Play();
    }
}