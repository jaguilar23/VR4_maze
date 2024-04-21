using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    // private CollisionDetection myColDec;
    
    private SkinnedMeshRenderer bodyMeshRender;

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

    // Start is called before the first frame update
    void Start()
    {
        
        myXrOrigin = GameObject.Find("XR Origin (XR Rig)");
        //GameObject myXrOrigin = GameObject.Find("XR Origin (XR Rig)");
        myRB = GetComponent<Rigidbody>();
        myXRRig = myXrOrigin.transform;
        inputData = myXrOrigin.GetComponent<InputData>();

        /*
        // skinned mesh
        GameObject childBodyObj = transform.GetChild(0).transform.GetChild(0).gameObject;
        bodyMeshRender = childBodyObj.GetComponent<SkinnedMeshRenderer>();
        */

    }

    // Update is called once per frame
    void Update()
    {
        xInput = Input.GetAxis("Horizontal");
        zInput = Input.GetAxis("Vertical");

        //bodyMeshRender.enabled = false;
        myXRRig.position = transform.position + transform.up * 1.6f;
        //myXRRig.rotation = transform.rotation;

        /*
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
        */
    }
    
    private void FixedUpdate()
    {
        Vector3 localMove = transform.TransformDirection(moveAmount) * Time.fixedDeltaTime;
        myRB.MovePosition(myRB.position + localMove);

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
    /*
    // check if player is positioned above a slope
    private bool slopeCheck()
    {
        onSlope = Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f);
        // perform a raycast if the length between the player and the floor beneath it is +0.3f
        // the information about the object will be stored on slopeHit
        if (onSlope)
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }
    */


}
