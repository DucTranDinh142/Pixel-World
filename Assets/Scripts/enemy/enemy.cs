using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{

    public Transform player;
    public float detectionRange = 6f;
    public float moveSpeed = 2f;
    public float jumpForce = 7f;
    public float checkDistance = 0.1f;
    public GameObject groundCheck;

    public GameObject frontCheck;
    public float obstacleCheckRadius = 0.2f;
    public LayerMask groundLayer;

    public int damage = 10;
    public float attackCooldown = 1f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float idleTime;
    private Vector2 randomTarget;
    private Animator animator;
    private float lastAttackTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        CheckIfGrounded();
        //Debug.Log(isGrounded);

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            FollowPlayer();
        }
        if (Mathf.Abs(rb.velocity.x) > 0.1f)
        {
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
        Vector3 scale = transform.localScale;

        if (player.position.x < transform.position.x)
        {
            scale.x = Mathf.Abs(scale.x);
        }
        else
        {
            scale.x = -Mathf.Abs(scale.x);
        }

        transform.localScale = scale;
    }

    void CheckIfGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, checkDistance, groundLayer);
    }

    void FollowPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(dir.x * moveSpeed, rb.velocity.y);

        CheckAndJumpObstacle();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.green;
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.transform.position, checkDistance);
    }
    void CheckAndJumpObstacle()
    {
        Collider2D hit = Physics2D.OverlapCircle(frontCheck.transform.position, obstacleCheckRadius, groundLayer);
        if (hit != null && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                PlayerHealth ph = collision.gameObject.GetComponent<PlayerHealth>();
                if (ph != null)
                {
                    ph.TakeDamage(damage);
                    lastAttackTime = Time.time;
                }
            }
        }
    }
}
