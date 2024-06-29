using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGunScript : MonoBehaviour
{
    public int magazineSize = 15;
    public int currentMag = 0;
    public PlayerStatsScript playerStatsScript;

    public Transform gunPoint;
    public Transform directionPoint;
    public LayerMask targetMask;
    public GunShotLineEffect lineEffect;
    public ParticleSystem blood;
    public float maxRayCastDistance = 100f;
    public float weaponDamage = 100f;
    public PlayerMovementScript playerMovementScript;
    public pauseManagerScript pauseManager;
    public bool canShoot = true;
    public Animator pistolAnimator;
    public PlayerAimScript playerAimScript;
    public bool reload = false;
    public bool reloadDone = false;


    private void Start()
    {
        currentMag = magazineSize;
    }


    private void Update()
    {
        if (pauseManager.InventoryOpen == false)
        {
            if (playerMovementScript.PlayerSprint == false)
            {
                pistolGun();
                pistolAnimator.SetBool("CanShoot", canShoot);
                pistolAnimator.SetBool("Reload", reload);
            }
        }

        if (reloadDone)
        {
            
            int excess = magazineSize - currentMag;
            ItemData HGAmmo = playerStatsScript.findItemData("Handgun Ammo", true);
            if (excess > HGAmmo.amount)
            {
                currentMag += HGAmmo.amount;
                HGAmmo.amount = 0;
            }
            else
            {
                currentMag = magazineSize;
                HGAmmo.amount -= excess;
            }
            playerAimScript.stopAim = false;
            reloadDone = false;
        }
        
    }

    private void pistolGun()
    {
        Vector3 rayCastDirection = (gunPoint.position - directionPoint.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(gunPoint.position, rayCastDirection, Mathf.Infinity, targetMask);


        if (Input.GetMouseButtonDown(0) && canShoot == true && reload == false && playerAimScript.stopAim == false)
        {
            if(currentMag == 0 && playerStatsScript.findItemData("Handgun Ammo", true).amount > 0)
            {
                reload = true;
                playerAimScript.stopAim = true;
            }
            else if(currentMag > 0)
            {
                playerMovementScript.audioManager.Play("Shot");

                currentMag -= 1;
                canShoot = false;

                if (hit.collider != null)
                {
                    lineEffect.gunShot(gunPoint.position, hit.point);
                }
                else
                {
                    lineEffect.gunShot(gunPoint.position, (Vector2)gunPoint.position + (Vector2)rayCastDirection * maxRayCastDistance);
                }

                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        var bloodPosition = blood.transform.position;
                        bloodPosition.x = hit.point.x;
                        bloodPosition.y = hit.point.y;
                        blood.transform.position = bloodPosition;

                        var shape = blood.shape;
                        var rotate = shape.rotation;
                        if (blood.transform.position.x < transform.position.x)
                        {
                            rotate.y = 90;
                        }
                        else
                        {
                            rotate.y = -90;
                        }
                        shape.rotation = rotate;
                        blood.Play();

                        EnemyStatsScript script = hit.collider.GetComponentInParent<EnemyStatsScript>();
                        script.DealDamageToEnemy(hit.collider, weaponDamage);

                    }
                }
            }
            

        }

        //reload

        if(Input.GetKeyDown(KeyCode.R) && reload == false && playerStatsScript.findItemData("Handgun Ammo", true).amount > 0 && currentMag < magazineSize)
        {
            StartCoroutine(delayReloadAudio());
            reload = true;
            playerAimScript.stopAim = true;
        }


    }

    private IEnumerator delayReloadAudio()
    {
        yield return new WaitForSeconds(1f);
        playerMovementScript.audioManager.Play("Reload");
    }


}
