using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSoundTrigger : MonoBehaviour
{
    public AudioSource waterAudio;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure your player has the "Player" tag
        {
            if (!waterAudio.isPlaying)
            {
                waterAudio.Play();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            waterAudio.Stop();
        }
    }
}

