using UnityEngine;

namespace Helpers
{
    public static class InputManager
    {
        public static Vector3 GetMousePosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit, 10_000f))
                return hit.point;
            
            return Vector3.zero;
        }
    }
}