using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Helm : MonoBehaviour
{

    public Transform Ship;
    float rotateAmount;
    Rigidbody test;
    


    public void rotate(Vector2 dir)
    {
       
        Mathf.Clamp(transform.rotation.y, 360, -360);
        
        transform.Rotate(new Vector3(0, dir.x, 0) * Time.deltaTime * 20);
        rotateAmount += dir.x;

                

    }
    
}
