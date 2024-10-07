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
    Vector2 mouseInput;
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
        cam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
    }

    
    void OnMove(InputValue value)
    {
        DirectionInput = value.Get<Vector2>();
        Debug.Log(DirectionInput);
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
        cameraRotation();
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
            velocity.y = -2f;
        }
    }

    private void Move()
    {


        Vector3 move = (transform.right * DirectionInput.x) + (transform.forward * DirectionInput.y);

        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    private void cameraRotation()
    {
        mouseInput.x = mouseInput.x * mouseSense * Time.deltaTime;
        mouseInput.y = mouseInput.y * mouseSense * Time.deltaTime;

        xRotation -= mouseInput.y;
        xRotation = Mathf.Clamp(xRotation, -80, 80);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        Player.Rotate(Vector3.up * mouseInput.x);
    }
}
