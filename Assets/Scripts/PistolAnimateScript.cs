using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolAnimateScript : MonoBehaviour
{
    public PlayerGunScript gunScript;
    public Animator animator;


    public void ShootDone()
    {
        gunScript.canShoot = true;
    }
    
    public void ReloadDone()
    {
        gunScript.reload = false;
        gunScript.reloadDone = true;
    }



}
