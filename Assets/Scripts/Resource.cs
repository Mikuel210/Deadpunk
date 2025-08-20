
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Resource : MonoBehaviour
{
    private ResourceSO _resourceSO;
    private Vector2Int _origin;

    public static Resource Create(Vector3 worldPosition, ResourceSO resourceSO, Vector2Int origin)
    {
        GameObject prefab = resourceSO.prefabs[Random.Range(0, resourceSO.prefabs.Count - 1)];
        Quaternion rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        float scale = Random.Range(0.75f, 1.25f);
        
        Transform resourceTransform = Instantiate(prefab, worldPosition, rotation).transform;
        resourceTransform.localScale = Vector3.one * scale;
        
        Resource resource = resourceTransform.GetComponent<Resource>();
        resource._resourceSO = resourceSO;
        resource._origin = origin;

        return resource;
    }

    public void Demolish()
    {
        Destroy(gameObject);
    }
    
    public ResourceSO GetResourceSO() => _resourceSO;
}

[Serializable]
public class ResourceAmountPair
{
    public ResourceSO resource;
    public int amount;
}