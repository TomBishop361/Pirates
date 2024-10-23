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
                InstantiatedTreasure = Instantiate(Treasure, null, false);
                InstantiatedTreasure.transform.position = transform.position + (Vector3.down * 0.5f) ;
                digCount++;
                break;
            case 1:
                InstantiatedTreasure.transform.position += (Vector3.up*0.25f);
                digCount++;
                break;

            case 2:
                InstantiatedTreasure.transform.position += (Vector3.up * 0.25f);
                digCount++;
                break;
            case 3:
                InstantiatedTreasure.transform.position += (Vector3.up * 0.25f);
                Destroy(this);
                break;
        }
    }

}
