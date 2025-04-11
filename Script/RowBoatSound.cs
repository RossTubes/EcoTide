using UnityEngine;

public class RowBoatSound : MonoBehaviour
{
    public AudioSource engineAudio;
    public Rigidbody boatRigidbody;
    public GameObject boatCam; // Reference to the boat camera
    public float speedThreshold = 0.1f; // Minimum speed before sound plays

    private void Update()
    {
        bool isPlayerInBoat = boatCam.activeSelf; // Check if boat camera is active
        float speed = boatRigidbody.velocity.magnitude;

        if (isPlayerInBoat && speed > speedThreshold)
        {
            if (!engineAudio.isPlaying)
            {
                engineAudio.Play();
            }
        }
        else
        {
            if (engineAudio.isPlaying)
            {
                engineAudio.Stop();
            }
        }
    }
}
