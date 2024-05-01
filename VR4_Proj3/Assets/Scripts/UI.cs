using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI : MonoBehaviour
{

    public bool isFinished = false;
    public bool isCaught = false;
   
    
    public bool restartButtonPressed;
    public GameObject start;
    public GameObject playerObject;
    
    
    [SerializeField] TextMeshProUGUI myWin;
    [SerializeField] TextMeshProUGUI myLose;
    public GameObject myRestart;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isFinished)
        {
            myWin.gameObject.SetActive(true);
            myRestart.gameObject.SetActive(true);
        }

        if (isCaught)
        {
            myLose.gameObject.SetActive(true);
            myRestart.gameObject.SetActive(true);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Exit")
        {
            isFinished = true;
        }

        if (collision.gameObject.tag == "Enemy")
        {
            isCaught = true;
        }
    }

    public void restartToggle()
    {
        restartButtonPressed = true;
    }

    public void restart()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null) 
        { 
            playerObject.transform.position = start.gameObject.transform.position;
            restartButtonPressed = true;
        
        }
    }
}
