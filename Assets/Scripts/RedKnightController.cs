using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedKnightController : PlayerControllerBase
{
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
    }

    // Override other methods if you need specific behaviors for LightKnight
}
