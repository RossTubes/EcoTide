using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BoatController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float turnSpeed = 50f;
    private bool isDriving = false;
    public float interactionRange = 3f; // Distance required to interact
    private bool isNearBoat = false;
    public Transform boat;
    public Transform playerTF;
    public TextMeshProUGUI BoatInteractionText; // Assign in Inspector


    public GameObject playerCam;
    public GameObject boatCam;

    public Transform enterPoint;
    public Transform exitPoint;

    private Rigidbody rb;
    private GameObject player;
    private MeshRenderer playerMeshRenderer;

    public Vector3 startPosition;
    public Quaternion startRotation;

    [Header("Timer Settings")]
    public float maxRowTime;
    private float currentRowTime;
    private bool isTiming = false;

    [Header("Timer UI")]
    public TextMeshProUGUI timerText;
    public Image BackGroundTimer;
    public string timerLabel = "Stamina Left:";

    [Header("Explanation UI")]
    public TextMeshProUGUI explaInUI;
    public Image BackGroundExplain;

    [Header("Plastic Collection Reference")]
    public PlasticCollection plasticCollection;

    [Header("Boat Animation")]
    public Animator leftPaddleAnimator;
    public Animator rightPaddleAnimator;

    [Header("Boat Character Reference")]
    public GameObject boatCharacter;

    private Coroutine hideExplanationCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        startRotation = transform.rotation;

        if (timerText != null)
            timerText.text = "";

        if (BackGroundTimer != null)
            BackGroundTimer.enabled = false;

        if (explaInUI != null)
            explaInUI.text = "";

        if (BackGroundExplain != null)
            BackGroundExplain.enabled = false;

        if (boatCharacter != null)
            boatCharacter.SetActive(false); // Hide the boat character at start
    }

    void Update()
    {
        // Check if the player is near the boat
        isNearBoat = Vector3.Distance(boat.position, playerTF.position) < interactionRange;

        // Show interaction text only when the player is near the boat and not already driving
        if (isNearBoat && !isDriving)
        {
            BoatInteractionText.gameObject.SetActive(true);
            BoatInteractionText.text = "Press F to enter";
        }
        else
        {
            BoatInteractionText.gameObject.SetActive(false);
        }

        if (isDriving)
        {
            float moveInput = Input.GetAxis("Vertical");
            float turnInput = Input.GetAxis("Horizontal");

            transform.Translate(Vector3.forward * moveInput * moveSpeed * Time.deltaTime);
            transform.Rotate(Vector3.up * turnInput * turnSpeed * Time.deltaTime);

            // Paddle animation control
            bool isMoving = Mathf.Abs(moveInput) > 0.1f;

            if (leftPaddleAnimator != null)
                leftPaddleAnimator.SetBool("IsRowing", isMoving);

            if (rightPaddleAnimator != null)
                rightPaddleAnimator.SetBool("IsRowing", isMoving);

            // Timer logic
            if (isTiming)
            {
                currentRowTime -= Time.deltaTime;

                if (timerText != null)
                    timerText.text = timerLabel + " " + Mathf.CeilToInt(currentRowTime).ToString();

                if (currentRowTime <= 0f)
                {
                    Debug.Log("Time's up! Forcing exit with penalty.");
                    ForceExitWithPenalty();
                }
            }

            // Exit control
            if (Input.GetKeyDown(KeyCode.F))
            {
                ExitBoat();
            }
        }
    }

    public void EnterBoat(GameObject playerObj)
    {
        player = playerObj;

        playerCam.SetActive(false);
        boatCam.SetActive(true);

        playerMeshRenderer = player.GetComponentInChildren<MeshRenderer>();
        if (playerMeshRenderer != null)
            playerMeshRenderer.enabled = false;

        player.SetActive(false);

        isDriving = true;
        currentRowTime = maxRowTime;
        isTiming = true;

        if (boatCharacter != null)
            boatCharacter.SetActive(true);

        if (timerText != null)
            timerText.text = timerLabel + " " + Mathf.CeilToInt(maxRowTime);

        if (BackGroundTimer != null)
            BackGroundTimer.enabled = true;

        if (explaInUI != null)
            explaInUI.text = "Use WASD to row and F to exit the boat.";

        if (BackGroundExplain != null)
            BackGroundExplain.enabled = true;

        if (hideExplanationCoroutine != null)
            StopCoroutine(hideExplanationCoroutine);

        hideExplanationCoroutine = StartCoroutine(HideExplanationUIAfterDelay(5f));

        Debug.Log("Player entered boat. Timer and explanation UI shown.");
    }

    public void ExitBoat()
    {
        if (player == null)
        {
            Debug.LogWarning("ExitBoat called, but player is null.");
            return;
        }

        isDriving = false;
        isTiming = false;

        playerCam.SetActive(true);
        boatCam.SetActive(false);
        player.SetActive(true);

        if (playerMeshRenderer != null)
            playerMeshRenderer.enabled = true;

        if (boatCharacter != null)
            boatCharacter.SetActive(false);

        ResetBoat();

        if (timerText != null)
            timerText.text = "";

        if (BackGroundTimer != null)
            BackGroundTimer.enabled = false;

        if (explaInUI != null)
            explaInUI.text = "";

        if (BackGroundExplain != null)
            BackGroundExplain.enabled = false;

        if (hideExplanationCoroutine != null)
        {
            StopCoroutine(hideExplanationCoroutine);
            hideExplanationCoroutine = null;
        }

        if (leftPaddleAnimator != null)
            leftPaddleAnimator.SetBool("IsRowing", false);

        if (rightPaddleAnimator != null)
            rightPaddleAnimator.SetBool("IsRowing", false);

        player = null;
    }

    private void ForceExitWithPenalty()
    {
        if (player == null)
        {
            Debug.LogWarning("ForceExitWithPenalty called but player is null.");
            return;
        }

        isDriving = false;
        isTiming = false;

        playerCam.SetActive(true);
        boatCam.SetActive(false);
        player.SetActive(true);

        if (playerMeshRenderer != null)
            playerMeshRenderer.enabled = true;

        if (boatCharacter != null)
            boatCharacter.SetActive(false);

        ResetBoat();

        if (plasticCollection != null)
        {
            plasticCollection.ResetPlastic();
        }

        if (timerText != null)
            timerText.text = "";

        if (BackGroundTimer != null)
            BackGroundTimer.enabled = false;

        if (explaInUI != null)
            explaInUI.text = "";

        if (BackGroundExplain != null)
            BackGroundExplain.enabled = false;

        if (hideExplanationCoroutine != null)
        {
            StopCoroutine(hideExplanationCoroutine);
            hideExplanationCoroutine = null;
        }

        if (leftPaddleAnimator != null)
            leftPaddleAnimator.SetBool("IsRowing", false);

        if (rightPaddleAnimator != null)
            rightPaddleAnimator.SetBool("IsRowing", false);

        Debug.Log("Penalty applied: plastic reset due to timeout.");
        player = null;
    }

    private void ResetBoat()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private IEnumerator HideExplanationUIAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (explaInUI != null)
            explaInUI.text = "";

        if (BackGroundExplain != null)
            BackGroundExplain.enabled = false;

        hideExplanationCoroutine = null;
    }
}
