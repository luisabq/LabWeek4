using UnityEngine;
using UnityEngine.InputSystem;

// Moves the player left and right, then keeps it inside the camera view.
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float edgePadding = 0.5f; // Leaves a little space at the edge.

    private Camera mainCamera;
    private float minX;
    private float maxX;

    private void Start()
    {
        mainCamera = Camera.main;
        RecalculateBounds();
    }

    private void RecalculateBounds()
    {
        // Find the world-space screen edges at the player's depth.
        float distanceToPlayer = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);

        Vector3 leftEdge = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f, distanceToPlayer));
        Vector3 rightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1f, 0f, distanceToPlayer));

        minX = leftEdge.x + edgePadding;
        maxX = rightEdge.x - edgePadding;
    }

    private void Update()
    {
        float horizontalInput = GetHorizontalInput();

        // Move only along X.
        Vector3 movement = Vector3.right * horizontalInput * moveSpeed * Time.deltaTime;
        Vector3 targetPosition = transform.position + movement;

        // Keep the ship on screen.
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);

        transform.position = targetPosition;
    }

    // Simple keyboard input for the lab.
    private float GetHorizontalInput()
    {
        if (Keyboard.current == null) return 0f;

        float input = 0f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) input -= 1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) input += 1f;
        return input;
    }
}