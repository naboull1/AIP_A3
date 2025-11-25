using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Movement")]
    public Vector2 velocity;    // manually controlled velocity

    [Header("Collision")]
    [SerializeField] private float hitRadius = 0.2f; // approximate size of the projectile

    [Header("Lifetime")]
    public float lifetime = 3f; // destroy after 3 seconds
    private float timer = 0f;

    private void Update()
    {
        float dt = Time.deltaTime;

        //move projectile 
        Vector3 pos = transform.position;
        pos += (Vector3)(velocity * dt);
        transform.position = pos;

        //Check for collision with any obstacle
        CheckCollisionWithObstacles();

        //destroy after time
        timer += dt;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void CheckCollisionWithObstacles()
    {
        // Loop over all active obstacles
        foreach (Obstacle obstacle in Obstacle.AllObstacles)
        {
            if (obstacle == null) continue;

            // Distance between projectile and obstacle centres
            float dist = Vector2.Distance(transform.position, obstacle.transform.position);

            // If distance is less than sum of radius, they overlap
            if (dist <= hitRadius + obstacle.radius)
            {
                // Destroy both projectile and obstacle
                Destroy(obstacle.gameObject);
                Destroy(gameObject);
                break;
            }
        }
    }
}
