using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DigSpotTrigger : MonoBehaviour
{
    PlayerControls playerControls;
    public GameObject Treasure;
    GameObject InstantiatedTreasure;
    int digCount = 0;

    private void Start()
    {
        playerControls = PlayerControls.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerControls.currentDigZone = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerControls.currentDigZone = null;
        }
    }

    public void dig()
    {
        switch (digCount)
        {
            case 0:
                InstantiatedTreasure = Instantiate(Treasure, transform, false);
                InstantiatedTreasure.transform.localPosition = Vector3.zero;
                digCount++;
                break;
            case 1:
                InstantiatedTreasure.transform.localPosition += new Vector3(0, 0.25f, 0);
                digCount++;
                break;

            case 2:
                InstantiatedTreasure.transform.localPosition += new Vector3(0, 0.25f, 0);
                digCount++;
                break;
            case 3:
                InstantiatedTreasure.transform.localPosition = new Vector3(0, 1, 0);
                Destroy(this);
                break;
        }
    }

}
