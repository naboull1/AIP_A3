using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPlaneController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float baseForwardSpeed = 8f;     // constant speed to the right
    [SerializeField] private float strafeAcceleration = 30f;  // how quickly input changes your extra velocity
    [SerializeField] private float maxStrafeSpeed = 10f;      // max extra speed from input/dash
    [SerializeField] private float velocityDrag = 5f;         // how quickly extra velocity fades when not holding input

    [Header("Dash Settings")]
    [SerializeField] private float dashBoost = 15f;           // strength of dash impulse
    [SerializeField] private float dashCooldown = 1.0f;       // seconds between dashes

    [Header("Collision Settings")]
    [SerializeField] private float collisionRadius = 0.6f;    // approximate size of player for collisions

    [Header("Score")]
    [SerializeField] private int scorePerCollectible = 10;
    private int score = 0;

    [Header("Shooting Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private float fireCooldown = 0.2f;

    private Vector2 velocity;            // extra velocity controlled by player/dash
    private Vector3 spawnPosition;       // where we respawn on death

    private float dashCooldownTimer = 0f;
    private float fireCooldownTimer = 0f;

    private void Awake()
    {
        spawnPosition = transform.position;
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        // --- Update cooldown timers ---
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= dt;

        if (fireCooldownTimer > 0f)
            fireCooldownTimer -= dt;

        // --- Read input ---
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        float vertical = Input.GetAxisRaw("Vertical");   // W/S or Up/Down
        Vector2 inputDir = new Vector2(horizontal, vertical).normalized;

        // --- Apply strafe acceleration based on input ---
        // This only affects the "extra" velocity, not the base forward speed
        velocity += inputDir * strafeAcceleration * dt;

        // --- Apply drag when not really steering ---
        if (inputDir.sqrMagnitude < 0.01f)
        {
            // Pull extra velocity back toward zero over time
            velocity = Vector2.MoveTowards(velocity, Vector2.zero, velocityDrag * dt);
        }

        // --- Dash (adds a burst into velocity) ---
        HandleDash(inputDir);

        // --- Clamp extra velocity so it doesn't get insane ---
        if (velocity.magnitude > maxStrafeSpeed)
        {
            velocity = velocity.normalized * maxStrafeSpeed;
        }

        // --- Move the plane ---
        // Always move forward at baseForwardSpeed, plus any extra velocity
        Vector2 move = new Vector2(baseForwardSpeed, 0f) + velocity;
        Vector3 pos = transform.position;
        pos += (Vector3)(move * dt);
        transform.position = pos;

        // --- Collisions ---
        CheckCollisionWithObstacles();
        CheckCollisionWithCollectibles();
        CheckCollisionWithBoss();

        // --- Shooting ---
        HandleShooting();
    }

    private void HandleDash(Vector2 inputDir)
    {
        if (dashCooldownTimer > 0f)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector2 dashDir;

            if (inputDir.sqrMagnitude > 0.01f)
            {
                dashDir = inputDir.normalized;
            }
            else
            {
                dashDir = Vector2.right; // default forward dash
            }

            // Add an instant burst to extra velocity
            velocity += dashDir * dashBoost;

            dashCooldownTimer = dashCooldown;
        }
    }

    private void HandleShooting()
    {
        if (fireCooldownTimer > 0f)
            return;

        if (Input.GetMouseButtonDown(0) ||
            Input.GetKeyDown(KeyCode.LeftControl) ||
            Input.GetKeyDown(KeyCode.F))
        {
            GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile p = proj.GetComponent<Projectile>();
            p.velocity = Vector2.right * projectileSpeed;

            fireCooldownTimer = fireCooldown;
        }
    }

    private void CheckCollisionWithObstacles()
    {
        foreach (Obstacle obstacle in Obstacle.AllObstacles)
        {
            if (obstacle == null) continue;

            float dist = Vector2.Distance(transform.position, obstacle.transform.position);

            if (dist <= collisionRadius + obstacle.radius)
            {
                OnHitObstacle(obstacle);
                break;
            }
        }
    }

    private void OnHitObstacle(Obstacle obstacle)
    {
        Debug.Log("Player hit obstacle: " + obstacle.name);

        transform.position = spawnPosition;
        velocity = Vector2.zero;
    }

    private void CheckCollisionWithCollectibles()
    {
        for (int i = Collectible.AllCollectibles.Count - 1; i >= 0; i--)
        {
            Collectible c = Collectible.AllCollectibles[i];
            if (c == null) continue;

            float dist = Vector2.Distance(transform.position, c.transform.position);

            if (dist <= collisionRadius + c.radius)
            {
                OnCollect(c);
            }
        }
    }

    private void OnCollect(Collectible c)
    {
        score += scorePerCollectible;
        Debug.Log($"Collected {c.name}. Score = {score}");

        Destroy(c.gameObject);
    }

    private void CheckCollisionWithBoss()
    {
        foreach (Boss boss in Boss.AllBosses)
        {
            if (boss == null) continue;

            float dist = Vector2.Distance(transform.position, boss.transform.position);

            if (dist <= collisionRadius + boss.radius)
            {
                OnHitBoss(boss);
                break;
            }
        }
    }

    private void OnHitBoss(Boss boss)
    {
        Debug.Log($"Reached boss! Current score = {score}, required = {boss.requiredScoreToDefeat}");

        if (score >= boss.requiredScoreToDefeat)
        {
            Debug.Log("You defeated the boss! YOU WIN!");
            ReloadCurrentScene();
        }
        else
        {
            Debug.Log("Not enough score. Boss destroys you. GAME OVER.");
            ReloadCurrentScene();
        }
    }

    private void ReloadCurrentScene()
    {
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }
}
