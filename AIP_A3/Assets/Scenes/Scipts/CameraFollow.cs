using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;      // socket to place camera here
    [SerializeField] private float followSpeed = 5f;// speed to keep up with the player
    [SerializeField] private float xOffset = 2f;    // how far ahead of the player to look
    [SerializeField] private bool lockY = true;     // keep Y fixed?

    private float initialY;
    private float initialZ;

    private void Start()
    {
        // Remember starting Y/Z so we don't drift off
        initialY = transform.position.y;
        initialZ = transform.position.z;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Desired camera position
        float targetX = target.position.x + xOffset;
        float targetY = lockY ? initialY : target.position.y;
        //move camera
        Vector3 desiredPos = new Vector3(targetX, targetY, initialZ);

        // Smooth follow
        transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);
    }
}
