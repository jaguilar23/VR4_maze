using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemy;
    private BoxCollider bc;
    private bool touchedCollider;
    private float countDown = 25f;

    private void Start()
    {
        bc = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (touchedCollider && countDown > 0)
        {
            countDown -= Time.deltaTime;
        }
        else if (touchedCollider && countDown <=0 )
        {
            enemy.SetActive(true);
            enemy.GetComponent<EnemyMovement>().PlaySpawnNoise();
            Object.Destroy(this.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && !touchedCollider)
        {
            bc.enabled = false;
            touchedCollider = true;
        }
    }
}
