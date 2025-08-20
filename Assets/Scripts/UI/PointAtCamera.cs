using UnityEngine;

public class PointAtCamera : MonoBehaviour
{
    void Update()
    {
        Transform cameraPosition = Camera.main.transform;
        transform.LookAt(cameraPosition.forward - cameraPosition.position);
        transform.eulerAngles = new(transform.eulerAngles.x, 0, 0);
    }
}
