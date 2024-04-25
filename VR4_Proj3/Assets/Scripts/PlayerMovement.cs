using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    // private CollisionDetection myColDec;
    
    private float xInput;
    private float zInput;
    public float movementSpeed = 5.0f;

    private InputData inputData;
    GameObject myXrOrigin;
    private Rigidbody myRB;         // player rigidbody
    private Transform myXRRig;      // XR headset rigidbody

    [Header("Ground Check")]
    public float playerHeight = 0.6f;
    public LayerMask groundedMask;
    public bool grounded;
    public float currentGravity = 0f;
    private float maxGravity = -9.8f;

    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;

    // animation
    private Animator anim;

    // comprehensive list of player animation states
    private enum State { idle, run, sprint };
    private State currentState;

    // Start is called before the first frame update
    void Start()
    {
        
        myXrOrigin = GameObject.Find("XR Origin (XR Rig)");
        myRB = GetComponent<Rigidbody>();
        myXRRig = myXrOrigin.transform;
        inputData = myXrOrigin.GetComponent<InputData>();

        anim = GetComponent<Animator>();
        currentState = State.idle;  // set player's state to idle by default

    }

    // Update is called once per frame
    void Update()
    {
        xInput = Input.GetAxis("Horizontal");
        zInput = Input.GetAxis("Vertical");

        myXRRig.position = transform.position + transform.up * 1.6f;
        //myXRRig.rotation = transform.rotation;

        // fetching 2D joystick input
        if (inputData.leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 movement))
        {
            xInput = movement.x;
            zInput = movement.y;
        }
        else
        {
            // keyboard/controller input
            xInput = Input.GetAxis("Horizontal");
            zInput = Input.GetAxis("Vertical");
        }
        


        // Smoothed xz-movement
        Vector3 moveDir = new Vector3(xInput, 0, zInput).normalized;
        Vector3 targetMoveAmount = moveDir * movementSpeed;
        moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, 0.15f);

        // Ground check
        Vector3 playerHeightOffset = new Vector3(0.0f, 1.0f, 0.0f);
        Ray ray = new Ray(transform.position + playerHeightOffset, -transform.up);
        RaycastHit hit;

        // rotate with XR
        if (inputData.Device.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotD))
        {
            Vector3 eulerRotation = new Vector3(transform.eulerAngles.x, rotD.eulerAngles.y, transform.eulerAngles.z);
            transform.rotation = Quaternion.Euler(eulerRotation);
        }
        else
        {
            GameObject cameraTransform = myXrOrigin.transform.GetChild(0).GetChild(0).gameObject;
            Vector3 eulerRotation = new Vector3(transform.eulerAngles.x, myXRRig.eulerAngles.y, transform.eulerAngles.z);
            transform.rotation = Quaternion.Euler(eulerRotation);
        }

        grounded = (Physics.Raycast(ray, out hit, playerHeight + 0.1f, groundedMask)) ? true : false;

        anim.SetInteger("State", (int)currentState);
    }
    
    private void FixedUpdate()
    {
        Vector3 localMove = transform.TransformDirection(moveAmount) * Time.fixedDeltaTime;
        myRB.MovePosition(myRB.position + localMove);

        // check if the player is not moving
        if (moveAmount.magnitude <= 0.01f)
            currentState = State.idle;
        else if (movementSpeed > 2)
            currentState = State.sprint;
        else
            currentState = State.run;

        // turn off gravity when on a slope
        //myRB.useGravity = !grounded;
        if (!grounded)
        {
            if (currentGravity > maxGravity)
                currentGravity -= 0.1f;
        }
        else
            currentGravity = 0;

        Vector3 gravity = currentGravity * Vector3.up;
        myRB.AddForce(gravity, ForceMode.Acceleration);
    }
    
    public void changeAnimState(int state)
    {
        currentState = (State)state;
    }

}
