using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector2 velocity;   // manually controlled velocity
    public float lifetime = 3f; // destroy after 3 seconds

    private float timer = 0f;

    private void Update()
    {
        float dt = Time.deltaTime;

        // Move the projectile using manual physics
        Vector3 pos = transform.position;
        pos += (Vector3)(velocity * dt);
        transform.position = pos;

        // Count down lifetime
        timer += dt;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
