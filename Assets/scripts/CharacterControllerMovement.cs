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

    public AudioClip walkClip; // Walking sound
    public AudioClip sprintClip; // Sprinting sound
    public AudioClip atmosphericClip; // Atmospheric sound (e.g., wind, rain, etc.)
    private AudioSource audioSource;
    private AudioSource atmosphericAudioSource; // Separate AudioSource for atmospheric sound

    void Start()
    {
        controller = GetComponent<CharacterController>();
        originalHeight = controller.height;
        originalCenter = controller.center;

        // Get the AudioSource attached to the player for movement sounds
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        // Create a separate AudioSource for atmospheric sounds
        atmosphericAudioSource = gameObject.AddComponent<AudioSource>();
        atmosphericAudioSource.clip = atmosphericClip;
        atmosphericAudioSource.loop = true; // Loop the atmospheric sound
        atmosphericAudioSource.playOnAwake = true; // Start playing when the game starts

        // Start the atmospheric sound
        atmosphericAudioSource.Play();

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMouseLook();
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
            if (!audioSource.isPlaying || audioSource.clip != sprintClip)
            {
                audioSource.clip = sprintClip; // Switch to sprinting sound
                if (!audioSource.isPlaying)
                    audioSource.Play();
            }
        }
        else
        {
            currentSpeed = moveSpeed;
            if (!audioSource.isPlaying || audioSource.clip != walkClip)
            {
                audioSource.clip = walkClip; // Switch to walking sound
                if (!audioSource.isPlaying)
                    audioSource.Play();
            }
        }

        controller.Move(move * currentSpeed * Time.deltaTime);

        if (isMoving && !audioSource.isPlaying)
        {
            audioSource.Play(); // Ensure the audio starts if the player is moving
        }
        else if (!isMoving && audioSource.isPlaying)
        {
            audioSource.Stop(); // Stop the audio when the player is not moving
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
