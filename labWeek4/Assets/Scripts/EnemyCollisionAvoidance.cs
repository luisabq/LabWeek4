using UnityEngine;

// Pushes nearby enemies apart so they do not stack on top of each other.
public class EnemyCollisionAvoidance : MonoBehaviour
{
    [SerializeField] private float avoidRadius = 1.5f;
    [SerializeField] private float avoidStrength = 4f;
    [SerializeField] private LayerMask enemyLayer;

    // Called by EnemyOrbit.cs (if this component is present) and added to the orbit position.
    public Vector3 GetAvoidanceVector()
    {
        Vector3 avoidance = Vector3.zero;
        Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, avoidRadius, enemyLayer);

        foreach (Collider other in nearbyEnemies)
        {
            if (other.transform == transform) continue;

            Vector3 awayFromOther = transform.position - other.transform.position;
            float sqrDistance = awayFromOther.sqrMagnitude;
            if (sqrDistance < 0.0001f) continue;

            // Closer enemies push harder.
            avoidance += awayFromOther.normalized / sqrDistance;
        }

        return avoidance * avoidStrength;
    }
}