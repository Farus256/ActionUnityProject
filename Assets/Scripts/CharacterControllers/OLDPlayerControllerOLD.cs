using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerOLD : MonoBehaviour
{
    private Animator anim;
    public float moveSpeed = 5f;
    public float dashSpeed = 20f; // Скорость Dash
    public float dashDuration = 0.3f; // Длительность Dash
    public float dashCooldown = 1.5f; // Откат Dash
    private bool isDashing = false;

    private float dashCooldownTimer = 0f;

    private Vector2 moveInput;
    private Vector2 moveVelocity;
    public float attackDuration = 0.3f; // Длительность анимации удара
    private bool isAttacking = false;
    private float attackTimer = 10f;
    private int attackCount = 0;

    private float lastWPressTime = -1f;
    private float lastSPressTime = -1f;
    private float lastAPressTime = -1f;
    private float lastDPressTime = -1f;
    public float doubleTapTime = 0.3f; // Время для двойного нажатия

    public int maxHealth = 100;
    private int currentHealth;
    public int attackDamage = 10;
    public float attackRange = 1.5f; // Дальность атаки
    public float attackCooldown = 1f; // Откат атаки

    private float attackCooldownTimer = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        // Движение игрока
        moveInput = new Vector2(Input.GetAxis("Horizontal"), 0);
        moveVelocity = moveInput.normalized * moveSpeed;

        // Логика для поворота персонажа
        if (moveInput.x < 0)
        {
            transform.localScale = new Vector3(-5, 5, 5); // Поворот влево
        }
        else if (moveInput.x > 0)
        {
            transform.localScale = new Vector3(5, 5, 5); // Поворот вправо
        }

        // Анимация движения
        if (moveInput != Vector2.zero)
        {
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }

        // Проверка двойного нажатия для Dash
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (Time.time - lastWPressTime < doubleTapTime && dashCooldownTimer <= 0)
            {
                StartCoroutine(Dash(Vector2.up));
            }
            lastWPressTime = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (Time.time - lastSPressTime < doubleTapTime && dashCooldownTimer <= 0)
            {
                StartCoroutine(Dash(Vector2.down));
            }
            lastSPressTime = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            if (Time.time - lastAPressTime < doubleTapTime && dashCooldownTimer <= 0)
            {
                StartCoroutine(Dash(Vector2.left));
            }
            lastAPressTime = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            if (Time.time - lastDPressTime < doubleTapTime && dashCooldownTimer <= 0)
            {
                StartCoroutine(Dash(Vector2.right));
            }
            lastDPressTime = Time.time;
        }

        if (Input.GetMouseButtonDown(0))
        {
            attackCount++;
            if (attackCount >= 2)
            {
                attackTimer = attackDuration;
                anim.SetBool("isCombo", true);
                Attack();
            }
        }

        // Атака
        if (Input.GetMouseButtonDown(0) && !isAttacking && attackCount < 2) // Левая кнопка мыши
        {
            isAttacking = true;
            attackTimer = attackDuration;
            anim.SetBool("isAttacking", true);
            Attack();
        }

        if ((Input.GetMouseButtonDown(1) && !isAttacking))
        {
            isAttacking = true;
            attackTimer = attackDuration;
            anim.SetBool("isAttacking2", true);
            Attack();
        }

        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0)
            {
                isAttacking = false;
                anim.SetBool("isAttacking", false);
                anim.SetBool("isAttacking2", false);
                anim.SetBool("isCombo", false);
                attackCount = 0;
            }
        }

        // Обновление таймера отката Dash
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if (!isAttacking && !isDashing) // Перемещение только если не атакуем и не в состоянии Dash
        {
            transform.Translate(moveVelocity * Time.fixedDeltaTime);
        }
    }

    private IEnumerator Dash(Vector2 direction)
    {
        isDashing = true;
        anim.SetBool("isDashing", true);
        dashCooldownTimer = dashCooldown;
        float startTime = Time.time;

        while (Time.time < startTime + dashDuration)
        {
            transform.Translate(direction * dashSpeed * Time.deltaTime);
            yield return null;
        }

        isDashing = false;
        anim.SetBool("isDashing", false);
    }

    private void Attack()
    {
        if (attackCooldownTimer <= 0)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange);

            foreach (Collider2D enemy in hitEnemies)
            {
                if (enemy.CompareTag("Enemy"))
                {
                    enemy.GetComponent<EnemyController>().TakeDamage(attackDamage);
                }
            }

            attackCooldownTimer = 0;
        }
    }

    public void TakeDamage(int damage)
    {
        anim.SetTrigger("isHit");
        Debug.Log("меня хуярят!");
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Логика смерти игрока
        Debug.Log("Player died!");
        // Вы можете добавить анимацию смерти, перезапуск уровня и т.д.
    }
}
