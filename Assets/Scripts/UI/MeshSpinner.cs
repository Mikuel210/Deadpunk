using UnityEngine;

public class MeshSpinner : MonoBehaviour
{
    private const float RotationSpeed = 90;
    private Transform _mesh;
    
    void Start() => _mesh = transform.Find("Mesh");
    void Update() => _mesh.rotation = Quaternion.Euler(0, _mesh.rotation.eulerAngles.y + RotationSpeed * Time.deltaTime, 0);
}
