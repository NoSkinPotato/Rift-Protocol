using Cinemachine;
using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Mathematics;
using UnityEngine;

public class PlayerAimScript : MonoBehaviour
{
    public bool isFacingLeft = false;
    public Transform arm;
    public float angle;
    public float maxAngle = 30f;
    public Transform handpoint;
    public Transform jointPoint;
    public LineRenderer otherArm;
    public PlayerMovementScript playerMovement;
    public pauseManagerScript pauseManager;
    public Animator upperBodyAnim;
    public bool stopAim = false;

    void LateUpdate()
    {
        if(pauseManager.InventoryOpen == false)
        {
            if (playerMovement.PlayerSprint == false)
            {
                if (stopAim == false)
                {
                    Vector3 mousePosition = Camera.main.ScreenToWorldPoint((Vector2)Input.mousePosition);
                    
                    PlayerFaceMouse(mousePosition);


                    Vector3 direction = (jointPoint.position - mousePosition).normalized;

                    angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    upperBodyAnim.SetFloat("Angle", angle);
                    upperBodyAnim.SetBool("isFacingLeft", isFacingLeft);

                    if (isFacingLeft)
                    {
                        angle = Mathf.Clamp(angle, -maxAngle, maxAngle);
                        arm.rotation = Quaternion.Euler(0f, 0f, angle);
                    }
                    else
                    {
                        if (angle >= -(180 - maxAngle) && angle <= 180 - maxAngle)
                        {
                            if (angle < 0)
                            {
                                angle = -(180 - maxAngle);
                            }
                            else
                            {
                                angle = 180 - maxAngle;
                            }
                        }
                        arm.rotation = Quaternion.Euler(0f, 0f, angle - 180);

                    }
                }
                else
                {
                    var rotate = arm.rotation;
                    rotate.z = 0;
                    arm.rotation = rotate;
                }

                float z = 2f;

                otherArm.SetPosition(0, new Vector3(otherArm.transform.position.x, otherArm.transform.position.y, z));
                otherArm.SetPosition(1, new Vector3(handpoint.transform.position.x, handpoint.transform.position.y, z));

                
            }
            else
            {
                otherArm.enabled = false;
            }
        }
        
        

    }

    

    private void PlayerFaceMouse(Vector3 mousePosition)
    {
        if(transform.position.x < mousePosition.x)
        {
            if (isFacingLeft)
            {
                Vector3 scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
                
                isFacingLeft = false;
            }

        }
        else
        {
            if (!isFacingLeft)
            {
                Vector3 scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
                
                isFacingLeft = true;
            }

        }
    }

   
}
