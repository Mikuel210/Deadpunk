
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource", menuName = "Scriptable Objects/Resource")]
public class ResourceSO : ScriptableObject
{
    public List<GameObject> prefabs;
}