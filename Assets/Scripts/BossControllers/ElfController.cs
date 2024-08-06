using UnityEngine;
using System.Collections;

public class ElfController : EnemyControllerBase
{
    protected override void Start()
    {
        moveSpeed = 2f;
        base.Start();
        // Additional initialization specific to the boss
    }

    protected override IEnumerator Attack()
    {
        isAttacking = true;
        animator.SetTrigger("isAttacking");
        attackCooldownTimer = attackCooldown;

        // Attack logic
        if (Vector2.Distance(transform.position, playerTransform.position) <= attackRange)
        {
            playerTransform.GetComponent<PlayerControllerBase>().TakeDamage(attackDamage, facingDirection);
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    protected override IEnumerator Dash()
    {
        isDashing = true;
        animator.SetBool("isDashing", true);
        dashCooldownTimer = dashCooldown;

        Vector2 dashDirection = (playerTransform.position - transform.position).normalized;
        Vector2 dashTarget = new Vector2(transform.position.x + dashDirection.x * dashDistance, transform.position.y);

        float dashTime = 0.3f; // Example value
        float elapsedTime = 0f;

        while (elapsedTime < dashTime)
        {
            transform.position = Vector2.Lerp(transform.position, dashTarget, elapsedTime / dashTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
        animator.SetBool("isDashing", false);
    }

    protected override IEnumerator Jump()
    {
        isJumping = true;
        animator.SetTrigger("isJumping");
        jumpCooldownTimer = jumpCooldown;

        Vector2 jumpTarget = new Vector2(transform.position.x + dashDistance * facingDirection, transform.position.y + jumpHeight);
        float jumpTime = 0.3f; // Example value
        float elapsedTime = 0f;

        while (elapsedTime < jumpTime)
        {
            transform.position = Vector2.Lerp(transform.position, jumpTarget, elapsedTime / jumpTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isJumping = false;
    }
}
