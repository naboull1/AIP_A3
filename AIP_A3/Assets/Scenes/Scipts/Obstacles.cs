using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Collision Settings")]
    [SerializeField] public float radius = 0.5f; // approximate half-size for simple circular collision

    // Keep track of all active obstacles in the scene
    public static List<Obstacle> AllObstacles = new List<Obstacle>();

    private void OnEnable()
    {
        if (!AllObstacles.Contains(this))
        {
            AllObstacles.Add(this);
        }
    }

    private void OnDisable()
    {
        AllObstacles.Remove(this);
    }

    private void OnDrawGizmosSelected()
    {
        // Just for debugging: shows the collision radius in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
