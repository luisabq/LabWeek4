using UnityEngine;

// Orbits this enemy around a center point using math.
[RequireComponent(typeof(EnemySpeedByDistance))]
public class EnemyOrbit : MonoBehaviour
{
    public enum OrbitMethod { Trigonometric, Quaternion, Matrix }

    [SerializeField] private OrbitMethod method = OrbitMethod.Quaternion;
    [SerializeField] private Transform orbitCenter;
    [SerializeField] private float fallbackRadius = 3f; // Used if we spawn exactly on the center.

    private EnemySpeedByDistance speedByDistance;
    private EnemyCollisionAvoidance collisionAvoidance; // optional stretch-goal component
    private float currentAngleDegrees;
    private float orbitRadius;              
    private Vector3 initialOffsetDirection; // unit vector pointing from center to this enemy's start

    private void Awake()
    {
        speedByDistance = GetComponent<EnemySpeedByDistance>();
        collisionAvoidance = GetComponent<EnemyCollisionAvoidance>(); // null if not attached, and that's fine
    }

    private void Start()
    {
        Vector3 offset = transform.position - orbitCenter.position;
        orbitRadius = offset.magnitude;

        if (orbitRadius > 0.0001f)
        {
            initialOffsetDirection = offset / orbitRadius; // equivalent to offset.normalized, reusing the magnitude we already computed
        }
        else
        {
            // Give it a small starting circle instead of dividing by zero later.
            orbitRadius = fallbackRadius;
            initialOffsetDirection = Vector3.right;
        }
    }

    private void Update()
    {
        // Convert linear speed into angular speed so larger circles do not drift.
        float linearSpeed = speedByDistance.GetCurrentSpeed();
        float angularSpeedDegreesPerSecond = (linearSpeed / Mathf.Max(orbitRadius, 0.0001f)) * Mathf.Rad2Deg;
        currentAngleDegrees += angularSpeedDegreesPerSecond * Time.deltaTime;

        Vector3 offset = method switch
        {
            OrbitMethod.Trigonometric => GetTrigonometricOffset(),
            OrbitMethod.Matrix => GetMatrixOffset(),
            _ => GetQuaternionOffset(),
        };

        Vector3 orbitPosition = orbitCenter.position + offset;

        if (collisionAvoidance != null)
        {
            orbitPosition += collisionAvoidance.GetAvoidanceVector() * Time.deltaTime;
        }

        transform.position = orbitPosition;
    }

    // Direct sine/cosine orbit.
    private Vector3 GetTrigonometricOffset()
    {
        float radians = currentAngleDegrees * Mathf.Deg2Rad;
        float x = Mathf.Cos(radians) * orbitRadius;
        float y = Mathf.Sin(radians) * orbitRadius;
        return new Vector3(x, y, 0f);
    }

    // Rotate the starting offset with a quaternion.
    private Vector3 GetQuaternionOffset()
    {
        Quaternion rotation = Quaternion.Euler(0f, 0f, currentAngleDegrees);
        return rotation * initialOffsetDirection * orbitRadius;
    }

    // Same orbit, but using a rotation matrix.
    private Vector3 GetMatrixOffset()
    {
        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0f, 0f, currentAngleDegrees));
        Vector3 baseOffset = initialOffsetDirection * orbitRadius;
        return rotationMatrix.MultiplyPoint3x4(baseOffset);
    }
}