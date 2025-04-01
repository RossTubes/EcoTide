using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasticSpawner : MonoBehaviour
{
    public GameObject plasticBoxPrefab; // Assign in Inspector
    public Transform waterArea; // Assign the water GameObject
    public int maxBoxes = 25; // Max boxes at a time
    public float spawnInterval = 1f; // Delay between spawns
    public LayerMask waterLayer; // Assign "Water" LayerMask in Inspector

    private BoxCollider waterBounds;

    void Start()
    {
        if (waterArea == null)
        {
            Debug.LogError("Water area is not assigned!");
            return;
        }

        waterBounds = waterArea.GetComponent<BoxCollider>();
        if (waterBounds == null)
        {
            Debug.LogError("Water area must have a BoxCollider!");
            return;
        }

        StartCoroutine(SpawnBoxes());
    }

    IEnumerator SpawnBoxes()
    {
        while (true) // Keep checking for available spawn slots
        {
            int currentBoxCount = GameObject.FindGameObjectsWithTag("Plastic").Length;

            if (currentBoxCount < maxBoxes)
            {
                Vector3 spawnPosition = GetValidSpawnPosition();
                if (spawnPosition != Vector3.zero)
                {
                    SpawnBox(spawnPosition);
                }
                else
                {
                    Debug.LogWarning("Could not find a valid water position. Retrying...");
                }
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnBox(Vector3 position)
    {
        GameObject newBox = Instantiate(plasticBoxPrefab, position, Quaternion.identity);
        newBox.tag = "Plastic"; // Ensure the box has the correct tag
    }

    Vector3 GetValidSpawnPosition()
    {
        for (int i = 0; i < 10; i++) // Try multiple attempts to find a valid position
        {
            float x = Random.Range(waterBounds.bounds.min.x, waterBounds.bounds.max.x);
            float z = Random.Range(waterBounds.bounds.min.z, waterBounds.bounds.max.z);
            Vector3 rayStart = new Vector3(x, waterBounds.bounds.max.y + 1f, z); // Start above the water

            // Cast a ray downwards to detect the water surface
            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 5f, waterLayer))
            {
                return hit.point + Vector3.up * 0.5f; // Spawn slightly above the water
            }
        }

        return Vector3.zero; // Return zero if no valid position is found
    }
}
