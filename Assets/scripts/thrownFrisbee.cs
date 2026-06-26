using UnityEngine;

public class thrownFrisbee : MonoBehaviour
{
    private Vector2 velocity;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
        }
    }

    public void Launch(Vector2 direction, float speed, Collider2D playerCollider)
    {
        velocity = direction.normalized * speed;

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = direction.x < 0;
        }

        if (playerCollider != null && circleCollider != null)
        {
            Physics2D.IgnoreCollision(circleCollider, playerCollider, true);
        }
    }

    void Update()
    {
        transform.position += (Vector3)(velocity * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) return;

        if (collision.CompareTag("target"))
        {
            if (gameManager.instance != null)
            {
                gameManager.instance.RemoveTarget();
            }
            Destroy(collision.gameObject);
            Destroy(gameObject);
            return;
        }

        if (collision.CompareTag("ground"))
        {
            Destroy(gameObject);
        }
    }
}
