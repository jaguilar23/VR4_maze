using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using UnityEngine;
using TMPro;


public class PlayerMovement : MonoBehaviour
{
    // private CollisionDetection myColDec;
    
    private float xInput;
    private float zInput;
    public float movementSpeed = 5.0f;
    private bool isMoving = false;

    private InputData inputData;
    GameObject myXrOrigin;
    private Rigidbody myRB;         // player rigidbody
    private Transform myXRRig;      // XR headset rigidbody
    public GameObject leftController;
    public GameObject rightController;
    public float smoothTime = 0.1f; // for xrRig to follow player smoothly
    private Vector3 xrVelocity = Vector3.zero;

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

    // UI holds capture and win conditions
    private UI UIobj;
    public Transform deathLocation;
    public Transform winLocation;
    private bool hasTeleported; // prevents getting teleported twice

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

        UIobj = GetComponent<UI>();
    }

    // Update is called once per frame
    void Update()
    {
        xInput = Input.GetAxis("Horizontal");
        zInput = Input.GetAxis("Vertical");

        float xSight = 0.0f;


        if (UIobj.isCaught && !hasTeleported)
        {
            transform.position = deathLocation.position;
            myXRRig.position = transform.position;
            hasTeleported = true;
        }
        else if (UIobj.isFinished && !hasTeleported)
        {
            transform.position = winLocation.position;
            myXRRig.position = transform.position;
            hasTeleported = true;
        }
        
        if (UIobj.isCaught || UIobj.isFinished)
        {
            leftController.SetActive(true);
            rightController.SetActive(true);
        }

        //else
        //XRRig.position = new Vector3(transform.position.x, myXRRig.position.y, transform.position.z);

        //myXRRig.position = transform.position + new Vector3(0, 1.3f, 0) + (transform.forward * 0.2f);
        //myXRRig.position = Vector3.SmoothDamp(myXRRig.position, transform.position + new Vector3(0, 1.0f, 0), ref xrVelocity, smoothTime);
        //myXRRig.position = transform.position + new Vector3(0, 1.3f, 0);

        // fetching 2D joystick input
        if (inputData.leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 movement))
        {
            xInput = movement.x;
            zInput = movement.y;

            if ((xInput != 0) || (zInput != 0))
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }
        }
        else
        {
            // keyboard/controller input
            xInput = Input.GetAxis("Horizontal");
            zInput = Input.GetAxis("Vertical");

            if ((xInput != 0) || (zInput != 0))
            {
                isMoving = true;
            } else
            {
                isMoving = false;
            }
        }

        if (inputData.rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 sight))
        {
            xSight = sight.x;
        }
        else
        {
            xSight = Input.GetAxis("Mouse X");
        }


        myXRRig.transform.GetChild(0).GetComponent<CameraMovement>().inputX = xSight; // sets x input to mouse input

        // Smoothed xz-movement
        Vector3 moveDir = new Vector3(xInput, 0, zInput).normalized;
        Vector3 targetMoveAmount = moveDir * movementSpeed;
        moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, 0.15f);

        // Ground check
        Vector3 playerHeightOffset = new Vector3(0.0f, 1.0f, 0.0f);
        Ray ray = new Ray(transform.position + playerHeightOffset, -transform.up);
        RaycastHit hit;
        
        transform.rotation = myXRRig.transform.GetChild(0).gameObject.transform.rotation;

        grounded = (Physics.Raycast(ray, out hit, playerHeight + 0.1f, groundedMask)) ? true : false;

        anim.SetInteger("State", (int)currentState);
    }
    
    private void FixedUpdate()
    {
        Vector3 localMove = transform.TransformDirection(moveAmount) * Time.fixedDeltaTime;
        myRB.MovePosition(myRB.position + localMove);


        // check if the player is not moving
        if (!isMoving)
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
