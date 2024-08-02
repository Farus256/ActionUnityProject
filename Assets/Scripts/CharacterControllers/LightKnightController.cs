using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightKnightController : PlayerControllerBase
{
    private Sensor_HeroKnight wallSensorR1;
    private Sensor_HeroKnight wallSensorR2;
    private Sensor_HeroKnight wallSensorL1;
    private Sensor_HeroKnight wallSensorL2;
    private bool noBlood = false;

    protected override void Start()
    {
        // Initialize the specific stats for LightKnight
        currentHealth = 150;
        speed = 5f;
        attackRange = 1.5f;
        jumpForce = 7f;
        rollSpeed = 8f;
        rollDuration = 0.5f;
        attackDamage = 25;
        rollDistance = 5f;

        base.Start(); // Call the base class Start method to initialize components
    }

    protected override void InitializeComponents()
    {
        base.InitializeComponents(); // Call the base class InitializeComponents method

        // Initialize additional components specific to LightKnight
        groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
    }

    public override void TakeDamage(int damage, int facingDirectionEnemy)
    {
        if (blocking && facingDirectionEnemy != facingDirection || rolling)
        {
            if (blocking) { animator.SetTrigger("Block"); }
            return;
        }

        animator.SetTrigger("Hurt");

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            noBlood = false;
            animator.SetBool("noBlood", noBlood);
            animator.SetBool("Death", true);
            gameObject.tag = "Untagged";
            mDied = true;
        }
    }

    // Override other methods if you need specific behaviors for LightKnight
}
