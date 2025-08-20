using UnityEngine;

public class PointSpriteAtCamera : MonoBehaviour {

	void Update()
	{
		Transform cameraTransform = Camera.main.transform;
		Vector3 eulerAngles = transform.eulerAngles;
		
		transform.LookAt(cameraTransform.position);
		transform.eulerAngles = new(eulerAngles.x, transform.eulerAngles.y, 0);
	}

}