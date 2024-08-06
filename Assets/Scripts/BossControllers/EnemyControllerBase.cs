using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class EnemyControllerBase : MonoBehaviour
{
    protected int currentHealth;
    protected float moveSpeed;
    protected float attackRange;
    protected float attackCooldown;
    protected int attackDamage;
    protected float dashDistance;
    protected float dashCooldown;
    protected float jumpHeight;
    protected float jumpCooldown;
    protected int specialAttackDamage;

    protected bool isAttacking = false;
    protected bool isDashing = false;
    protected bool isJumping = false;
    protected bool isDead = false;

    protected float attackCooldownTimer = 0f;
    protected float dashCooldownTimer = 0f;
    protected float jumpCooldownTimer = 0f;

    protected Transform playerTransform;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;

    protected int facingDirection = 1;

    protected virtual void Start()
    {
        InitializeComponents();
    }

    protected virtual void Update()
    {
        if (isDead) return;

        HandleTimers();
        if (CheckForAlivePlayer())
        {
            HandleEnemyActions();
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }

    protected virtual void InitializeComponents()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = 200; // Example value, this can be set differently in derived classes
    }

    protected virtual void HandleTimers()
    {
        if (attackCooldownTimer > 0) attackCooldownTimer -= Time.deltaTime;
        if (dashCooldownTimer > 0) dashCooldownTimer -= Time.deltaTime;
        if (jumpCooldownTimer > 0) jumpCooldownTimer -= Time.deltaTime;
    }

    protected virtual bool CheckForAlivePlayer()
    {
        return playerTransform != null;
    }

    protected virtual void HandleEnemyActions()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (!isAttacking && !isDashing && !isJumping)
        {
            if (distanceToPlayer > attackRange)
            {
                FollowPlayer();
            }
            else
            {
                StartCoroutine(Attack());
            }

            if (dashCooldownTimer <= 0)
            {
                StartCoroutine(Dash());
            }

            if (jumpCooldownTimer <= 0)
            {
                StartCoroutine(Jump());
            }
        }
    }

    protected virtual void FollowPlayer()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);

        UpdateDirection(direction.x);
        animator.SetBool("isRunning", true);
    }

    protected virtual void UpdateDirection(float directionX)
    {
        if (directionX > 0)
        {
            spriteRenderer.flipX = false;
            facingDirection = 1;
        }
        else if (directionX < 0)
        {
            spriteRenderer.flipX = true;
            facingDirection = -1;
        }
    }

    protected abstract IEnumerator Attack();

    protected abstract IEnumerator Dash();

    protected abstract IEnumerator Jump();

    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;

        Debug.Log("Enemy is hit!");
        animator.SetTrigger("isHit");
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        animator.SetTrigger("isDead");
        isDead = true;
        Debug.Log("Enemy died!");
    }
}
