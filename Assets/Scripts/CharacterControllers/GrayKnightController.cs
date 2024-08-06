using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrayKnightController : PlayerControllerBase
{
    private void Start()
    {
        currentHealth = 150;
        speed = 5f;
        attackRange = 1.5f;
        jumpForce = 7f;
        rollSpeed = 8f;
        rollDuration = 0.5f;
        attackDamage = 25;
        rollDistance = 5f;

        base.Start();
    }
    protected override void HandleInput()
    {

        float inputX = Input.GetAxis("Horizontal");

        UpdateDirection(inputX);

        if (!rolling && !blocking)
            body2d.velocity = new Vector2(inputX * speed, body2d.velocity.y);

        if (Input.GetMouseButtonDown(0) && timeSinceAttack > 0.25f && !rolling)
            HandleAttack();

        if (Input.GetMouseButtonDown(1) && !rolling)
        {
            currentAttack = 2;
            HandleAttack();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !rolling && !isWallSliding && timeSinceRoll > 0.8f)
            StartCoroutine(Roll());

        if (Input.GetKeyDown("space") && grounded && !rolling)
            Jump();
    }

}
