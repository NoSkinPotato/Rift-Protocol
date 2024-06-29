using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private PlayerStatsScript playerStatsScript;
    public EnemyStatsScript enemyStatsScript;
    private inventoryManagement management;
    private Transform player;
    public bool isFacingLeft = true;
    public Rigidbody2D rb;
    public float walkSpeed = 2f;
    public float runSpeed = 6f;
    public Animator animator;
    public float viewDistance = 15f;
    public float awarenessDistance = 4f;
    public float timeBeforeChange = 5f;
    float movementTimer = 0;
    public float attackSpeed = 0.5f;
    float attackTimer = 0;
    public bool shot = false;
    public bool agro = false;
    bool justAgro = false;
    public float attackDistance = 2f;
    public SpiderDamageEffects effects;
    private bool dead = false;
    public LayerMask playerGroundMask;
    public LayerMask groundMask;
    public bigBoySpecific bigboy;
    public Audio[] spiderAudio;

    private void Awake()
    {
        foreach (Audio audi in spiderAudio)
        {
            audi.source = gameObject.AddComponent<AudioSource>();

            audi.source.loop = audi.loop;
            audi.source.volume = audi.volume;
            audi.source.pitch = audi.pitch;
            audi.source.clip = audi.audioClip;

        }
    }

    private void Play(string name)
    {

        Audio a = System.Array.Find(spiderAudio, audi => name == audi.audioName);

        if (a == null)
        {
            Debug.Log(name + " Not Found");
            return;
        }

        a.source.Play();

    }


    private void Start()
    {
        playerStatsScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatsScript>();
        player = playerStatsScript.GetComponent<PlayerMovementScript>().lowerBody.transform;
        enemyStatsScript = GetComponent<EnemyStatsScript>();
        management = FindObjectOfType<inventoryManagement>();
        attackTimer = attackSpeed;

        findDirectionAndTime(2);
    }

    

    private void Update()
    {
        if (effects.enemyHealth <= 0 && dead == false)
        {
            Dead();

            return;
        }

        if(dead == false)
        {
            if (isFacingLeft == true && rb.velocity.x > 0)
            {
                Vector3 scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
                isFacingLeft = false;
            }
            else if (isFacingLeft == false && rb.velocity.x < 0)
            {
                Vector3 scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
                isFacingLeft = true;
            }

        }

    }

    private void FixedUpdate()
    {
        if(dead == false)
        {
            if (Vector2.Distance(transform.position, player.position) <= attackDistance)
            {
                animator.SetBool("Attacking", true);
                rb.velocity = Vector2.zero;

                if (attackTimer >= attackSpeed)
                {
                    animator.SetBool("Attack", true);
                    attackTimer = 0;
                }
                else
                {
                    attackTimer += Time.fixedDeltaTime;
                }


            }
            else
            {
                viewCones();
                animator.SetBool("Attacking", false);

                if(attackTimer < attackSpeed)
                {
                    attackTimer += Time.fixedDeltaTime;
                }
            }
        }


    }

    public void deliverAttack()
    {
        animator.SetBool("Attack", false);
        playerStatsScript.damagePlayer(effects.spiderDamage);
    }


    private void viewCones()
    {
        if(shot == true || justAgro == true)
        {
            Agro();
            return;
        }


        if (notOnWall())
        {
            float distance = Vector2.Distance(transform.position, player.position);

            if (distance <= viewDistance && distance >= 0 && distance > awarenessDistance)
            {
                if ((isFacingLeft == false && transform.position.x < player.position.x) || (isFacingLeft && transform.position.x > player.position.x))
                {

                    Agro();
                    return;


                }
            }
            else if (distance <= awarenessDistance)
            {
                Agro();
                return;
            }
        }

        


        RandomMovements();

    }


    private bool notOnWall()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, Mathf.Infinity, playerGroundMask);
        if (hit.collider != null && hit.collider.gameObject.CompareTag("Player"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    

    private void Agro()
    {

        if(justAgro == false)
        {
            Play("Agro");
            justAgro = true;
        }

        animator.SetBool("Run", true);
        animator.SetBool("Crawl", false);
        Vector2 direction = (player.position - transform.position).normalized;



        if(direction.y > direction.x)
        {
            direction = new Vector2(direction.x, 0f);

        }

        rb.velocity = direction * runSpeed;

    }

    private void RandomMovements()
    {
        agro = false;
        animator.SetBool("Crawl", true);
        animator.SetBool("Run", false);
        if (movementTimer < timeBeforeChange)
        {
            movementTimer += Time.deltaTime;
            stuckOnWall();
        }
        else
        {
            findDirectionAndTime(2);

        }

    }

    private void stuckOnWall()
    {

        


        if(transform.localScale.x > 0)
        {
            //facingLeft
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, attackDistance, groundMask);
            

            if (hit.collider != null)
            {
                findDirectionAndTime(0);
            }


        }
        else
        {
            //facingRight

            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, attackDistance, groundMask);
            
            if (hit.collider != null)
            {
                findDirectionAndTime(1);
            }
        }

    }

    private void OnDrawGizmos()
    {
      
    }

    private void findDirectionAndTime(int x)
    {
        if(x == 2)
        {
            x = Random.Range(0, 2);
        }

        if (x == 1)
        {
            rb.velocity = new Vector2(-walkSpeed, 0);
        }
        else
        {
            rb.velocity = new Vector2(walkSpeed, 0);
        }
        timeBeforeChange = Random.Range(5f, 10f);
        movementTimer = 0;
    }

    private void Dead()
    {

        if(bigboy != null)
        {
            bigboy.victoryScreen();
        }

        Play("Dead");
        animator.SetBool("Dead", true);
        rb.velocity = Vector2.zero;
        rb.drag = 50f;

        dead = true;

        foreach (Collider2D col in enemyStatsScript.colliders)
        {
            col.enabled = false;
        }

        management.dropItem(transform.position);
    }

}
