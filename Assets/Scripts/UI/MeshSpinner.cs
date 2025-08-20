using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshSpinner : MonoBehaviour
{
    private const float RotationSpeed = 90;
    private Transform _mesh;

    void Start() {
        _mesh = transform.Find("Mesh");
        
        // Scale outline
        List<MeshRenderer> renderers = _mesh.GetComponentsInChildren<MeshRenderer>().ToList();
        
        _mesh.TryGetComponent<MeshRenderer>(out var meshRenderer);
        if (meshRenderer) renderers.Add(meshRenderer);
        
        foreach (MeshRenderer renderer in renderers)
            renderer.material.SetFloat("_OutlineWidth", 0.1f);
    }
    void Update() => _mesh.rotation = Quaternion.Euler(_mesh.rotation.eulerAngles.x, _mesh.rotation.eulerAngles.y + RotationSpeed * Time.deltaTime, 0);
}
