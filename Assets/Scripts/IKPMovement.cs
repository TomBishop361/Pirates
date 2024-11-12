using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Rendering;


public class IKPMovement : MonoBehaviour
{
    public float bodyHeight = 0.15f;    
    public GameObject[] TargetProjection;
    public GameObject[] IKTargets;
    public Transform bodyTarget;
    public float[] FootTiming = new float[8];
    [SerializeField]
    LayerMask layerMask;
    List<float> HitList = new List<float>();
    [Range(0f,1)]
    public float StepTime = 1f;
    public float StepDistance = 1f;
    public float stepIntervals = 0.05f;


    
    Vector3 PreviousWorldLocation;
    
    public Vector3 Velocity;
    

    private void Start()
    {        
        PreviousWorldLocation = transform.position;

        for (int i = 0; i < FootTiming.Length; i++)
        {
            FootTiming[i] = i*stepIntervals;

        }

    }

    private void Update()
    {
        CalculateVelocity();
        LegMovement();
        FootTime();
        BodyMove();
        
        

    }

    void FootTime()
    {
        for (int i = 0; i < FootTiming.Length; i++)
        {
            FootTiming[i] += Time.deltaTime;
            
        }
    }

    void CalculateVelocity()
    {
        
        if(transform.position != PreviousWorldLocation)
        {

            Velocity = Vector3.Lerp(Velocity, (transform.position - PreviousWorldLocation) / Time.deltaTime, 0.1f);
            
        }
        else
        {
            Velocity = Vector3.Lerp(Velocity, Vector3.zero,1);
            
        }       
        
        PreviousWorldLocation = transform.position;
        
    }

    void BodyMove()
    {
        RaycastHit hit;
        Debug.DrawRay(bodyTarget.position + (Vector3.up*5), bodyTarget.transform.TransformDirection(Vector3.down) * 7);
        if (Physics.Raycast(bodyTarget.position + (Vector3.up * 5), bodyTarget.transform.TransformDirection(Vector3.down), out hit, 10f, layerMask)){
            
            transform.position = hit.point + (bodyTarget.transform.TransformDirection(Vector3.up) * bodyHeight);
        }
    }

    void BodyTilt()
    {
        Vector3 offSet = Vector3.zero;
        //LeftAvg
        float LSumHeight = 0;
        for (int i = 3; i < 7; i++)
        {
            LSumHeight += IKTargets[i].transform.position.y;

        }
        LSumHeight = LSumHeight * 0.25f;



        float RSumHeight = 0;
        for (int i = 0; i < 3; i++)
        {
            RSumHeight += IKTargets[i].transform.position.y;

        }
        RSumHeight = RSumHeight * 0.25f;

        float Diff = RSumHeight - LSumHeight;
        float avg = (RSumHeight + LSumHeight) * 0.5f;
        float height = avg + 0.15f;


        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y , -Diff*30);
        //transform.position = new Vector3(transform.position.x, avg, transform.position.z);
    }



    void LegMovement()
    {
         
        for (int i = 0; i < IKTargets.Length; i++)
        {
            Debug.DrawRay(TargetProjection[i].transform.position + (Vector3.up * 2), (Vector3.down + Vector3.ClampMagnitude(Velocity, StepDistance)) * 1000, Color.green);

            if (FootTiming[i] >= StepTime)
            {
              
                RaycastHit hit;
                if (Physics.Raycast(TargetProjection[i].transform.position, Vector3.down + Vector3.ClampMagnitude(Velocity,StepDistance), out hit, Mathf.Infinity, layerMask))
                {
                    Debug.DrawRay(TargetProjection[i].transform.position + (Vector3.up * 2), (Vector3.down + Vector3.ClampMagnitude(Velocity, StepDistance)) * hit.distance, Color.red);
                    IKTargets[i].transform.position = Vector3.Lerp(IKTargets[i].transform.position, hit.point, RemapClamped(FootTiming[i], 0, 0.2f, 0, 1));
                }
                FootTiming[i] = 0;
            }
            
        }
        

    }

    public static float RemapClamped(float aValue, float aIn1, float aIn2, float aOut1, float aOut2)
    {
        float t = (aValue - aIn1) / (aIn2 - aIn1);
        t = Mathf.Clamp(t,0,1);
        return aOut1 + (aOut2 - aOut1) * t;
    }


    private void OnValidate()
    {
        if(StepTime < IKTargets.Length * stepIntervals)
        {
            StepTime = IKTargets.Length * stepIntervals + 0.05f;
        }
    }
}
