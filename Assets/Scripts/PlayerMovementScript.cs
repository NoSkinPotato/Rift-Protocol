using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    public float movementSpeed = 2f;
    public float sprintSpeed = 4f;
    public Rigidbody2D rb;
    public Animator lowerBodyAnimator;
    public bool PlayerSprint = false;
    public PlayerAimScript aimScript;
    public Animator RunAnimator;
    public GameObject upperBody;
    public GameObject lowerBody;
    public float speedToMax = 1f;
    public float currentSpeed = 0;
    float savedSpeed;
    public float startSprintSpeed;
    bool startSpeed = false;
    float horizontal;
    public float speed;
    public float staminaDropRate = 2f;
    public PlayerStatsScript playerStatsScript;
    bool exhausted = false;
    public PlayerGunScript pistolScript;
    public craftingSystem crafting;

    public pauseManagerScript pauseManager;
    public Transform CameraPosWhenInventory;
    [HideInInspector]
    public audioManager audioManager;
    public float value = 2.5f;
    private float timer = 0;
    bool playAudio = false; 

    private void Start()
    {
        savedSpeed = currentSpeed;
        audioManager = FindObjectOfType<audioManager>();
        audioManager.Play("Ambience");
    }

    private void Update()
    {
        CameraPosWhenInventory.position = transform.position + Vector3.right * 5.25f;
    }

    private void FixedUpdate()
    {
        if (pauseManager.InventoryOpen == false && crafting.craftingMenuOpen == false)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            Vector2 direction = new Vector3(horizontal, 0);
            rb.velocity = direction * speed;

            stepAudioControl();

            lowerBodyAnimator.SetFloat("Horizontal", Mathf.Abs(horizontal));

            if (playerStatsScript.currentPlayerStamina <= 0)
            {
                exhausted = true;
            }else if(playerStatsScript.currentPlayerStamina > 200)
            {
                exhausted = false;
            }


            if (Input.GetKey(KeyCode.LeftShift) && Mathf.Abs(horizontal) > 0 && pistolScript.canShoot && playerStatsScript.currentPlayerStamina > 0f)
            {
                if (exhausted == true)
                {
                    if (playerStatsScript.currentPlayerStamina > 200)
                    {
                        
                        if(playAudio == true)
                        {
                            audioManager.Stop("Running");
                            playAudio = false;
                        }
                        exhausted = false;
                    }
                    else
                    {
                        if (playAudio == false)
                        {
                            audioManager.Play("Running");
                            playAudio = true;
                        }
                    }

                }
                else
                {
                    PlayerSprint = true;

                    upperBody.SetActive(false);
                    lowerBody.SetActive(false);
                    PlayerFaceSprint(horizontal);
                    RunAnimator.SetBool("Run", true);
                    if (startSpeed == false)
                    {
                        
                        speed = startSprintSpeed;
                        startSpeed = true;
                    }

                    

                    speedUp();

                }


            }
            else
            {

                if(exhausted == false && playAudio == true)
                {
                    audioManager.Stop("Running");
                    playAudio = false;

                }

                PlayerSprint = false;
                if ((horizontal < 0 && aimScript.isFacingLeft == false) || (horizontal > 0 && aimScript.isFacingLeft == true))
                {
                    speed = movementSpeed / 1.25f;
                }
                else
                {
                    speed = movementSpeed;
                }

                currentSpeed = savedSpeed;
                upperBody.SetActive(true);
                lowerBody.SetActive(true);
                RunAnimator.SetBool("Run", false);
                startSpeed = false;

            }


            if ((aimScript.isFacingLeft == true && horizontal > 0) || (aimScript.isFacingLeft == false && horizontal < 0))
            {
                lowerBodyAnimator.SetBool("RunForward", false);
            }
            else
            {
                lowerBodyAnimator.SetBool("RunForward", true);
            }
        }
    }

    private void stepAudioControl()
    {
        if (Mathf.Abs(horizontal) > 0)
        {
            if (timer < value / speed)
            {
                timer += Time.deltaTime;
            }
            else
            {
                audioManager.Play("Step");
                timer = 0;
            }

        }
        
    }

    
    private void speedUp()
    {
        if(currentSpeed < 1)
        {
            currentSpeed += Time.deltaTime;
            
        }

        RunAnimator.SetFloat("SpeedMultiplier", currentSpeed);

        if (speed < sprintSpeed)
        {
            speed += Time.deltaTime * speedToMax;
        }

        playerStatsScript.currentPlayerStamina -= Time.deltaTime * speed * staminaDropRate;

    }


    private void PlayerFaceSprint(float horizontal)
    {
        if (horizontal >= 0)
        {
            if (aimScript.isFacingLeft)
            {
                Vector3 scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
                aimScript.isFacingLeft = false;
            }
        }
        else if(horizontal < 0)
        {
            if (!aimScript.isFacingLeft)
            {
                Vector3 scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
                aimScript.isFacingLeft = true;
            }
        }
    }


}
