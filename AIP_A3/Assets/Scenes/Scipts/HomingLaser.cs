using UnityEngine;

public class HomingLaser : MonoBehaviour
{
    [Header("Homing Settings")]
    [SerializeField] private float speed = 12f;          // how fast it moves
    [SerializeField] private float turnSpeed = 4f;       // how quickly it turns towards the player
    [SerializeField] public float radius = 0.3f;         // size for collision

    [Header("Lifetime")]
    [SerializeField] private float lifetime = 5f;        // destroy after this many seconds

    [Header("Target")]
    [SerializeField] private Transform target;           // the thing we are chasing (player)

    private Vector2 direction;           // current movement direction (normalized)
    private float timer = 0f;

    private void Start()
    {
        // If no target assigned in Inspector, try to find the player by tag
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
        }

        // Initial direction: if we have a target, aim at it; otherwise, just go right
        if (target != null)
        {
            Vector2 toTarget = (target.position - transform.position);
            if (toTarget.sqrMagnitude > 0.001f)
            {
                direction = toTarget.normalized;
            }
            else
            {
                direction = Vector2.right;
            }
        }
        else
        {
            direction = Vector2.right;
        }
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        // 1. Steer the direction towards the target
        if (target != null)
        {
            Vector2 toTarget = (Vector2)(target.position - transform.position);

            if (toTarget.sqrMagnitude > 0.001f)
            {
                Vector2 desiredDir = toTarget.normalized;

                // Smoothly rotate current direction towards desired direction
                // Lerp factor scaled by turnSpeed and dt
                direction = Vector2.Lerp(direction, desiredDir, turnSpeed * dt).normalized;
            }
        }

        // 2. Move along the current direction
        Vector3 pos = transform.position;
        pos += (Vector3)(direction * speed * dt);
        transform.position = pos;

        // 3. Optional: rotate the laser sprite to face its movement direction
        if (direction.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        // 4. Lifetime
        timer += dt;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    // This will let the Boss set the target and initial direction later
    public void Init(Transform newTarget)
    {
        target = newTarget;

        Vector2 toTarget = (target.position - transform.position);
        if (toTarget.sqrMagnitude > 0.001f)
        {
            direction = toTarget.normalized;
        }
        else
        {
            direction = Vector2.right;
        }
    }
}
