using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floating : MonoBehaviour
{
    public Rigidbody rigidBody;
    public float depthBeforeSubmerge = 1f;
    public float displacementAmount = 3f;
    public int floaterCount = 1;
    public float waterDrag = 0.99f;
    public float waterAnglerDrag = 0.5f;

    private WaveManager currentWaveManager;

    private void FixedUpdate()
    {
        // Find the closest water plane dynamically
        currentWaveManager = GetClosestWaveManager();

        if (currentWaveManager == null) return; // Prevent errors if no water is found

        rigidBody.AddForceAtPosition(Physics.gravity / floaterCount, transform.position, ForceMode.Acceleration);

        float waveHeight = currentWaveManager.GetWaveHeight(transform.position.x); // Use the closest water's wave height
        if (transform.position.y < waveHeight)
        {
            float displacementMultiplier = Mathf.Clamp01((waveHeight - transform.position.y) / depthBeforeSubmerge) * displacementAmount;
            rigidBody.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0f), transform.position, ForceMode.Acceleration);
            rigidBody.AddForce(displacementMultiplier * -rigidBody.velocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
            rigidBody.AddTorque(displacementMultiplier * -rigidBody.angularVelocity * waterAnglerDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }

    private WaveManager GetClosestWaveManager()
    {
        WaveManager[] waveManagers = FindObjectsOfType<WaveManager>();
        WaveManager closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (WaveManager waveManager in waveManagers)
        {
            // Get the world position of the water plane
            Vector3 waterPosition = waveManager.transform.position;

            // Get the horizontal distance between the boat and the water (ignoring Y)
            float distance = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                                              new Vector3(waterPosition.x, 0, waterPosition.z));

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = waveManager;
            }
        }
        return closest;
    }
}