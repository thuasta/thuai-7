using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private Animator animator;
    private readonly string _message;
    readonly int health;
   /* enum MessageKind
    {
        PERFORM_MOVE,
        PERFORM_ATTACK,
        PERFORM_USE_MEDICINE,
        PERFORM_USE_GRENADE,
        PERFORM_SWITCH_ARM

    }*/
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_message == "PERFORM_MOVE")
        {
            animator.SetBool("Running", true);
        }
        if(_message == "PERFORM_ATTACK")
        {
            animator.SetBool("Firing", true);
        }
        if(_message == "PERFORM_USE_MEDICINE")
        {
            animator.SetBool("Drinking", true);
        }/*
        if(this.health )
        {
            animator.SetBool("Ishurt", true);
        }
        if(this.health == 0 )
        {
            animator.Set*/
    }
}
