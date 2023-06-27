using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class MovementScript : MonoBehaviour
{
    // rayCast will touch ground from half player height to halt when movement key releases
    public float groundDrag;
    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround; // 0.8 is working
    bool grounded;
    public float rayCastHeight;

    [Header("Player Speed & Orientation")]
    public float playerSpeed;
    public Transform orient;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Vector3 position = transform.position;
        grounded = Physics.Raycast(position, Vector3.down, playerHeight * rayCastHeight + 0.3f, whatIsGround);

        myInput();
        Debug.DrawRay(transform.position + new Vector3(0, 0.05f, 0), Vector3.down, Color.blue);
        if (grounded)
        {
            Debug.Log(grounded);
            rb.drag = groundDrag;
        }
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        movePlayer();
    }

    void myInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    void movePlayer()
    {
        moveDirection = moveDirection = orient.forward * verticalInput + orient.right * horizontalInput;
        rb.AddForce(moveDirection.normalized * playerSpeed, ForceMode.Force);
    }
}
