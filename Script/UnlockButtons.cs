using System.Collections;
using TMPro;
using UnityEngine;

public class UnlockButtons : MonoBehaviour
{
    public TextMeshProUGUI interactionText; // Assign in Inspector
    public GameObject House; // The house to unlock
    public Transform player; // Assign the player in Inspector
    public Transform button; // Assign the button object in Inspector
    private bool HouseUnlocked = false; // Prevents UI from updating after unlocking
    private bool isNearButton = false;
    public float interactionRange = 3f; // Distance required to interact
    public int moneyAmount; // Public variable to specify the amount of money to be deducted, editable in Inspector
    public string houseDescription = "House"; // Description of the house (editable in Inspector)

    private void Start()
    {
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false); // Hide text initially
        }

        if (House != null)
        {
            House.SetActive(false); // Ensure house starts hidden
        }
    }

    private void Update()
    {
        if (interactionText == null || button == null || player == null)
        {
            Debug.LogError("Missing References! Make sure all public variables are assigned in the Inspector.");
            return;
        }

        if (HouseUnlocked) return; // Stop UI updates if house is unlocked

        // Check if the player is near the button
        isNearButton = Vector3.Distance(button.position, player.position) < interactionRange;

        if (isNearButton)
        {
            interactionText.gameObject.SetActive(true);
            interactionText.text = $"Press F to Unlock {houseDescription} for {moneyAmount}$"; // Display dynamic money amount and house description

            // Check if player presses 'F'
            if (Input.GetKeyDown(KeyCode.F) && GameManager.Instance.Money >= moneyAmount && House != null)
            {
                if (GameManager.Instance.SpendMoney(moneyAmount)) // Deduct dynamic money amount
                {
                    House.SetActive(true);
                    interactionText.text = $"{houseDescription} Unlocked!"; // Change text to reflect the unlocked house
                    HouseUnlocked = true; // Prevent UI updates

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
        }

        // Destroy button AFTER the text disappears
        Destroy(button.gameObject);
        Destroy(gameObject); // Optional: Destroy this script's GameObject if necessary
    }
}
