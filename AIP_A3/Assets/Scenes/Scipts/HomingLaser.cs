using System.Collections.Generic;
using UnityEngine;


public class HomingLaser : MonoBehaviour
{
    [Header("Homing Settings")]
    [SerializeField] private float speed = 12f;          // how fast it laser moves
    [SerializeField] private float turnSpeed = 4f;       // how quickly it turns towards the player
    [SerializeField] public float radius = 0.3f;         // size for collision

    [Header("Lifetime")]
    [SerializeField] private float lifetime = 5f;        // destroy laser after this many seconds

    private Transform target;       // socket to chase player
    private Vector2 direction;      // current movement direction (normalized)
    private float timer = 0f;       
    private bool initialized = false;

    public static List<HomingLaser> AllLasers = new List<HomingLaser>();

    private void OnEnable()
    {
        if (!AllLasers.Contains(this))
            AllLasers.Add(this);
    }

    private void OnDisable()
    {
        AllLasers.Remove(this);
    }



    //setups target
    public void Init(Transform newTarget)
    {
        target = newTarget;
        initialized = true;

        if (target != null)
        {
            Vector2 toTarget = (Vector2)(target.position - transform.position);
            if (toTarget.sqrMagnitude > 0.001f)
                direction = toTarget.normalized;
            else
                direction = Vector2.right;
        }
        else
        {
            Debug.LogWarning("HomingLaser.Init called with null target!");
            direction = Vector2.right;
        }
    }

    //update laser postions every tick
    private void Update()
    {
        float dt = Time.deltaTime;

        if (!initialized || target == null)
        {
            // No valid target, just move forward in whatever direction we have
            Vector3 pos = transform.position;
            pos += (Vector3)(direction * speed * dt);
            transform.position = pos;

            timer += dt;
            if (timer >= lifetime)
                Destroy(gameObject);

            return;
        }

        //Steer the direction towards the target
        Vector2 toTarget = (Vector2)(target.position - transform.position);
        if (toTarget.sqrMagnitude > 0.001f)
        {
            Vector2 desiredDir = toTarget.normalized;
            direction = Vector2.Lerp(direction, desiredDir, turnSpeed * dt).normalized;
        }

        //Move along the current direction
        Vector3 newPos = transform.position;
        newPos += (Vector3)(direction * speed * dt);
        transform.position = newPos;

        //Rotate sprite to face movement direction
        if (direction.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        // destroy after lifetime reached
        timer += dt;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
