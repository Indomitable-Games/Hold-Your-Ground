using Assets.Scripts;

using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    //[SerializeField] private float sprintMultiplier = 1.5f;

    public HealthBar healthBar;

    private Rigidbody2D rb;
    private PlayerInputHandler inputHandler;
    private Vector2 velocity;

    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        //collider2D = GetComponent<Collider2D>();
        moveSpeed = Globals.playerSpeed;
    }

    // Start is called before the first frame update
    void Start()
    {
        inputHandler = PlayerInputHandler.Instance;

        healthBar.SetMaxHealth(Globals.fuel);
    }

    // Update is called once per frame
    void Update()
    {
        velocity = inputHandler.MoveInput;

    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    void ApplyMovement()
    {
        float speed = moveSpeed;
        /*if (inputHandler.SprintValue > 0)
        {
            animator.speed = sprintMultiplier;
            speed *= sprintMultiplier;
        }*/
        rb.linearVelocity = new Vector2(velocity.x * speed, velocity.y * speed);

        if (velocity.sqrMagnitude > 0.01f) // Prevent jittering when velocity is near zero
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90); // Offset by 90 degrees
        }
        //animator.SetFloat("Horizontal", velocity.x);
        //animator.SetFloat("Vertical", velocity.y);
        //animator.SetFloat("Speed", velocity.sqrMagnitude);
        healthBar.SetHealth(healthBar.GetHealth() - 1);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag != "World")
            return;
        other.gameObject.GetComponent<Tilemap>().SetTile(Vector3Int.RoundToInt(this.gameObject.transform.position) - new Vector3Int(1,0,0), null);
        other.gameObject.GetComponent<Tilemap>().SetTile(Vector3Int.RoundToInt(this.gameObject.transform.position), null);


    }
}
