using TMPro;
using UnityEngine;
using System.Collections;

public class ButtonCity : MonoBehaviour
{
    public TextMeshProUGUI interactionText; // Assign in Inspector
    public Transform player; // Assign the player in Inspector
    public Transform button; // Assign the button object in Inspector
    public float interactionRange = 3f; // Distance required to interact
    public GameObject City; // Use GameObject instead of Transform
    public GameObject border;

    private bool isNearButton = false;
    private bool cityUnlocked = false; // Prevents UI from updating after unlocking
    private void Start()
    {
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false); // Hide text initially
        }

        if (City != null)
        {
            City.SetActive(false); // Ensure city starts hidden
        }
    }

    private void Update()
    {
        if (interactionText == null || button == null || player == null)
        {
            Debug.LogError("Missing References! Make sure all public variables are assigned in the Inspector.");
            return;
        }

        if (cityUnlocked) return; // Stop UI updates if city is unlocked

        // Check if the player is near the button
        isNearButton = Vector3.Distance(button.position, player.position) < interactionRange;

        if (isNearButton)
        {
            interactionText.gameObject.SetActive(true);
            interactionText.text = "Press F to Unlock MoodapusCity for 500$";

            // Check if player presses 'F'
            if (Input.GetKeyDown(KeyCode.F) && GameManager.Instance.Money >= 500 && City != null)
            {
                if (GameManager.Instance.SpendMoney(500)) // Deduct money if enough
                {
                    City.SetActive(true);
                    interactionText.text = "City Unlocked!";
                    cityUnlocked = true; // Prevent UI updates

                    StartCoroutine(HideTextAndDestroyButton()); // Hide text, then destroy the button
                }
            }
        }
        else
        {
            interactionText.gameObject.SetActive(false);
        }
    }

    private IEnumerator HideTextAndDestroyButton()
    {
        yield return new WaitForSeconds(3f);

        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
            border.gameObject.SetActive(false);
        }

        // Destroy button AFTER the text disappears
        Destroy(button.gameObject);
        Destroy(gameObject); // Optional: Destroy this script's GameObject if necessary
    }
}
