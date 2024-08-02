using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] public float attackRange = 1.5f;
    [SerializeField] public float attackCooldown = 1f;
    [SerializeField] public int attackDamage = 10;
    [SerializeField] public int maxHealth = 200;

    [SerializeField] public float dashDistance = 5f;
    [SerializeField] public float dashCooldown = 10f;
    [SerializeField] public float jumpHeight = 1.5f;
    [SerializeField] public float jumpCooldown = 6f;
    [SerializeField] public int specialAttackDamage = 20;

    public int facingDirection = 1;

    private int currentHealth;
    private float attackCooldownTimer = 0f;
    private float dashCooldownTimer = 0f;
    private float jumpCooldownTimer = 10f;
    private Transform playerTransform;
    private Animator anim;
    private PlayerControllerBase playerObj;
    private bool isAttacking = false;
    private bool isDashing = false;
    private bool isJumping = false;
    private bool isDead = false;

    void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerBase>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void FixedUpdate()
    {
        if (isDead)
        {
            return; // Прекращаем выполнение если босс мертв
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (CheckForAlivePlayer())
        {
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

                // Логика для дэш-атаки
                if (dashCooldownTimer <= 0)
                {
                    StartCoroutine(Dash());
                }

                // Логика для прыжка
                if (jumpCooldownTimer <= 0)
                {
                    StartCoroutine(Jump());
                }
            }

            if (attackCooldownTimer > 0)
            {
                attackCooldownTimer -= Time.deltaTime;
            }

            if (dashCooldownTimer > 0)
            {
                dashCooldownTimer -= Time.deltaTime;
            }

            if (jumpCooldownTimer > 0)
            {
                jumpCooldownTimer -= Time.deltaTime;
            }
        }
        else
        {
            anim.SetBool("isRunning", false);
            anim.SetBool("isAttacking", false);
           // anim.SetBool("Idle", true);
        }
    }

    bool CheckForAlivePlayer()
    {
        return GameObject.FindGameObjectWithTag("Player") != null;
    }

    void FollowPlayer()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);

        // Поворот босса в сторону игрока
        if (direction.x > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            facingDirection = 1;
        }
        else if (direction.x < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            facingDirection = -1;
        }

        anim.SetBool("isRunning", true);
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        anim.SetBool("isRunning", false);
        anim.SetTrigger("isAttacking");
        attackCooldownTimer = attackCooldown;

        if (Vector2.Distance(transform.position, playerTransform.position) <= attackRange && playerObj.rolling == false)
        {
            playerTransform.GetComponent<PlayerControllerBase>().TakeDamage(attackDamage, facingDirection);
        }

        yield return new WaitForSeconds(attackCooldown - 0.5f); // Оставшаяся часть времени отката атаки

        isAttacking = false;
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        anim.SetBool("isDashing", true);
        dashCooldownTimer = dashCooldown;

        Vector2 dashDirection = (playerTransform.position - transform.position).normalized;
        Vector2 dashTarget = new Vector2(transform.position.x + dashDirection.x * dashDistance, transform.position.y);

        float dashTime = 0.3f; // Время выполнения дэша
        float elapsedTime = 0f;

        while (elapsedTime < dashTime)
        {
            transform.position = Vector2.Lerp(transform.position, dashTarget, elapsedTime / dashTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
        anim.SetBool("isDashing", false);
    }

    private IEnumerator Jump()
    {
        isJumping = true;
        anim.SetTrigger("isJumping");
        jumpCooldownTimer = jumpCooldown;

        Vector2 jumpTarget = new Vector2(transform.position.x + dashDistance * facingDirection, transform.position.y + jumpHeight);
        float jumpTime = 0.3f; // Время выполнения прыжка
        float elapsedTime = 0f;

        while (elapsedTime < jumpTime)
        {
            transform.position = Vector2.Lerp(transform.position, jumpTarget, elapsedTime / jumpTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }


        // Спуск вниз
        elapsedTime = 0f;
        /*while (elapsedTime < jumpTime)
        {
            transform.position = Vector2.Lerp(jumpTarget, transform.position, elapsedTime / jumpTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }*/

        isJumping = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // Если босс мертв, он не получает урон

        Debug.Log("Boss is hit!");
        anim.SetTrigger("isHit");
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Логика смерти босса
        anim.SetTrigger("isDead");
        Debug.Log("Boss died!");
        isDead = true;


    }


}
