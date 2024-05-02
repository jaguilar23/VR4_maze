using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorMovement : MonoBehaviour
{
    private Transform playerTarget;
    public Transform mirror;

    // Start is called before the first frame update
    void Start()
    {
        playerTarget = GameObject.Find("Idle").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 localPlayer = mirror.InverseTransformPoint(playerTarget.position);
        transform.position = mirror.TransformPoint(new Vector3(localPlayer.x, localPlayer.y + 1.6f, -localPlayer.z));

        Vector3 lookatmirror = mirror.TransformPoint(new Vector3(-localPlayer.x, localPlayer.y, localPlayer.z));
        //transform.LookAt(lookatmirror);
    }
}
