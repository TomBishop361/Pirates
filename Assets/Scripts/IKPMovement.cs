using System.Collections;
using UnityEngine;

public class IKPMovement : MonoBehaviour
{
    public GameObject[] StaticTargets;
    public GameObject[] IKTargets;
    [SerializeField]
    LayerMask layerMask;
    //Vector3 HitVec3;
   

    private void FixedUpdate()
    {
        LegMovement();
        BodyTilt();
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
        float height = avg ;

        offSet = new Vector3(transform.position.x, height, transform.position.z);
        transform.position = offSet;
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y , -Diff*30);

    }



    void LegMovement()
    {
        for (int i = 0; i < StaticTargets.Length; i++)
        {
            Vector3 HitVec3;
            RaycastHit hit;
            if (Physics.Raycast(StaticTargets[i].transform.position, Vector3.down, out hit, Mathf.Infinity, layerMask))
            {
                HitVec3 = hit.point;                
                if (Vector3.Distance(HitVec3, IKTargets[i].transform.position) > Random.Range(.75f, 1.75f))
                {

                    StartCoroutine(Move(IKTargets[i], HitVec3));
                }
            }
        }
    }

    private IEnumerator Move(GameObject IKTarget, Vector3 HitVec3)
    {
        float time = 0;
        Vector3 IKTargetPos = IKTarget.transform.position;
        while (time < 1f)
        {            
            time += Time.deltaTime * 10;
            IKTarget.transform.position = Vector3.Lerp(IKTarget.transform.position, HitVec3, time);
            yield return null;
        }


    }
}
