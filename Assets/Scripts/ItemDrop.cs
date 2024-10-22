using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    public bool groundCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - hit.distance, transform.position.z);
            transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            return true;
        }
        else
        {
            return false;
        }
        
    }
}
