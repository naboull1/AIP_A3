using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class PlayerPlaneController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float baseForwardSpeed = 8f;     // constant speed to the right
    [SerializeField] private float strafeAcceleration = 30f;  // how quickly input changes your extra velocity
    [SerializeField] private float maxStrafeSpeed = 10f;      // max extra speed from dash
    [SerializeField] private float velocityDrag = 5f;         // how quickly extra velocity fades when not holding input

    [Header("Dash Settings")]
    [SerializeField] private float dashBoost = 15f;           // strength of dash impulse
    [SerializeField] private float dashCooldown = 1.0f;       // seconds between dashes

    [Header("Collision Settings")]
    [SerializeField] private float collisionRadius = 0.6f;    // size of player for collisions


    [Header("Score")]
    [SerializeField] private int scorePerCollectible = 10;
    private int score = 0;
    public int CurrentScore => score;



    [Header("Shooting Settings")]
    [SerializeField] private GameObject projectilePrefab;   // socket for projectile prefab
    [SerializeField] private float projectileSpeed = 20f;   //  speed
    [SerializeField] private float fireCooldown = 0.2f;     // cooldown

    private Vector2 velocity;            // extra velocity controlled by player/dash
    private Vector3 spawnPosition;       // respawn location upon death

    private float dashCooldownTimer = 0f;
    private float fireCooldownTimer = 0f;

    [SerializeField] private UIManager uiManager;
    private bool isGameOver = false;




    private void Awake()
    {
        spawnPosition = transform.position;
    }


    private void Update()
    {
        float dt = Time.deltaTime;

        // update cooldown timers
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= dt;

        if (fireCooldownTimer > 0f)
            fireCooldownTimer -= dt;

        // read input
        float horizontal = Input.GetAxisRaw("Horizontal"); // horizonal movement
        float vertical = Input.GetAxisRaw("Vertical");   // vertical movement
        Vector2 inputDir = new Vector2(horizontal, vertical).normalized;

        velocity += inputDir * strafeAcceleration * dt;

        if (inputDir.sqrMagnitude < 0.01f)
        {
            velocity = Vector2.MoveTowards(velocity, Vector2.zero, velocityDrag * dt);
        }

        // adds a burst into velocity
        HandleDash(inputDir);

        if (velocity.magnitude > maxStrafeSpeed)
        {
            velocity = velocity.normalized * maxStrafeSpeed;
        }

        // move the plane
        // always move forward at baseForwardSpeed, plus any extra velocity
        Vector2 move = new Vector2(baseForwardSpeed, 0f) + velocity;
        Vector3 pos = transform.position;
        pos += (Vector3)(move * dt);
        transform.position = pos;

        // Collisions
        CheckCollisionWithObstacles();
        CheckCollisionWithCollectibles();
        CheckCollisionWithBoss();
        CheckCollisionWithHomingLasers();


        // shooting
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

    // triggers shooting
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

    //collision function for blocking obstalces
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

    //function for action when colliding with obstacles
    private void OnHitObstacle(Obstacle obstacle)
    {
        Debug.Log("Player hit obstacle: " + obstacle.name);

        transform.position = spawnPosition;
        velocity = Vector2.zero;
    }

    // collision function for colliding with collectables
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

    // function for what happens with colliding with collectables
    private void OnCollect(Collectible c)
    {
        score += scorePerCollectible;
        Debug.Log($"Collected {c.name}. Score = {score}");

        Destroy(c.gameObject);
    }

    //function for colliding with boss
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

    // function for what happend with collision with boss which is end game loop action
    private void OnHitBoss(Boss boss)
    {
        if (isGameOver) return;
        isGameOver = true;

        if (score >= boss.requiredScoreToDefeat)
        {
            Debug.Log("You defeated the boss! YOU WIN!");
            if (uiManager != null)
            {
                uiManager.ShowMessage("YOU WIN!");
            }
        }
        else
        {
            Debug.Log("Not enough score. Boss destroys you. GAME OVER.");
            if (uiManager != null)
            {
                uiManager.ShowMessage("YOU LOSE!");
            }
        }

        // Wait a bit so player can read the message, then reload
        StartCoroutine(ReloadAfterDelay(2f));
    }


    // reloads scene on death
    private void ReloadCurrentScene()
    {
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }


    // function for collision with homing laser from boss
    private void CheckCollisionWithHomingLasers()
    {
        for (int i = HomingLaser.AllLasers.Count - 1; i >= 0; i--)
        {
            HomingLaser laser = HomingLaser.AllLasers[i];
            if (laser == null) continue;

            float dist = Vector2.Distance(transform.position, laser.transform.position);

            if (dist <= collisionRadius + laser.radius)
            {
                OnHitByLaser(laser);
                break; // player is dead/reset, no need to check more
            }
        }
    }


    //function for what happens when hit with laser
    private void OnHitByLaser(HomingLaser laser)
    {
        Debug.Log("Player hit by homing laser!");

        // destroy the laser that hit us
        Destroy(laser.gameObject);

        // Same behaviour as hitting an obstacle for now:
        transform.position = spawnPosition;
        velocity = Vector2.zero;
    }

    private IEnumerator ReloadAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ReloadCurrentScene();
    }

}
