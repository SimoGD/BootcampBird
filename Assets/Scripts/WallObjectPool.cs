// bron: https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Pool.ObjectPool_1.html?utm_source=chatgpt.com
using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
public class WallObjectPool : MonoBehaviour
{
    public GameObject[] WallPrefabs;
    private int _colorIndex = 0;
    private ObjectPool<GameObject> pool;

    [Tooltip("How often a wall spawns in.")]
    [SerializeField] private float spawnTimer = 3f;
    [Tooltip("Despawn a wall after the specified amount of time.")]
    [SerializeField] private float despawnTimer = 10f;
    [Tooltip("Number of instances in the pool.")]
    [SerializeField] private int poolCapacity = 9;
    [Tooltip("Max number of pooled objects. ")]
    [SerializeField] private int maxSize = 9;
    [Tooltip("Travelling speed of the walls")]
    [SerializeField] private int speed = 50;    

    private void Awake()
    {
        // Create a pool with the four core callbacks.
        pool = new ObjectPool<GameObject>(
            createFunc: CreateItem,
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: OnDestroyItem,
            collectionCheck: true,   // helps catch double-release mistakes
            defaultCapacity: poolCapacity,
            maxSize: maxSize
        );

        GameObject[] prewarmedPool = new GameObject[poolCapacity];

        // create all prefabs
        for (int i = 0; i < WallPrefabs.Length; i++ )
        {
            prewarmedPool[i] = pool.Get();
            
        }

        // store them in the pool
        for (int i = 0; i < poolCapacity; i++)
        {
            pool.Release(prewarmedPool[i]);
        }
    }
    private void Start()
    {
        InvokeRepeating(nameof(SpawnWall), 0.0f, spawnTimer);
    }
    private GameObject SpawnWall()
    {

        GameObject pooledObject = GetRandomPooledObject();

        StartCoroutine(ReturnAfter(pooledObject, despawnTimer));
        
        return pooledObject;
    }

    private GameObject GetRandomPooledObject()
    {
        List<GameObject> availableObjects = new List<GameObject>();

        // Collect all inactive objects from the pool.
        while (pool.CountInactive > 0)
        {
            availableObjects.Add(pool.Get());
        }

        // If there are no inactive objects available, create a new one
        if (availableObjects.Count == 0)
        {
            return pool.Get();
        }

        int randomIndex = Random.Range(0, availableObjects.Count);

        GameObject selectedObject = availableObjects[randomIndex];

        // Return all other objects to the pool.
        for (int i = 0; i < availableObjects.Count; i++)
        {
            if (i != randomIndex)
            {
                pool.Release(availableObjects[i]);
            }
        }

        // Send one random wall back.
        return selectedObject;
    }

    // Creates a new pooled GameObject the first time (and whenever the pool needs more).
    private GameObject CreateItem()
    {
        int randIndex = Random.Range(0, WallPrefabs.Length);

        GameObject pooledObject = Instantiate(WallPrefabs[randIndex], transform, false);

        // Change its color to something random.
        Renderer[] renderers = pooledObject.GetComponentsInChildren<Renderer>();

        // Pick a random bright color.
        Color[] colors =
        {
            Color.red,
            Color.yellow,
            Color.green,
            Color.blue,
            Color.magenta,
        };

        Color color = colors[_colorIndex % colors.Length];

        // Color each obstical part of the prefab with the randomly chosen color.
        foreach (Renderer renderer in renderers)
        {
            string objectName = renderer.gameObject.name;

            if(objectName.StartsWith("Obstacle"))
            {
                renderer.material.color = color;
                _colorIndex++;
            }
        }
       
        // Set the travelling speed of the current wall.
        WallMovement movement = pooledObject.GetComponent<WallMovement>();
        if (movement != null)
            movement.SetSpeed(speed);

        pooledObject.name = "PooledWall";
        pooledObject.SetActive(false);

        return pooledObject;
    }

    // Called when an item is taken from the pool.
    private void OnGet(GameObject pooledObject)
    {
        pooledObject.SetActive(true);
    }

    // Called when an item is returned to the pool.
    private void OnRelease(GameObject pooledObject)
    {
        pooledObject.SetActive(false);
    }

    // Called when the pool decides to destroy an item (e.g., above max size).
    private void OnDestroyItem(GameObject pooledObject)
    {
        Destroy(pooledObject);
    }

    private System.Collections.IEnumerator ReturnAfter(GameObject pooledObject, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        // Give it back to the pool.
        pool.Release(pooledObject);
    }
}
