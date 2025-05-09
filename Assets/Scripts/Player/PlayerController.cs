using Assets.Scripts;
using Assets.Scripts.Objects;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    //[SerializeField] private float sprintMultiplier = 1.5f;

    public HealthBar healthBar;
    public ResourceManager resourceManager;

    private Rigidbody2D rb;
    private PlayerInputHandler inputHandler;
    private Vector2 velocity;

    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        //collider2D = GetComponent<Collider2D>();
        moveSpeed = Globals.Player.Speed;
    }

    // Start is called before the first frame update
    void Start()
    {
        inputHandler = PlayerInputHandler.Instance;

        healthBar.SetMaxHealth(Globals.Player.Fuel);
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
        List<TileBase> actualCollides = new List<TileBase>();
        actualCollides.Add(other.gameObject.GetComponent<Tilemap>().GetTile(Vector3Int.RoundToInt(this.gameObject.transform.position)));
        actualCollides.Add(other.gameObject.GetComponent<Tilemap>().GetTile(Vector3Int.RoundToInt(this.gameObject.transform.position) - new Vector3Int(1, 0, 0)));
        other.gameObject.GetComponent<Tilemap>().SetTile(Vector3Int.RoundToInt(this.gameObject.transform.position) - new Vector3Int(1,0,0), null);
        other.gameObject.GetComponent<Tilemap>().SetTile(Vector3Int.RoundToInt(this.gameObject.transform.position), null);

        foreach(TileBase tile in actualCollides)
        {
            Debug.Log(tile.name + actualCollides.IndexOf(tile));
            string resourceKey = Globals.TileResourceMap[tile.name];
            Resource resourceDictKey = Globals.ResourceDictionary[resourceKey];

            Globals.Player.PlayerResources[resourceDictKey] = Globals.Player.PlayerResources.GetValueOrDefault(resourceDictKey, 0) + 1;
            Debug.Log(resourceDictKey.Name + ": " + Globals.Player.PlayerResources[resourceDictKey]);
        }
    }
}
