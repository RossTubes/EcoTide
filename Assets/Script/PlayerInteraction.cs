using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRange = 3f;
    public LayerMask Boat;
    private BoatController boat;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Collider[] boats = Physics.OverlapSphere(transform.position, interactionRange, Boat);
            if (boats.Length > 0)
            {
                BoatController boat = boats[0].GetComponentInParent<BoatController>(); // Search in parent objects
                if (boat != null)
                {
                    Debug.Log("Entering Boat: " + boat.gameObject.name);
                    boat.EnterBoat(gameObject);
                }
            }
        }
    }
}