using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [SerializeField] int currentHealth = 100;
    [SerializeField] float speed = 4.0f;
    [SerializeField] float attackRange = 1.5f;
    [SerializeField] float jumpForce = 7.5f;
    [SerializeField] float rollSpeed = 1.0f;
    [SerializeField] float rollDuration = 0.5f;
    [SerializeField] int attackDamage = 10;
    [SerializeField] float rollForce = 10.0f;
    [SerializeField] bool noBlood = false;
    [SerializeField] GameObject slideDust;
    [SerializeField] float rollDistance = 5f;

    public bool mDied = false;

    private Animator animator;
    private Rigidbody2D body2d;
    private Sensor_HeroKnight groundSensor;
    private Sensor_HeroKnight wallSensorR1;
    private Sensor_HeroKnight wallSensorR2;
    private Sensor_HeroKnight wallSensorL1;
    private Sensor_HeroKnight wallSensorL2;
    private bool isWallSliding = false;
    private bool grounded = false;
    public bool rolling = false;
    private bool blocking = false;
    private int facingDirection = 1;
    private int nowFacingDirection = 1;
    private int currentAttack = 0;
    private float timeSinceAttack = 0.0f;
    private float timeSinceRoll = 0.0f;
    private float delayToIdle = 0.0f;
    private float rollCurrentTime;

    private Collider2D characterCollider;
    private List<Collider2D> ignoredColliders = new List<Collider2D>();

    void Start()
    {
        InitializeComponents();
    }

    void Update()
    {
        if(mDied)
        {
            return;
        }
        HandleTimers();
        CheckGroundStatus();
        HandleInput();
        HandleAnimations();
    }

    void InitializeComponents()
    {
        animator = GetComponent<Animator>();
        body2d = GetComponent<Rigidbody2D>();
        characterCollider = GetComponent<Collider2D>();

        groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
    }

    void HandleTimers()
    {
        timeSinceAttack += Time.deltaTime;
        timeSinceRoll += Time.deltaTime;

        if (rolling)
            rollCurrentTime += Time.deltaTime;

    }

    void CheckGroundStatus()
    {
        if (!grounded && groundSensor.State())
        {
            grounded = true;
            animator.SetBool("Grounded", grounded);
        }

        if (grounded && !groundSensor.State())
        {
            grounded = false;
            animator.SetBool("Grounded", grounded);
        }
    }

    void HandleInput()
    {
        float inputX = Input.GetAxis("Horizontal");

        UpdateDirection(inputX);

        if (!rolling)
            body2d.velocity = new Vector2(inputX * speed, body2d.velocity.y);

        if (Input.GetKeyDown("e") && !rolling)
            TriggerDeath();

        else if (Input.GetKeyDown("q") && !rolling)
            TriggerHurt();

        else if (Input.GetMouseButtonDown(0) && timeSinceAttack > 0.25f && !rolling)
            HandleAttack();

        else if (Input.GetMouseButtonDown(1) && !rolling)
            StartBlocking();

        else if (Input.GetMouseButtonUp(1))
            StopBlocking();

        if (Input.GetKeyDown(KeyCode.LeftShift) && !rolling && !isWallSliding && timeSinceRoll > 0.8f)
            StartCoroutine(Roll());

        /*if(Input.GetKeyUp(KeyCode.LeftShift))
            stopRoll();*/

        else if (Input.GetKeyDown("space") && grounded && !rolling)
            Jump();
    }

    void UpdateDirection(float inputX)
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

    void TriggerDeath()
    {
        animator.SetBool("noBlood", noBlood);
        animator.SetTrigger("Death");
    }

    void TriggerHurt()
    {
        animator.SetTrigger("Hurt");
    }

    void HandleAttack()
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
                enemy.GetComponent<BossController>().TakeDamage(attackDamage);
            }
        }

        timeSinceAttack = 0.0f;
    }

    void StartBlocking()
    {
        blocking = true;
        nowFacingDirection = facingDirection;
        animator.SetTrigger("Block");
        animator.SetBool("IdleBlock", true);
    }

    void StopBlocking()
    {
        blocking = false;
        animator.SetBool("IdleBlock", false);
    }

    void Jump()
    {
        animator.SetTrigger("Jump");
        grounded = false;
        animator.SetBool("Grounded", grounded);
        body2d.velocity = new Vector2(body2d.velocity.x, jumpForce);
        groundSensor.Disable(0.2f);
    }

    void HandleAnimations()
    {
        animator.SetFloat("AirSpeedY", body2d.velocity.y);

        isWallSliding = (wallSensorR1.State() && wallSensorR2.State()) || (wallSensorL1.State() && wallSensorL2.State());
        animator.SetBool("WallSlide", isWallSliding);

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

    private IEnumerator Roll()
    {
        animator.SetTrigger("Roll");
        rolling = true;

        IgnoreEnemyCollisions(true);

        Vector2 rollDirection = new Vector2(facingDirection, 0).normalized;
        Vector2 rollTarget = new Vector2(transform.position.x + rollDirection.x * rollDistance, transform.position.y);

        float rollTime = 0.5f;
        float elapsedTime = 0f;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        while (elapsedTime < rollTime)
        {
            Vector2 newPosition = Vector2.Lerp(transform.position, rollTarget, elapsedTime / rollTime);
            rb.MovePosition(newPosition);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rolling = false;
        IgnoreEnemyCollisions(false);
        timeSinceRoll = 0.0f;
    }
    void stopRoll()
    {
        rolling = false;
    }

    void IgnoreEnemyCollisions(bool ignore)
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

    public void TakeDamage(int damage, int facingDirectionEnemy)
    {
        if (blocking || rolling)
        {
            if (blocking) { animator.SetTrigger("Block"); }
            return;
        }

        animator.SetTrigger("Hurt");
        Debug.Log("меня хуярят!");
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            animator.SetTrigger("Death");
            gameObject.tag = "Untagged";
            mDied = true;
        }
    }

    public bool isDied()
    {
        return mDied;
    }

    // Animation Events
    void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (facingDirection == 1)
            spawnPosition = wallSensorR2.transform.position;
        else
            spawnPosition = wallSensorL2.transform.position;

        if (slideDust != null)
        {
            GameObject dust = Instantiate(slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            dust.transform.localScale = new Vector3(facingDirection, 1, 1);
        }
    }
}
