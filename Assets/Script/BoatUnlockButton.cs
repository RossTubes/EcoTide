using System.Collections;
using TMPro;
using UnityEngine;

public class BoatUnlockButton : MonoBehaviour
{
    public TextMeshProUGUI interactionText; // Assign in Inspector
    public GameObject BigBoat; // The big boat to unlock
    public GameObject SmallBoat; // The small boat to deactivate
    public Transform player; // Assign in Inspector
    public Transform button; // Assign in Inspector
    private bool BoatUnlocked = false; // Prevents UI updates after unlocking
    private bool isNearButton = false;
    public float interactionRange = 3f; // Distance required to interact
    public int moneyAmount; // Public variable to specify the amount of money to be deducted

    private void Start()
    {
        if (interactionText == null)
        {
            Debug.LogError("interactionText is NOT assigned in the Inspector!");
        }

        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(true);
            interactionText.text = ""; // Ensure UI is visible
        }

        if (BigBoat != null)
        {
            BigBoat.SetActive(false); // Hide big boat initially
        }

        if (SmallBoat != null)
        {
            SmallBoat.SetActive(true); // Ensure small boat is visible at the start
        }
    }

    private void Update()
    {
        if (interactionText == null || button == null || player == null)
        {
            Debug.LogError("Missing References! Make sure all public variables are assigned in the Inspector.");
            return;
        }

        if (BoatUnlocked) return; // Stop UI updates if house is unlocked

        // Check if the player is near the button
        isNearButton = Vector3.Distance(button.position, player.position) < interactionRange;
        Debug.Log("Is Near Button: " + isNearButton);
        Debug.Log("Interaction Text Active: " + interactionText.gameObject.activeSelf);

        if (isNearButton)
        {
            interactionText.gameObject.SetActive(true);
            interactionText.text = "Press F to Unlock Big Boat for $" + moneyAmount;

            // Check if player presses 'F'
            if (Input.GetKeyDown(KeyCode.F) && GameManager.Instance.Money >= moneyAmount && BigBoat != null)
            {
                if (GameManager.Instance.SpendMoney(moneyAmount)) // Deduct money
                {
                    BigBoat.SetActive(true);
                    interactionText.text = "Boat Unlocked!";
                    BoatUnlocked = true; // Prevents further updates

                    if (SmallBoat != null)
                    {
                        SmallBoat.SetActive(false); // Deactivate small boat
                    }

                    StartCoroutine(HideTextAndDestroyButton()); // Hide text, then destroy the button
                    Debug.Log("Is Near Button: " + isNearButton);
                    Debug.Log("Interaction Text Active: " + interactionText.gameObject.activeSelf);
                    Debug.Log("Boat Unlocked: " + BoatUnlocked);

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
