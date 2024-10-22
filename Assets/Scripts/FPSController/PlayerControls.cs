using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    [SerializeField]
    Transform Player;
    Camera cam;
    public CharacterController controller;
    public DigSpotTrigger currentDigZone;
    public GameObject EquipedTool;
    public GameObject HeldItem;

    public GameObject[] Tools;


    public enum PlayerState { Walking, Helm, Cannon }
    public LayerMask constantRayLayerMask;

    #region Mouse Vars
    [Header("Mouse")]
    [Range(0, 100)]
    public float mouseSense = 100f;
    public Vector2 mouseInput = Vector2.zero;
    float xRotation;
    #endregion

    #region Interaction Vars
    float interacting;
    bool ItemIsHeld;
    #endregion


    #region Move Vars
    [Header("Movement")]
    Vector2 DirectionInput;
    public float speed;
    public float jumpHeight;
    public float gravity = -9.81f;
    Vector3 velocity;
    [SerializeField]
    Transform GroundCheck;
    float groundDist = 0.2f;
    public LayerMask GroundMask;
    public bool isGrounded;
    float jump;
    #endregion


    private static PlayerControls _Instance;
    public static PlayerControls Instance
    {
        get
        {
            return _Instance;
        }
    }


    bool _isGrounded
    {
        get => isGrounded;
        set
        {
            if (value == false)
            {
                StartCoroutine(coyoteTime(value));
            }
            else
            {
                isGrounded = value;
            }
        }
    }

    IEnumerator coyoteTime(bool value)
    {
        yield return new WaitForSeconds(.2f);

        isGrounded = value;
    }

    private void Awake()
    {
        if (_Instance != null && _Instance != this)
        {
            Destroy(this);
        }
        else
        {
            _Instance = this;
        }



        cam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
    }


    void OnMove(InputValue value)
    {
        DirectionInput = value.Get<Vector2>();
    }

    void OnInteract(InputValue value)
    {
        interacting = value.Get<float>();
    }

    void OnLook(InputValue value)
    {

        mouseInput = value.Get<Vector2>();

    }

    void OnFire(InputValue value)
    {
        Debug.Log("Workin");
        if (currentDigZone != null)
        {
            currentDigZone.dig();
        }
    }

    void OnJump(InputValue value)
    {
        jump = value.Get<float>();
        _isGrounded = false;
        Debug.Log(value.Get<float>());

    }

    private void FixedUpdate()
    {
        ConstantRaycast();
    }

    private void ConstantRaycast()
    {
        if (!ItemIsHeld)
        {
            RaycastHit hit;
            if (Physics.SphereCast(cam.transform.position, 0.08f, Camera.main.transform.forward, out hit, 2.0f, constantRayLayerMask))
            {
                if (hit.transform.CompareTag("Chest"))
                {
                    TreasureScript Chest = hit.transform.GetComponent<TreasureScript>();
                    Chest.isObserved = true;
                    if (interacting == 1)
                    {
                        Chest.interacting = true;
                    }
                }

            }
        }
    }

    private void Update()
    {

        rotate();
        Move();
        Jump();
    }

    private void Jump()
    {

        _isGrounded = Physics.CheckSphere(GroundCheck.position, groundDist, GroundMask);
        if (jump > 0 && isGrounded)
        {

            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -1f;
        }
    }

    private void Move()
    {


        Vector3 move = (cam.transform.right * DirectionInput.x) + (cam.transform.forward * DirectionInput.y);
        move.y = 0f;
        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void rotate()
    {
        transform.localEulerAngles = new Vector3(0, cam.gameObject.transform.localEulerAngles.y, 0f);
    }



    void OnDrop(InputValue value)
    {
        HeldItem.transform.SetParent(null);
        if (!HeldItem.GetComponent<ItemDrop>().groundCheck())
        {
            HeldItem.transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);

        }
        HeldItem.transform.eulerAngles = new Vector3(0, HeldItem.transform.eulerAngles.y, 0);
        ItemIsHeld = false;
        EquipedTool.SetActive(true);

    }

    public void PickUp(GameObject pickupItem)
    {
        ItemIsHeld = true;
        if (EquipedTool.activeSelf == true) EquipedTool.SetActive(false);
        HeldItem = pickupItem;
        pickupItem.transform.SetParent(cam.transform);
        pickupItem.transform.localPosition = new Vector3(0, -.5f, 1f);
        pickupItem.transform.rotation = new Quaternion(0, 0, 0, 0);
    }


}
