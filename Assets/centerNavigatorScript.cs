using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class centerNavigatorScript : MonoBehaviour
{
    public bool canChange = true;
    public Animator animator;
    public void canChangeNavigation()
    {
        canChange = true;
    }

    public void changeImmediateTransport()
    {
        animator.SetBool("ImmediateTransport", false);
    }
}
