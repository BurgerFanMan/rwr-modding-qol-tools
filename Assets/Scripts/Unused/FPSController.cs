using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FPSController : MonoBehaviour
{
    // The speed at which the player moves
    public float speed = 10.0f;

    // The gravity applied to the player
    public float gravity = 10.0f;

    // The jump force applied to the player
    public float jumpForce = 10.0f;

    // The character controller component attached to the player
    private CharacterController controller;

    // The current vertical speed of the player
    private float verticalSpeed = 0.0f;

    // The speed at which the player moves
    public float movementSpeed = 10.0f;

    // The speed at which the player can look around
    public float mouseSensitivity = 2.0f;

    // Reference to the camera attached to the player
    public Camera playerCamera;

    // Use this for initialization
    void Start()
    {
        // Get the character controller component attached to the player
        controller = GetComponent<CharacterController>();

        playerCamera = Camera.main;

        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Get the horizontal and vertical inputs from the player
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate the direction the player should move in
        Vector3 direction = new Vector3(horizontalInput, 0, verticalInput);

        // Rotate the direction vector to match the player's rotation
        direction = transform.rotation * direction;

        // Calculate the speed at which the player should move
        Vector3 velocity = direction * speed;

        // Apply gravity to the player
        verticalSpeed -= gravity * Time.deltaTime;

        // Apply the vertical speed to the player's velocity
        velocity.y = verticalSpeed;

        // Move the player
        controller.Move(velocity * Time.deltaTime);

        // Check if the player is on the ground
        if (controller.isGrounded)
        {
            // If the player is on the ground and the space bar is pressed, jump
            if (Input.GetButtonDown("Jump"))
            {
                verticalSpeed = jumpForce;
            }
        }

        // Move the camera along with the player
        playerCamera.transform.position = transform.position;

        // Get the horizontal and vertical input axes for looking around
        float yaw = Input.GetAxis("Mouse X") * mouseSensitivity;
        float pitch = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate the player based on the input axes for looking around
        transform.Rotate(0, yaw, 0);
        playerCamera.transform.Rotate(-pitch, 0, 0);
    }
}
