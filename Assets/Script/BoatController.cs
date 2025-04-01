using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float turnSpeed = 50f;
    private bool isDriving = false;

    public GameObject playerCam;
    public GameObject boatCam;

    public Transform enterPoint;
    public Transform exitPoint;

    private Rigidbody rb;
    private GameObject player;
    private PlayerMovement movementScript;
    private Rigidbody playerRb;
    private MeshRenderer playerMeshRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isDriving)
        {
            float moveInput = Input.GetAxis("Vertical"); // W/S or Up/Down
            float turnInput = Input.GetAxis("Horizontal"); // A/D or Left/Right

            // Move and rotate the boat manually
            transform.Translate(Vector3.forward * moveInput * moveSpeed * Time.deltaTime);
            transform.Rotate(Vector3.up * turnInput * turnSpeed * Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.F)) // Press F to exit
            {
                ExitBoat();
            }
        }
    }

    public void EnterBoat(GameObject playerObj)
    {
        player = playerObj; // Store player reference

        // Switch to boat camera
        playerCam.SetActive(false);
        boatCam.SetActive(true);

        // Disable the player's capsule MeshRenderer (optional for hiding player)
        playerMeshRenderer = player.GetComponentInChildren<MeshRenderer>();
        if (playerMeshRenderer != null)
        {
            playerMeshRenderer.enabled = false;
        }

        // Disable the player GameObject completely
        player.SetActive(false);

        isDriving = true;
        Debug.Log("Player entered the boat.");
    }

    public void ExitBoat()
    {
        if (player == null)
        {
            Debug.LogWarning("ExitBoat called, but player reference is null!");
            return;
        }

        isDriving = false;

        // Switch back to player camera
        playerCam.SetActive(true);
        boatCam.SetActive(false);

        // Enable the player GameObject
        player.SetActive(true);

        // Re-enable the MeshRenderer (if previously disabled)
        if (playerMeshRenderer != null)
        {
            playerMeshRenderer.enabled = true;
        }

        player = null; // Clear reference
    }
}