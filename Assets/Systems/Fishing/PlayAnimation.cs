using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimation : MonoBehaviour
{
    [SerializeField] private AnimationClip animation_to_play;
    [SerializeField] private AnimationClip destruction_animation;
    private Animator animator;

    private void Awake() 
    {
        animator = GetComponent<Animator>();
        animator.Play(animation_to_play.name);
        
    }

    public void PlayDestruction()
    {
        animator.Play(destruction_animation.name);
    }
}
