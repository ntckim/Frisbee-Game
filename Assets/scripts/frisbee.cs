using UnityEngine;

public class frisbee : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    [SerializeField] private float speed = 5f;

    private bool caught;
    private bool playerTouching;
    private Vector3 nextPosition;

    void Start()
    {
        SetupCollider();

        if (pointA != null)
        {
            transform.position = pointA.position;
            nextPosition = pointB != null ? pointB.position : pointA.position;
        }
    }

    void Update()
    {
        if (caught || pointA == null || pointB == null) return;

        transform.position = Vector3.MoveTowards(transform.position, nextPosition, speed * Time.deltaTime);
        if (transform.position == nextPosition)
        {
            nextPosition = nextPosition == pointA.position ? pointB.position : pointA.position;
        }

        if (playerTouching && Input.GetKeyDown(KeyCode.Space))
        {
            CatchFrisbee();
        }
    }

    private void SetupCollider()
    {
        CircleCollider2D col = GetComponent<CircleCollider2D>();
        if (col == null) col = gameObject.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.6f;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (caught) return;
        if (!collision.CompareTag("Player")) return;

        playerTouching = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerTouching = false;
    }

    private void CatchFrisbee()
    {
        if (caught) return;

        caught = true;
        playerTouching = false;
        gameObject.SetActive(false);

        if (gameManager.instance != null)
        {
            gameManager.instance.FrisbeeCaught();
        }
    }
}
