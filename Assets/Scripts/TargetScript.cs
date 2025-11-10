using UnityEngine;
using UnityEngine.SceneManagement;

public class TargetScript : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1.0f;

    private bool isDashing = false;
    private bool canDash = true;

    void Update()
    {
        Move();
    }

    void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
            StartCoroutine(Dash(horizontal, vertical));

        // Movimento normal
        if (!isDashing)
        {
            Vector2 movement = new Vector2(horizontal, vertical).normalized * walkSpeed;
            rb.linearVelocity = movement;
        }
    }

    private System.Collections.IEnumerator Dash(float horizontal, float vertical)
    {
        canDash = false;
        isDashing = true;

        Vector2 dashDirection = new Vector2(horizontal, vertical).normalized;
        if (dashDirection == Vector2.zero)
            dashDirection = transform.right; // se parado, dash pra frente

        rb.linearVelocity = dashDirection * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        rb.linearVelocity = Vector2.zero; // para após o dash

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
