using UnityEngine;

public class PlayerPlaneController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float acceleration = 15f;        // how fast we speed up when pressing keys
    [SerializeField] private float maxSpeed = 10f;            // cap on how fast we can go
    [SerializeField] private float friction = 5f;             // slows us down when not pressing anything
    [SerializeField] private float constantForwardSpeed = 4f; // auto-move to the right

    [Header("Dash Settings")]
    [SerializeField] private float dashBoost = 15f;           // how strong the dash impulse is
    [SerializeField] private float dashCooldown = 1.0f;       // time between dashes in seconds

    private Vector2 velocity;            // our manual velocity (no Rigidbody)
    private float dashCooldownTimer = 0; // counts down after a dash

    [Header("Shooting Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private float fireCooldown = 0.2f;

    private float fireCooldownTimer = 0f;


    private void Update()
    {
        float dt = Time.deltaTime;

        // --- COOLDOWN TIMER ---
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= dt;
        }

        // 1. Read input
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        float vertical = Input.GetAxisRaw("Vertical");   // W/S or Up/Down
        Vector2 inputDir = new Vector2(horizontal, vertical).normalized;

        // 2. Apply acceleration from input
        Vector2 accel = inputDir * acceleration;

        // 3. Add a constant push to the right ONLY if you're not holding left
        if (horizontal >= 0f)
        {
            accel += Vector2.right * constantForwardSpeed;
        }



        // 4. Integrate velocity: v = v + a * dt
        velocity += accel * dt;

        // 5. Apply friction when there's little/no input so it doesn't drift forever
        if (inputDir.sqrMagnitude < 0.01f)
        {
            // move velocity toward zero over time
            velocity = Vector2.MoveTowards(velocity, Vector2.zero, friction * dt);
        }

        // --- DASH INPUT ---
        HandleDash(inputDir);

        // 6. Clamp max speed (after dash too)
        if (velocity.magnitude > maxSpeed)
        {
            velocity = velocity.normalized * maxSpeed;
        }

        // 7. Integrate position: p = p + v * dt
        Vector3 pos = transform.position;
        pos += (Vector3)(velocity * dt);
        transform.position = pos;

        // --- FIRE COOLDOWN TIMER ---
        if (fireCooldownTimer > 0f)
        {
            fireCooldownTimer -= dt;
        }

        // --- SHOOTING INPUT ---
        HandleShooting();

    }

    private void HandleDash(Vector2 inputDir)
    {
        // Only dash when cooldown is ready and Space is pressed
        if (dashCooldownTimer > 0f)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Decide dash direction:
            // - If player is pressing a direction, dash that way
            // - Otherwise, default to the right (forward)
            Vector2 dashDir;

            if (inputDir.sqrMagnitude > 0.01f)
            {
                dashDir = inputDir.normalized;
            }
            else
            {
                dashDir = Vector2.right; // default forward dash
            }

            // Apply an instant boost to velocity
            velocity += dashDir * dashBoost;

            // Start cooldown
            dashCooldownTimer = dashCooldown;
        }
    }

    private void HandleShooting()
    {
        // Only fire when cooldown ready
        if (fireCooldownTimer > 0f)
            return;

        // Left mouse OR Ctrl OR F key
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.F))
        {
            // Create projectile
            GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            // Get projectile script and assign manual velocity
            Projectile p = proj.GetComponent<Projectile>();
            p.velocity = Vector2.right * projectileSpeed;

            // Reset cooldown
            fireCooldownTimer = fireCooldown;
        }
    }

}
