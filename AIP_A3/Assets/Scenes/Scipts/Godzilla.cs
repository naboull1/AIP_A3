using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("Collision Settings")]
    [SerializeField] public float radius = 1.0f;  // approximate boss size

    [Header("Win Condition")]
    [SerializeField] public int requiredScoreToDefeat = 50;

    [Header("Homing Laser Settings")]
    [SerializeField] private Transform player;             // assign in Inspector
    [SerializeField] private GameObject homingLaserPrefab; // assign in Inspector
    [SerializeField] private float fireInterval = 2.0f;    // seconds between shots
    [SerializeField] private float activationX = 20f;      // player.x at which boss starts firing

    private float fireTimer = 0f;
    private int shotsFired = 0;
    private bool hasActivated = false;

    // Keep track of all bosses for player collision logic
    public static List<Boss> AllBosses = new List<Boss>();

    private void OnEnable()
    {
        if (!AllBosses.Contains(this))
        {
            AllBosses.Add(this);
        }
    }

    private void OnDisable()
    {
        AllBosses.Remove(this);
    }

    private void Start()
    {
        Debug.Log("Boss Start() called");

        // Automatically calculate activation point based on boss location
        activationX = transform.position.x - 3f;
        Debug.Log($"Boss activationX automatically set to {activationX}");

        // Player assignment
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log("Boss found player by tag: " + player.name);
            }
            else
            {
                Debug.LogWarning("Boss could not find player with tag 'Player'");
            }
        }
        else
        {
            Debug.Log("Boss using player reference from Inspector: " + player.name);
        }
    }


    private void Update()
    {
        if (player == null)
            return;

        // Only start firing once the player reaches activationX
        if (player.position.x < activationX)
            return;

        fireTimer += Time.deltaTime;

        if (fireTimer >= fireInterval)
        {
            FireHomingLaser();
            fireTimer = 0f;
        }
    }



    private void FireHomingLaser()
    {
        if (homingLaserPrefab == null)
        {
            Debug.LogWarning("Boss has no HomingLaser prefab assigned!");
            return;
        }

        // Slight random offset so they don't stack perfectly
        Vector3 spawnPos = transform.position;
        spawnPos += new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), 0f);

        GameObject laserObj = Instantiate(homingLaserPrefab, spawnPos, Quaternion.identity);

        HomingLaser laser = laserObj.GetComponent<HomingLaser>();
        if (laser != null && player != null)
        {
            laser.Init(player);
        }

        shotsFired++;
        Debug.Log($"Boss fired homing laser #{shotsFired} at time {Time.time}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
