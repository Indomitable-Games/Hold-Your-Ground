using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    public float speed = 2f;
    public float jumpForce = 5f;
    public float attackRange = 1f;
    public int damage = 10;
    public LayerMask groundLayer;
    public LayerMask wallLayer;

    private GameObject player;
    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the Player GameObject has the correct tag.");
        }
    }

    void Update()
    {
        if (player == null) return;

        CheckGrounded();

        if (IsWithinAttackRange())
        {
            Debug.Log("Enemy attacks the player for " + damage + " damage!");
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Stop moving
        }
        else
        {
            MoveTowardsPlayer();
            JumpIfNeeded();
        }
    }

    void MoveTowardsPlayer()
    {
        float direction = Mathf.Sign(player.transform.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
    }

    void CheckGrounded()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);
    }

    void JumpIfNeeded()
    {
        if (!isGrounded) return; // Only jump if grounded

        float direction = Mathf.Sign(player.transform.position.x - transform.position.x);
        Vector2 position = transform.position;

        RaycastHit2D wallCheck = Physics2D.Raycast(position, Vector2.right * direction, 0.6f, wallLayer);

        if (wallCheck.collider) // Jump only when hitting a wall
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    bool IsWithinAttackRange()
    {
        return Vector2.Distance(transform.position, player.transform.position) < attackRange;
    }
}
