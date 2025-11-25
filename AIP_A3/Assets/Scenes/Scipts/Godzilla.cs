using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("Collision Settings")]
    [SerializeField] public float radius = 1.0f;  // approximate boss size

    //amount of collectibles needed to win against the boss
    [Header("Win Condition")]
    [SerializeField] public int requiredScoreToDefeat = 50;

    
    [Header("Homing Laser Settings")]
    [SerializeField] private Transform player;             // slot for player
    [SerializeField] private GameObject homingLaserPrefab; // slot for laster prefab
    [SerializeField] private float fireInterval = 2.0f;    // how often boss shoots lasers
    [SerializeField] private float activationX = 20f;      // distance of player.x at which boss starts firing

    //fire time and number of shots fired for stats
    private float fireTimer = 0f;
    private int shotsFired = 0;
    private bool hasActivated = false;

    // keep track of all bosses for player collision logic
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

        // Player assignment debugging and checking 
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

        // fires when player is in range
        if (player.position.x < activationX)
            return;

        fireTimer += Time.deltaTime;

        if (fireTimer >= fireInterval)
        {
            FireHomingLaser();
            fireTimer = 0f;
        }
    }


    //function for firing lasers
    private void FireHomingLaser()
    {
        if (homingLaserPrefab == null)
        {
            Debug.LogWarning("Boss has no HomingLaser prefab assigned!");
            return;
        }
        //position calculations
        Vector3 spawnPos = transform.position;
        spawnPos += new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), 0f);
        //spawns a laser
        GameObject laserObj = Instantiate(homingLaserPrefab, spawnPos, Quaternion.identity);

        HomingLaser laser = laserObj.GetComponent<HomingLaser>();
        if (laser != null && player != null)
        {
            laser.Init(player);   // ⬅ this is the ONLY target assignment now
        }
        //updates shot count
        shotsFired++;
        Debug.Log($"Boss fired homing laser #{shotsFired} at time {Time.time}");
    }

}
