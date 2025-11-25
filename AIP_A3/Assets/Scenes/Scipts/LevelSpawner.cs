using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;           // socket for player transform position
    [SerializeField] private Transform boss;             // socket for boss transform position
    [SerializeField] private GameObject obstaclePrefab; // socket for obstacles 
    [SerializeField] private GameObject collectiblePrefab;  //socket for collectibles 

    [Header("Spawning Ahead of Player")]
    [SerializeField] private float firstSpawnOffsetX = 3f;   // how far in front of the player the first stuff appears avoids start crashes
    [SerializeField] private float spawnStepX = 2f;          // distance between spawn "columns"
    [SerializeField] private float spawnAheadDistance = 5f;  // how far ahead of the player we keep spawning
    [SerializeField] private float minY = -2.5f;
    [SerializeField] private float maxY = 2.5f;
    [SerializeField] private int obstaclesPerStep = 1;       // how many obstacles per column
    [SerializeField] private int collectiblesPerStep = 1;    // how many collectibles per column

    [Header("Boss Timing")]
    [SerializeField] private float timeToBoss = 20f;     // seconds until boss appears, changing this determines how long each round goes for
    [SerializeField] private float bossSpawnAheadX = 3f; // boss appears this far in front of player when time is up

    [Header("Safety")]
    [SerializeField] private float minDistanceFromPlayer = 1.5f; // don't spawn stuff right on top of player

    private float nextSpawnX;
    private float timer = 0f;
    private bool bossSpawned = false;


    //start function to check everythign is in order
    private void Start()
    {
        if (player == null)
        {
            Debug.LogWarning("LevelSpawner: player not assigned!");
            enabled = false;
            return;
        }

        if (boss == null)
        {
            Debug.LogWarning("LevelSpawner: boss not assigned!");
            enabled = false;
            return;
        }

        if (obstaclePrefab == null || collectiblePrefab == null)
        {
            Debug.LogWarning("LevelSpawner: prefabs not assigned!");
            enabled = false;
            return;
        }

        // Start spawning a bit in front of the player
        nextSpawnX = player.position.x + firstSpawnOffsetX;

        // keep boss hidden until time is triggered
        boss.gameObject.SetActive(false);
    }

    //keep spawning collectbles and obstacles until time is reached and then boss spawns
    private void Update()
    {
        float dt = Time.deltaTime;
        timer += dt;

        // If boss already spawned, stop spawning new stuff
        if (bossSpawned)
            return;

        //Spawn new columns of obstacles/collectibles as player moves right
        float playerX = player.position.x;

        while (playerX + spawnAheadDistance >= nextSpawnX)
        {
            SpawnColumn(nextSpawnX);
            nextSpawnX += spawnStepX;
        }

        // 2. After timeToBoss seconds, spawn/activate the boss in front of the player and stop spawning
        if (timer >= timeToBoss)
        {
            SpawnBossInFront();
            bossSpawned = true;
        }
    }


    //randomly spawns objects along a column line
    private void SpawnColumn(float x)
    {
        // Spawn obstacles
        for (int i = 0; i < obstaclesPerStep; i++)
        {
            Vector3 pos;
            int attempts = 0;

            do
            {
                float y = Random.Range(minY, maxY);
                pos = new Vector3(x, y, 0f);
                attempts++;
                if (attempts > 10) break;
            }
            while (Vector2.Distance(pos, player.position) < minDistanceFromPlayer);

            Instantiate(obstaclePrefab, pos, Quaternion.identity);
        }

        // Spawn collectibles
        for (int i = 0; i < collectiblesPerStep; i++)
        {
            Vector3 pos;
            int attempts = 0;

            do
            {
                float y = Random.Range(minY, maxY);
                pos = new Vector3(x, y, 0f);
                attempts++;
                if (attempts > 10) break;
            }
            while (Vector2.Distance(pos, player.position) < minDistanceFromPlayer);

            Instantiate(collectiblePrefab, pos, Quaternion.identity);
        }
    }


    
    private void SpawnBossInFront()
    {
        // Place boss a bit in front of the player and activate it
        Vector3 pos = boss.position;
        pos.x = player.position.x + bossSpawnAheadX;
        boss.position = pos;

        boss.gameObject.SetActive(true);

        Debug.Log($"Boss spawned in front of player at x={boss.position.x}");

        // Boss.Start() will run now (on first activation), and your Boss script will set activationX etc based on its position
    }
}
