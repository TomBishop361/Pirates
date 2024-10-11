using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignFloor : MonoBehaviour
{
    public Transform Ship;

    private void LateUpdate()
    {
        transform.position = Ship.position;
        transform.localRotation = Ship.rotation;
    }
}
