using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    // Adjustables from editor
    [Header("Movements")]
    public float playerSpeed;
    public float jumpHeight;
    public float airVelocity;
    public float groundDrag; // rigidBody to control drag according to ground check

    [Header("Cam & Key Rotations")]
    public new Transform camera;
    public float turnSmoothTime;
    public float smoothVel;

    // trigger for ground check from collision functions
    public bool grounded;
    private Rigidbody rb;

    // Input variables
    float horizontalInput; // gets axis
    float verticalInput;   // gets axis
    Vector3 movement; // in which dir. should a player be moving according to inputs

    // Animation
    Animator animator;
    int animXposID;
    int animZposID;
    int animJump;
    public float animJumpTransitionTime = 0.15f;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        // setting value from animation Editor to code userBased named ID
        animXposID=Animator.StringToHash("moveX");
        animZposID = Animator.StringToHash("moveZ");
        animJump = Animator.StringToHash("Jump");
    }

    private void Update()
    {
        movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

        // control the velocity of rigidDoby if collision touches ground layer then ground drag will preform
        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
            // drag becomes zero and prevents player from slow motion jump/downfall
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        // if player speed not in sync with rigidBody physics then cap to speed
        speedControl();
        // for player to rotate with input and camera direction
        movePlayer(movement, grounded);
        // handle playerInputs and Animations
        playerInputAndAnimations(movement, grounded);
    }

    void movePlayer(Vector3 direction, bool isOnGround)
    {
        // if player is touching the ground then only can rotate
        if (isOnGround && direction.magnitude > 0.01f)
        {
            // get the angle & set the player face to input and camera direction
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothVel, turnSmoothTime);

            // rotate the face of player
            rb.rotation = Quaternion.Euler(0, angle, 0);

            // face in the camera direction and move there
            Vector3 camDirection = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            rb.AddForce(camDirection.normalized * playerSpeed, ForceMode.Force);
        }
    }

    void playerInputAndAnimations(Vector3 playerDirection, bool isOnGround)
    {
        // player jump
        if (isOnGround)
        {
            // Animations
            animator.SetFloat(animXposID, playerDirection.x);
            animator.SetFloat(animZposID, playerDirection.z);

            if (Input.GetKey(KeyCode.Space))
            {
                animator.CrossFade(animJump, animJumpTransitionTime);

                rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
            }
        }
        // if player is in air or jumping then player should not have regular velocity of movement
        // and stop all strafing animations
        else if (!isOnGround)
        {
            // Animation stop
            animator.SetFloat(animXposID, 0);
            animator.SetFloat(animZposID, 0);

            rb.AddForce(playerDirection * airVelocity, ForceMode.Force);
        }
    }

    // get grounded bool true or false from these triggerEnter on specific tags
    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger.tag == "Ground")
        {
            grounded = true;
        }
    }
    private void OnTriggerExit(Collider trigger)
    {
        grounded = false;
    }

    void speedControl()
    {
        // taking velocity from rigidbody and storing it in flatVel. to get previous velocity of player
        Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        // limit the speed
        if (flatVel.magnitude > playerSpeed)
        {
            Vector3 limitSpeed = flatVel.normalized * playerSpeed;
            rb.velocity = new Vector3(limitSpeed.x, rb.velocity.y, limitSpeed.z);
        }
    }
}
