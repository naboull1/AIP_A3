using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Collision Settings")]
    [SerializeField] public float radius = 0.4f; // approx radius for pickup

    // Keep track of all active collectibles
    public static List<Collectible> AllCollectibles = new List<Collectible>();

    //add collectible to list
    private void OnEnable()
    {
        if (!AllCollectibles.Contains(this))
        {
            AllCollectibles.Add(this);
        }
    }

    private void OnDisable()
    {
        AllCollectibles.Remove(this);
    }

    //visual aid for radius in scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
