using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject player;

    private Vector3 lastPosition;
    private Vector3[] lastFewPositions = new Vector3[4];   // last 3 positions, not including current position
    private int posCount;
    private Vector3 currentGoal;    // last position player was at

    private int noMovementCount = 0;

    [Header("Line of sight")]
    public float distance = 10;
    public float angle = 30;
    public float height = 1.0f;
    public Color meshColor = Color.red;
    public int scanFrequency = 30;
    public LayerMask layers;
    public List<GameObject> Objects = new List<GameObject>();

    [Header("Aggro Mode")]
    public bool isAggro = false;
    public float coolDown = 1.0f;
    private float currentCoolDown;

    Collider[] colliders = new Collider[50];
    Mesh sightMesh;
    int count;
    float scanInterval;
    public float scanTimer;

    AudioSource audioSource;
    public AudioClip spawnNoise;
    public AudioClip closeNoise;
    public AudioClip deathNoise;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lastPosition = transform.position;

        for (int i = 0; i < lastFewPositions.Length; ++i)
            lastFewPositions[i] = transform.position;

        currentGoal = transform.position;
        scanInterval = 1.0f / scanFrequency;

        currentCoolDown = 0;
    }

    public void PlaySpawnNoise()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = spawnNoise;
        audioSource.Play();
    }

    public void PlayDeathNoise()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        audioSource.clip = deathNoise;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);

        if ((transform.position - lastPosition) != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(transform.position - lastPosition);
        

        scanTimer -= Time.deltaTime;
        if (scanTimer < 0)
        {
            scanTimer += scanInterval;
            Scan();
        }

        if (isAggro)
        {
            currentCoolDown = coolDown;
            isAggro = false;
        }

        
        // if zombie is frozen, count the number of consecutive stopped movements
        if ((transform.position - lastFewPositions[3]).magnitude > 0.1f)
            noMovementCount = 0;
        else
            noMovementCount += 1;

        //Debug.Log("Last few positions:\n" + lastFewPositions[0] + ", " + lastFewPositions[1] + ", " + lastFewPositions[2]);
        //Debug.Log("Current Speed: " + (transform.position - lastFewPositions[3]).magnitude);
        //Debug.Log(noMovementCount);

        if (currentCoolDown > 0)    // demon mode
        {
            currentCoolDown -= Time.deltaTime;
            agent.SetDestination(player.transform.position);
            agent.speed = 1.5f;
        }
        else // funky mode
        {
            agent.speed = 2.3f;

            if (ReachedGoal() || noMovementCount > 10)
            {
                agent.SetDestination(player.transform.position);
                currentGoal = player.transform.position;
            }
        }
        
        lastPosition = transform.position;  // updating last position

        if ((transform.position - player.transform.position).magnitude < 10 && !audioSource.isPlaying)
        {
            audioSource.clip = closeNoise;
            audioSource.Play();
        }

        /*
        for (int i = 0; i < lastFewPositions.Length - 1; ++i)
        {
            lastFewPositions[i + 1] = lastFewPositions[i];
        }
         */
        lastFewPositions[3] = lastFewPositions[2];
        lastFewPositions[2] = lastFewPositions[1];
        lastFewPositions[1] = lastFewPositions[0];
        lastFewPositions[0] = lastPosition;

    }

    public void Scan()
    {
        count = Physics.OverlapSphereNonAlloc(transform.position, distance, colliders, layers, QueryTriggerInteraction.Collide);

        Objects.Clear();
        for (int i = 0; i < count; ++i)
        {
            GameObject obj = colliders[i].gameObject;
            if (IsInSight(obj))
            {
                //currentCoolDown = coolDown; // reset aggro timer
                isAggro = true;
                Objects.Add(obj);
            }
        }

    }

    public bool IsInSight(GameObject obj)
    {
        Vector3 origin = transform.position;
        Vector3 dest = obj.transform.position;
        Vector3 direction = dest - origin;

        if (direction.y < 0 || direction.y > height)
            return false;

        direction.y = 0;
        float deltaAngle = Vector3.Angle(direction, transform.forward);
        
        if (deltaAngle > angle)
            return false;

        origin.y += height / 2;
        dest.y = origin.y;

        return true;
    }

    public bool ReachedGoal()
    {
        float distance = Vector3.Distance(transform.position, currentGoal);

        return distance < 1f;
    }

    Mesh CreateWedgeMesh()
    {
        Mesh mesh = new Mesh();

        int segments = 10;
        int numTriangles = (segments * 4) + 2 + 2;
        int numVertices = numTriangles * 3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
        Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

        Vector3 topCenter = bottomCenter + Vector3.up * height;
        Vector3 topRight = bottomRight + Vector3.up * height;
        Vector3 topLeft = bottomLeft + Vector3.up * height;

        int vert = 0;

        // left side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;

        // right side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        float currentAngle = -angle;
        float deltaAngle = (angle * 2) / segments;

        for (int i = 0; i < segments; ++i)
        {
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance;
            
            topRight = bottomRight + Vector3.up * height;
            topLeft = bottomLeft + Vector3.up * height;

            // far side
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;

            // top
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;

            // bottom
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomLeft;

            currentAngle += deltaAngle;
        }

        for (int i = 0; i < numVertices; ++i)
        {
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }
    
    // Calls function whenever a variable was adjusted in the editor
    private void OnValidate()
    {
        sightMesh = CreateWedgeMesh();
        scanInterval = 1.0f / scanFrequency;
    }

    private void OnDrawGizmos()
    {
        if (sightMesh)
        {
            Gizmos.color = meshColor;
            Gizmos.DrawMesh(sightMesh, transform.position, transform.rotation);
        }

        Gizmos.DrawWireSphere(transform.position, distance);
        for (int i = 0; i < count; ++i)
        {
            Gizmos.DrawSphere(colliders[i].transform.position, 0.2f);
        }

        Gizmos.color = Color.green;
        foreach (var obj in Objects)
        {
            Gizmos.DrawSphere(obj.transform.position, 5.0f);
        }
    }
}
