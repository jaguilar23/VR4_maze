using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBeacon : MonoBehaviour
{
    public Renderer objectToReveal;
    private bool isRevealed = false;

    private MeshRenderer myRenderer;
    private CapsuleCollider myCollider;

    // Start is called before the first frame update
    void Start()
    {
        objectToReveal.GetComponent<Renderer>().enabled = false;
        myRenderer = GetComponent<MeshRenderer>();
        myCollider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("Running Crawl") != null)
        {
            objectToReveal.gameObject.transform.position = GameObject.Find("Running Crawl").transform.position;
        }

        transform.Rotate(transform.up * 60 * Time.deltaTime, Space.Self);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isRevealed && collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(RevealObject());
        }

        // "Delete" powerup
        myRenderer.enabled = false;
        myCollider.enabled = false;
    }

    IEnumerator RevealObject()
    {
        // Set the object to be revealed to active
        objectToReveal.GetComponent<Renderer>().enabled = true;
        Debug.Log("object hit");

        // Wait for 5 seconds
        yield return new WaitForSeconds(10f);

        // Deactivate the object after 5 seconds
        objectToReveal.GetComponent<Renderer>().enabled = false;
        Debug.Log("object hide");

        // Mark as revealed
        isRevealed = true;
    }
}
