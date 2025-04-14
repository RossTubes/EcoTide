using System.Collections;
using UnityEngine;
using TMPro; // For UI Text

public class CarMove : MonoBehaviour
{
    public Vector3 targetPosition; // Set the destination point in the Inspector
    private Vector3 startPosition;
    public float speed = 5f;
    private MeshRenderer[] meshRenderers;
    private bool isMoving = false;

    public Transform player; // Assign the player in the Inspector
    public Transform npc; // Assign the NPC (person sitting on the chair) in the Inspector
    public float interactionRange = 3f; // Distance required to interact

    public PlasticCollection plasticCollection; // Reference to first PlasticCollection script
    public PlasticCollection plasticCollectionBigNet; // Reference to second PlasticCollection script

    public TextMeshProUGUI interactionText; // UI Text to show messages
    private bool isNearNPC = false;
    private float messageTimer = 0f; // Timer to keep message visible
    private float messageDuration = 2f; // Time to show message before resetting

    private int storedPlastic = 0; // Keeps track of total plastic across trips

    public GameObject boxes; // Reference to the "Boxes" child object

    private void Start()
    {
        startPosition = transform.position;
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        interactionText.gameObject.SetActive(false); // Hide text initially

        if (boxes != null)
        {
            boxes.SetActive(false); // Ensure boxes start hidden
        }
        else
        {
            Debug.LogError("Boxes GameObject is not assigned! Please assign it in the Inspector.");
        }
    }

    private void Update()
    {
        // Check if the player is near the NPC
        isNearNPC = Vector3.Distance(npc.position, player.position) < interactionRange;

        if (isNearNPC && messageTimer <= 0)
        {
            interactionText.gameObject.SetActive(true);
            interactionText.text = "Press F to talk"; // Default message

            if (Input.GetKeyDown(KeyCode.F) && !isMoving)
            {
                HandlePlasticInteraction();
            }
        }
        else if (!isNearNPC) // Hide text when far away
        {
            interactionText.gameObject.SetActive(false);
        }

        // Reduce message timer
        if (messageTimer > 0)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0 && isNearNPC)
            {
                interactionText.text = "Press F to talk"; // Reset back after delay
            }
        }
    }

    private void HandlePlasticInteraction()
    {
        int collectedPlastic = (plasticCollection != null ? plasticCollection.Plastic : 0) +
                               (plasticCollectionBigNet != null ? plasticCollectionBigNet.Plastic : 0);

        storedPlastic += collectedPlastic; // Store the collected plastic
        ResetPlasticCounts(); // Reset the boat counters to 0

        // Add money for each delivered plastic box
        GameManager.Instance.AddMoney(storedPlastic * 10);

        // Activate boxes if more than 5 plastic was collected
        if (storedPlastic >= 5 && boxes != null)
        {
            boxes.SetActive(true);
        }

        if (storedPlastic == 0)
        {
            interactionText.text = "Come back with some plastic!";
        }
        else if (storedPlastic > 0 && storedPlastic < 10)
        {
            interactionText.text = $"Great! Keep it up! (Total: {storedPlastic}/10)";
        }
        else if (storedPlastic >= 10)
        {
            interactionText.text = "The car is leaving!";
            storedPlastic = 0; // Reset after car leaves
            StartCoroutine(MoveRoutine());
        }

        messageTimer = messageDuration; // Keep the message on screen
    }

    private void ResetPlasticCounts()
    {
        if (plasticCollection != null)
        {
            plasticCollection.ResetPlastic();
        }

        if (plasticCollectionBigNet != null)
        {
            plasticCollectionBigNet.ResetPlastic();
        }
    }

    private IEnumerator MoveRoutine()
    {
        isMoving = true;

        // Move to target position
        yield return StartCoroutine(MoveToPosition(targetPosition));

        // Disable MeshRenderers
        yield return new WaitForSeconds(3);

        // Turn around
        transform.Rotate(0, 180, 0);
        boxes.SetActive(false);

        // Move back to start position
        yield return StartCoroutine(MoveToPosition(startPosition));

        // Disable MeshRenderers again
        yield return new WaitForSeconds(3);

        // Turn around again
        transform.Rotate(0, 180, 0);

        isMoving = false;
    }

    private IEnumerator MoveToPosition(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
    }

    private void SetMeshRenderers(bool state)
    {
        foreach (MeshRenderer mr in meshRenderers)
        {
            mr.enabled = state;
        }
    }
}
