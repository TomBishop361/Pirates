using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    [SerializeField]
    Transform Player;
    Camera cam;
    public CharacterController controller;

    #region Mouse Vars
    [Header("Mouse")]
    [Range(0,100)]
    public float mouseSense = 100f;
    public Vector2 mouseInput = Vector2.zero;
    float xRotation;
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
    float groundDist = 0.1f;
    public LayerMask GroundMask;
    bool isGrounded;
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
        if(_Instance != null && _Instance != this)
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

    void OnLook(InputValue value)
    {
        
        mouseInput = value.Get<Vector2>();

    }

    void OnJump(InputValue value)
    {
        jump = value.Get<float>();
        Debug.Log(value.Get<float>());

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

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -0f;
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

}
