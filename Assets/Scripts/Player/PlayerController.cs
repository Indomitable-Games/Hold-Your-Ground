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
        // Quick validation with strong typing
        if (other.tag != "World" || !(other.TryGetComponent<Tilemap>(out Tilemap tilemap)) ||
            !TryGetComponent<EdgeCollider2D>(out EdgeCollider2D edgeCollider))
            return;

        // Transform collider points to world space with explicit typing
        Vector2[] localPoints = edgeCollider.points;
        Vector2[] worldPoints = new Vector2[localPoints.Length];
        for (int i = 0; i < localPoints.Length; i++)
        {
            worldPoints[i] = (Vector2)transform.TransformPoint(localPoints[i]);
        }

        // Calculate bounds with explicit types
        Bounds bounds = new Bounds(worldPoints[0], Vector3.zero);
        for (int i = 1; i < worldPoints.Length; i++)
            bounds.Encapsulate(worldPoints[i]);

        bounds.Expand(0.1f);

        // Get tile range with strong typing
        Vector3Int minCell = tilemap.WorldToCell(bounds.min);
        Vector3Int maxCell = tilemap.WorldToCell(bounds.max);
        int tileCount = 0;

        // Process each potential tile
        for (int x = minCell.x; x <= maxCell.x; x++)
        {
            for (int y = minCell.y; y <= maxCell.y; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(cellPos);
                if (tile == null) continue;

                // Create tile rect with explicit types
                Vector3 cellCenter = tilemap.GetCellCenterWorld(cellPos);
                Vector2 tileSize = tilemap.cellSize;
                Rect tileRect = new Rect(
                    cellCenter.x - tileSize.x / 2f,
                    cellCenter.y - tileSize.y / 2f,
                    tileSize.x,
                    tileSize.y
                );

                // Check for intersection
                bool intersects = false;
                for (int i = 0; i < worldPoints.Length - 1 && !intersects; i++)
                {
                    intersects = LineIntersectsRect(worldPoints[i], worldPoints[i + 1], tileRect);
                }

                if (intersects)
                {
                    // Process resource collection with strong typing
                    if (Globals.TileResourceMap.TryGetValue(tile.name, out string resourceKey))
                    {
                        Resource resource = Globals.ResourceDictionary[resourceKey];
                        if ((!resource.IsBaseTile))
                        {
                            int currentAmount = Globals.Player.PlayerResources.GetValueOrDefault(resource, 0);
                            Globals.Player.PlayerResources[resource] = currentAmount + 1;
                            Debug.Log($"{resource.Name}: {Globals.Player.PlayerResources[resource]}");

                        }
                    }

                    // Clear the tile
                    tilemap.SetTile(cellPos, null);
                    tileCount++;
                }
            }
        }

        if (tileCount > 0)
        {
            Debug.Log($"Collected {tileCount} tiles");
        }
    }

    // Strongly typed intersection methods
    private bool LineIntersectsRect(Vector2 p1, Vector2 p2, Rect rect)
    {
        // Check if either point is inside
        if (rect.Contains(p1) || rect.Contains(p2)) return true;

        // Define rectangle corners with explicit Vector2 initialization
        Vector2 topLeft = new Vector2(rect.xMin, rect.yMax);
        Vector2 topRight = new Vector2(rect.xMax, rect.yMax);
        Vector2 bottomRight = new Vector2(rect.xMax, rect.yMin);
        Vector2 bottomLeft = new Vector2(rect.xMin, rect.yMin);

        // Check intersection with each edge
        return LinesIntersect(p1, p2, topLeft, topRight) ||
               LinesIntersect(p1, p2, topRight, bottomRight) ||
               LinesIntersect(p1, p2, bottomRight, bottomLeft) ||
               LinesIntersect(p1, p2, bottomLeft, topLeft);
    }

    private bool LinesIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        // Calculate direction vectors with explicit typing
        Vector2 d1 = p2 - p1;
        Vector2 d2 = p4 - p3;
        float det = d1.x * d2.y - d1.y * d2.x;

        // Parallel lines don't intersect
        if (Mathf.Approximately(det, 0f)) return false;

        Vector2 d3 = p1 - p3;
        float t = (d2.x * d3.y - d2.y * d3.x) / det;
        float u = (d1.x * d3.y - d1.y * d3.x) / det;

        // Check if intersection point is on both segments
        return t >= 0f && t <= 1f && u >= 0f && u <= 1f;
    }
}
