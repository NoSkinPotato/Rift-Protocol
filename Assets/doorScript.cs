using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorScript : MonoBehaviour
{
    public Animator animator;
    public void openDoor()
    {
        animator.SetTrigger("Open");
    }

    public void deactivateDoor()
    {
        gameObject.SetActive(false);
    }

}
