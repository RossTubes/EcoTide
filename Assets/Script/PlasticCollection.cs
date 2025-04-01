using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlasticCollection : MonoBehaviour
{
    public int Plastic = 0; // Make Plastic public so CarMover can access it
    public TextMeshProUGUI PlasticText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Plastic")) // Use CompareTag() for better performance
        {
            Plastic++;
            PlasticText.text = "Plastic: " + Plastic.ToString(); // Update text property
            Debug.Log("Plastic Collected: " + Plastic);
            Destroy(other.gameObject);
        }
    }

    // Method to reset plastic count
    public void ResetPlastic()
    {
        Plastic = 0;
        PlasticText.text = "Plastic: 0";
    }
}
