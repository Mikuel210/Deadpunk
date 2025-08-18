using UnityEngine;

namespace Helpers
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance;
        public Singleton() => Instance = this as T;
    }
}