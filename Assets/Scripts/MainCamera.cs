using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    private Vector3 velocity = Vector3.zero;
    private float fixedZ;

    void Start()
    {
        // Устанавливаем фиксированную координату Z
        fixedZ = transform.position.z;
    }

    void FixedUpdate()
    {
        Vector3 playerPosition = player.position + offset;
        playerPosition.z = fixedZ; // Фиксация оси Z

        Vector3 desiredPosition = new Vector3(playerPosition.x, playerPosition.y, fixedZ);
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed * Time.fixedDeltaTime);
        transform.position = smoothedPosition;
    }
}
