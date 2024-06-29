using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lootEffectsScript : MonoBehaviour
{

    public Animator animator;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
           
            animator.SetBool("onGround", true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            animator.SetBool("onGround", false);
        }
    }
}
