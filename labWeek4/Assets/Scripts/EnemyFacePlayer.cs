using UnityEngine;

// Turns the enemy to face the player using math.
public class EnemyFacePlayer : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float turnSpeedDegreesPerSecond = 180f;
    [SerializeField] private float spriteForwardOffsetDegrees = -90f;

    private void Update()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.z = 0f;
        if (directionToPlayer.sqrMagnitude < 0.0001f) return;

        // Atan2 gives us the angle directly.
        float targetAngleDegrees = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        targetAngleDegrees += spriteForwardOffsetDegrees;

        // Slerp keeps the turn smooth.
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngleDegrees);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            turnSpeedDegreesPerSecond * Time.deltaTime / 180f
        );
    }
}