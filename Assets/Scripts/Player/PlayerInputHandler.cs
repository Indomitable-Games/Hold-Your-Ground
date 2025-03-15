using Assets.Scripts;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name References")]
    [SerializeField] private string actionMapName = "Player";

    [Header("Action Name References")]
    [SerializeField] private string move = "Move";

    private InputAction moveAction;

    public static PlayerInputHandler Instance { get; private set; }

    public Vector2 MoveInput { get; private set; } = new Vector2(0, -1); // Default facing down
    private Vector2 targetDirection = new Vector2(0, -1); // The target direction from input
    private bool isMoving = false; // Track if movement is active

    [Header("Movement Settings")]
    public float MaxTurnAngle = 60f; // Maximum angle from (0,-1)
    public float TurnSpeed = .4f;   // How fast the player turns

    private void Awake()
    {
        MaxTurnAngle = Globals.playerTurnRadius;
        TurnSpeed = Globals.playerTurnSpeed;

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        moveAction = playerControls.FindActionMap(actionMapName).FindAction(move);
        RegisterInputActions();
    }

    private void FixedUpdate()
    {
        if (isMoving)
            MoveInput = CalcDirection();
    }

    private Vector2 CalcDirection()
    {

        float currentAngle = Mathf.Atan2(MoveInput.y, MoveInput.x) * Mathf.Rad2Deg;
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

        // Interpolate angle
        float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, TurnSpeed * Time.deltaTime);

        // Stop movement if the angle difference is small
        if (Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle)) < 0.5f)
        {
            MoveInput = targetDirection;
            isMoving = false;
        }

        // Convert back to vector
        return new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad));
    }

    void RegisterInputActions()
    {
        moveAction.performed += context =>
        {
            Vector2 newInput = context.ReadValue<Vector2>();
            if (newInput != Vector2.zero)
            {
                // Get input angle relative to (1,0)
                float newAngle = Mathf.Atan2(newInput.y, newInput.x) * Mathf.Rad2Deg;

                // Shift reference so (0,-1) is at 0 degrees
                newAngle -= 270f;
                if (newAngle < -180f) newAngle += 360f;

                // Clamp target angle
                float clampedAngle = Mathf.Clamp(newAngle, -MaxTurnAngle, MaxTurnAngle);

                // Convert back to world space
                float finalAngle = clampedAngle + 270f;
                targetDirection = new Vector2(Mathf.Cos(finalAngle * Mathf.Deg2Rad), Mathf.Sin(finalAngle * Mathf.Deg2Rad));

                isMoving = true; // Start moving as input is registered
            }
        };

        moveAction.canceled += context =>
        {
            isMoving = false; // Stop moving when input is released
        };
    }



    private void OnEnable()
    {
        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
    }
}
