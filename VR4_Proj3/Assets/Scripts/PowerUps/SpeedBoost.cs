using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    public float speedMultiplier = 2f; // Multiplier for player's walk speed
    public float duration = 5f; // Duration of speed boost
    private float originalSpeed;
    private bool boostActive = false;

    [SerializeField]
    private GameObject disableBoost = null;

    private MeshRenderer myRenderer;
    private CapsuleCollider myCollider;

    private void Start()
    {
        myRenderer = GetComponent<MeshRenderer>();
        myCollider = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        // rotate around and about
        transform.Rotate(transform.up * 60 * Time.deltaTime, Space.Self);
    }

    void OnCollisionEnter(Collision collision)
    {
        PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
        if (collision.gameObject.CompareTag("Player")) // If collided object is the player
        {
            Debug.Log("collide");
            if (!boostActive) // Check if boost is not already active
            {
                StartCoroutine(BoostSpeed(playerMovement));
                Debug.Log("Speed");
            }
            // "Delete" powerup
            myRenderer.enabled = false;
            myCollider.enabled = false;
        }


    }

    IEnumerator BoostSpeed(PlayerMovement playerMovement)
    {

        Debug.Log("Start Couroutine");
        boostActive = true;
        ActivateTurboSpeed(playerMovement);
        // playerController.walkSpeed += speedMultiplier; // Double the player's walk speed
        yield return new WaitForSeconds(duration);
        DeactivateTurboSpeed(playerMovement);
        // playerController.walkSpeed = 14; // Reset the player's walk speed 
        boostActive = false;
    }

    private void ActivateTurboSpeed(PlayerMovement playerMovement)
    {
        playerMovement.movementSpeed *= speedMultiplier;
    }

    private void DeactivateTurboSpeed(PlayerMovement playerMovement)
    {
        playerMovement.movementSpeed /= speedMultiplier;
    }
}