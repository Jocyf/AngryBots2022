using UnityEngine;
using System.Collections;

[System.Serializable]
public class ObjectCache : object
{
    public GameObject prefab;
    public int cacheSize;
    private GameObject[] objects;
    private int cacheIndex;
    public virtual void Initialize()
    {
        this.objects = new GameObject[this.cacheSize];
        // Instantiate the objects in the array and set them to be inactive
        int i = 0;
        while (i < this.cacheSize)
        {
            this.objects[i] = MonoBehaviour.Instantiate(this.prefab) as GameObject;
            this.objects[i].SetActive(false);
            this.objects[i].name = this.objects[i].name + i;
            i++;
        }
    }

    public virtual GameObject GetNextObjectInCache()
    {
        GameObject obj = null;
        // The cacheIndex starts out at the position of the object created
        // the longest time ago, so that one is usually free,
        // but in case not, loop through the cache until we find a free one.
        int i = 0;
        while (i < this.cacheSize)
        {
            obj = this.objects[this.cacheIndex];
            // If we found an inactive object in the cache, use that.
            if (!obj.activeSelf)
            {
                break;
            }
            // If not, increment index and make it loop around
            // if it exceeds the size of the cache
            this.cacheIndex = (this.cacheIndex + 1) % this.cacheSize;
            i++;
        }
        // The object should be inactive. If it's not, log a warning and use
        // the object created the longest ago even though it's still active.
        if (obj.activeSelf)
        {
            Debug.LogWarning(((("Spawn of " + this.prefab.name) + " exceeds cache size of ") + this.cacheSize) + "! Reusing already active object.", obj);
            Spawner.Destroy(obj);
        }
        // Increment index and make it loop around
        // if it exceeds the size of the cache
        this.cacheIndex = (this.cacheIndex + 1) % this.cacheSize;
        return obj;
    }

    public ObjectCache()
    {
        this.cacheSize = 10;
    }

}
[System.Serializable]
public partial class Spawner : MonoBehaviour
{
    public static Spawner spawner;
    public ObjectCache[] caches;
    public Hashtable activeCachedObjects;
    public virtual void Awake()
    {
        // Set the global variable
        Spawner.spawner = this;
        // Total number of cached objects
        int amount = 0;
        // Loop through the caches
        int i = 0;
        while (i < this.caches.Length)
        {
            // Initialize each cache
            this.caches[i].Initialize();
            // Count
            amount = amount + this.caches[i].cacheSize;
            i++;
        }
        // Create a hashtable with the capacity set to the amount of cached objects specified
        this.activeCachedObjects = new Hashtable(amount);
    }

    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        ObjectCache cache = null;
        // Find the cache for the specified prefab
        if (Spawner.spawner)
        {
            int i = 0;
            while (i < Spawner.spawner.caches.Length)
            {
                if (Spawner.spawner.caches[i].prefab == prefab)
                {
                    cache = Spawner.spawner.caches[i];
                }
                i++;
            }
        }
        // If there's no cache for this prefab type, just instantiate normally
        if (cache == null)
        {
            return UnityEngine.Object.Instantiate(prefab, position, rotation) as GameObject;
        }
        // Find the next object in the cache
        GameObject obj = cache.GetNextObjectInCache();
        // Set the position and rotation of the object
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        // Set the object to be active
        obj.SetActive(true);
        Spawner.spawner.activeCachedObjects[obj] = true;
        return obj;
    }

    public static void Destroy(GameObject objectToDestroy)
    {
        if (Spawner.spawner && Spawner.spawner.activeCachedObjects.ContainsKey(objectToDestroy))
        {
            objectToDestroy.SetActive(false);
            Spawner.spawner.activeCachedObjects[objectToDestroy] = false;
        }
        else
        {
            GameObject.Destroy(objectToDestroy);
        }
    }

}