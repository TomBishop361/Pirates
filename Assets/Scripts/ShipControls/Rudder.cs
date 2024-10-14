using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rudder : MonoBehaviour
{
    [SerializeField]
    Rigidbody Ship;

    [SerializeField]
    [Range(-0.5f,0.5f)]
    float torque = 0.5f;

    [SerializeField]
    float speed = 10;

    public void ChangeTorque()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Ship.AddTorque(new Vector3(0f, torque, 0f),ForceMode.Force);
        Ship.AddRelativeForce(new Vector3(-speed, 0, 0),ForceMode.Force);
     
    }
}
