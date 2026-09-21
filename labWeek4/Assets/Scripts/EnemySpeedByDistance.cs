using UnityEngine;

// Changes enemy speed based on distance to the player.
public class EnemySpeedByDistance : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float slowSpeed = 1f;
    [SerializeField] private float fastSpeed = 6f;
    [SerializeField] private float nearDistance = 3f;  // at or closer than this -> fastSpeed
    [SerializeField] private float farDistance = 12f;  // at or farther than this -> slowSpeed

    private float nearDistanceSquared;
    private float farDistanceSquared;

    private void Start()
    {
        nearDistanceSquared = nearDistance * nearDistance;
        farDistanceSquared = farDistance * farDistance;
    }

    public float GetCurrentSpeed()
    {
        float sqrDistance = (player.position - transform.position).sqrMagnitude;

        if (sqrDistance <= nearDistanceSquared) return fastSpeed;
        if (sqrDistance >= farDistanceSquared) return slowSpeed;

        // Blend between the two speeds without taking a square root.
        float t = Mathf.InverseLerp(nearDistanceSquared, farDistanceSquared, sqrDistance);
        return Mathf.Lerp(fastSpeed, slowSpeed, t);
    }

    // Used by EnemyOrbit.cs to drive the orbit speed.
}