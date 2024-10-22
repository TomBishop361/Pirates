using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreasureScript : MonoBehaviour
{
    public bool isObserved = false;
    [SerializeField]
    Canvas UI;
    [SerializeField]
    Image ring;
    public bool interacting;

    private void FixedUpdate()
    {
        if (isObserved && UI.gameObject.activeSelf == false)
        {
            UI.gameObject.SetActive(true);
        }
        else if (!isObserved && UI.gameObject.activeSelf == true)
        {
            UI.gameObject.SetActive(false);
        }
        if (interacting)
        {
            ring.fillAmount += 1f * Time.deltaTime;
        }
        else if (!interacting)
        {
            ring.fillAmount = 0;
        }
        if (ring.fillAmount >= 1)
        {
            PlayerControls.Instance.PickUp(transform.gameObject);
        }


        interacting = false;
        isObserved = false;
    }
    private void LateUpdate()
    {

    }
}
