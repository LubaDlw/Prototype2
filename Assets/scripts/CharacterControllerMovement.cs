using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerMovement : MonoBehaviour
{
    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 100f;
    public Transform cameraTransform;
    private float xRotation = 0f;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    private Vector3 velocity;
    private CharacterController controller;
    private bool isGrounded;

    [Header("Sprint & Crouch")]
    public float sprintSpeed = 8f;
    public float crouchSpeed = 2f;
    public float crouchHeight = 1f;
    private float originalHeight;
    private Vector3 originalCenter;
    private bool isSprinting = false;
    private bool isCrouching = false;

    public AudioClip walkClip; // Assign this from the Inspector
    private AudioSource audioSource;

    void Start()
    {
      
            controller = GetComponent<CharacterController>();
            originalHeight = controller.height;
            originalCenter = controller.center;

            // Get the AudioSource attached to the player
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = walkClip;
            audioSource.loop = true;
            audioSource.playOnAwake = false;

            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
        

    }

    void Update()
    {
        HandleMouseLook();
      //  HandleSprint(); // Must come before movement so speed is updated
        HandleCrouch();
        HandleMovement();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        // Determine if the player is attempting to move
        bool isMoving = move.magnitude > 0.1f;

        // Determine current speed
        float currentSpeed = moveSpeed;
        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftShift) && isMoving && !isCrouching)
        {
            currentSpeed = sprintSpeed;
        }

        controller.Move(move * currentSpeed * Time.deltaTime);

        if (isMoving)
        {
            Debug.Log("Calling Audio Source");
            // Check if the audio is not already playing to avoid restarting the clip repeatedly
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            // Stop the audio if the player stops moving
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }

        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }



    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = true;
            controller.height = crouchHeight;
            controller.center = new Vector3(originalCenter.x, crouchHeight / 2f, originalCenter.z);
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isCrouching = false;
            controller.height = originalHeight;
            controller.center = originalCenter;
        }
    }
}
