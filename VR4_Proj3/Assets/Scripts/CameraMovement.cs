using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform player;
    public float rotationSpeed = 5.0f;

    public float maxRadius = 2.5f;
    public float inputX = 0;

    private Vector3 camDistance;
    private Vector3 camDir;
    private float currentRad = 2.5f;

    private RaycastHit hit;

    private void Start()
    {
        // makes sure the cursor is locked in the middle of the screen and is locked
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("Player object not assigned!");
            return;
        }

        Vector3 start = new Vector3(player.position.x, transform.position.y, player.position.z);
        Vector3 end = transform.position;

        camDistance = end - start;
        camDir = camDistance.normalized;


        currentRad = camDistance.magnitude;

        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        // Check if camera hits a wall
        if (Physics.Raycast(start, camDir, out hit, maxRadius, layerMask))
        {
            //transform.position -= camDir * 0.1f;
            transform.position = hit.point - (camDir * 0.1f);
        } else
        {
            transform.position = start + (camDir * maxRadius);
        }
        /*
        Ray ray = new Ray(player.position, camDir);

        if (Physics.Raycast(ray, out hit, 100))
            Debug.DrawLine(ray.origin, hit.point);
        */

        transform.RotateAround(player.position, player.up, inputX * rotationSpeed);
        transform.LookAt(start);
        //transform.RotateAround(player.position, player.right, mouseY);
    }

    private void OnDrawGizmos()
    {
        Vector3 start = new Vector3(player.position.x, transform.position.y, player.position.z);
        Vector3 end = transform.position;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(start, start + (camDir * currentRad));

        Gizmos.DrawSphere(hit.point, 0.15f);
    }
}