using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class PlayerControllerBase : MonoBehaviour
{
    protected int currentHealth;
    protected float speed;
    protected float attackRange;
    protected float jumpForce;
    protected float rollSpeed;
    protected float rollDuration;
    protected int attackDamage;
    protected float rollForce;

    protected Sensor_HeroKnight groundSensor;
    protected GameObject slideDust;
    protected float rollDistance;
    protected GameObject playerCorpsePrefab;

    public bool mDied = false;

    protected Animator animator;
    protected Rigidbody2D body2d;

    protected bool isWallSliding = false;
    protected bool grounded = false;
    public bool rolling = false;
    protected bool blocking = false;
    protected int facingDirection = 1;
    protected int nowFacingDirection = 1;
    protected int currentAttack = 0;
    protected float timeSinceAttack = 0.0f;
    protected float timeSinceRoll = 0.0f;
    protected float delayToIdle = 0.0f;
    protected float rollCurrentTime;

    protected Collider2D characterCollider;
    protected List<Collider2D> ignoredColliders = new List<Collider2D>();

    protected virtual void Start()
    {
        InitializeComponents();
    }

    protected virtual void Update()
    {
        if (mDied)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                StartCoroutine(RestartPlayer());
            }
            return;
        }

        HandleTimers();
        CheckGroundStatus();
        HandleInput();
        HandleAnimations();
    }

    protected virtual void InitializeComponents()
    {
        animator = GetComponent<Animator>();
        body2d = GetComponent<Rigidbody2D>();
        characterCollider = GetComponent<Collider2D>();
        groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
    }

    protected virtual void HandleTimers()
    {
        timeSinceAttack += Time.deltaTime;
        timeSinceRoll += Time.deltaTime;

        if (rolling)
            rollCurrentTime += Time.deltaTime;
    }

    protected virtual void CheckGroundStatus()
    {
        bool sensorState = groundSensor.State();

        if (!grounded && sensorState)
        {
            grounded = true;
            animator.SetBool("Grounded", grounded);
        }

        if (grounded && !sensorState)
        {
            grounded = false;
            animator.SetBool("Grounded", grounded);
        }
    }

    protected virtual void HandleInput()
    {
        float inputX = Input.GetAxis("Horizontal");

        UpdateDirection(inputX);

        if (!rolling)
            body2d.velocity = new Vector2(inputX * speed, body2d.velocity.y);

        if (Input.GetKeyDown("e") && !rolling)
            TriggerDeath();

        if (Input.GetKeyDown("q") && !rolling)
            TriggerHurt();

        if (Input.GetMouseButtonDown(0) && timeSinceAttack > 0.25f && !rolling)
            HandleAttack();

        if (Input.GetMouseButtonDown(1) && !rolling)
            StartBlocking();

        if (Input.GetMouseButtonUp(1))
            StopBlocking();

        if (Input.GetKeyDown(KeyCode.LeftShift) && !rolling && !isWallSliding && timeSinceRoll > 0.8f)
            StartCoroutine(Roll());

        if (Input.GetKeyDown("space") && grounded && !rolling)
            Jump();
    }

    protected virtual void UpdateDirection(float inputX)
    {
        if (inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            facingDirection = 1;
        }
        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            facingDirection = -1;
        }
    }

    protected virtual void TriggerDeath()
    {
        // Add death logic here
    }

    protected virtual void TriggerHurt()
    {
        animator.SetTrigger("Hurt");
    }

    protected virtual void HandleAttack()
    {
        currentAttack++;

        if (currentAttack > 3)
            currentAttack = 1;

        if (timeSinceAttack > 1.0f)
            currentAttack = 1;

        animator.SetTrigger("Attack" + currentAttack);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                enemy.GetComponent<EnemyController>().TakeDamage(attackDamage);
            }
        }

        timeSinceAttack = 0.0f;
    }

    protected virtual void StartBlocking()
    {
        blocking = true;
        nowFacingDirection = facingDirection;
        animator.SetTrigger("Block");
        animator.SetBool("IdleBlock", true);
    }

    protected virtual void StopBlocking()
    {
        blocking = false;
        animator.SetBool("IdleBlock", false);
    }

    protected virtual void Jump()
    {
        animator.SetTrigger("Jump");
        grounded = false;
        animator.SetBool("Grounded", grounded);
        body2d.velocity = new Vector2(body2d.velocity.x, jumpForce);
        groundSensor.Disable(0.2f);
    }

    protected virtual void HandleAnimations()
    {
        animator.SetFloat("AirSpeedY", body2d.velocity.y);

        float inputX = Input.GetAxis("Horizontal");

        if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            delayToIdle = 0.05f;
            animator.SetInteger("AnimState", 1);
        }
        else
        {
            delayToIdle -= Time.deltaTime;
            if (delayToIdle < 0)
                animator.SetInteger("AnimState", 0);
        }
    }

    protected virtual IEnumerator Roll()
    {
        AE_SlideDust();
        animator.SetTrigger("Roll");
        rolling = true;

        IgnoreEnemyCollisions(true);

        Vector2 rollDirection = new Vector2(facingDirection, 0).normalized;
        Vector2 rollTarget = new Vector2(transform.position.x + rollDirection.x * rollDistance, transform.position.y);

        float rollTime = 0.1f;
        float elapsedTime = 0f;

        while (elapsedTime < rollTime)
        {
            Vector2 newPosition = Vector2.Lerp(transform.position, rollTarget, elapsedTime / rollTime);
            body2d.MovePosition(newPosition);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rolling = false;
        IgnoreEnemyCollisions(false);
        timeSinceRoll = 0.0f;
    }

    protected virtual void IgnoreEnemyCollisions(bool ignore)
    {
        Collider2D[] enemies = GameObject.FindObjectsOfType<Collider2D>();

        foreach (Collider2D enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Physics2D.IgnoreCollision(characterCollider, enemy, ignore);
            }
        }
    }

    public virtual void TakeDamage(int damage, int facingDirectionEnemy)
    {
        if ((blocking && facingDirectionEnemy != facingDirection) || rolling)
        {
            if (blocking) { animator.SetTrigger("Block"); }
            return;
        }

        animator.SetTrigger("Hurt");
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            animator.SetTrigger("Death");
            gameObject.tag = "Untagged";
            mDied = true;
        }
    }

    public bool IsDied()
    {
        return mDied;
    }

    protected virtual void SpawnCorpse()
    {
        if (playerCorpsePrefab != null)
        {
            Instantiate(playerCorpsePrefab, transform.position, transform.rotation);
        }
    }

    protected virtual IEnumerator RestartPlayer()
    {
        yield return new WaitForSeconds(0.05f);
        SpawnCorpse();

        transform.position = Vector3.zero;
        currentHealth = 100;
        mDied = false;
        gameObject.tag = "Player";
    }

    // Animation Events
    protected virtual void AE_SlideDust()
    {
        Vector3 spawnPosition = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);

        if (slideDust != null)
        {
            GameObject dust = Instantiate(slideDust, spawnPosition, gameObject.transform.localRotation);
            dust.transform.localScale = new Vector3(facingDirection, 1, 1);
        }
    }
}
