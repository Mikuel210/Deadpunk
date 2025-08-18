using UnityEngine;

public class House : MonoBehaviour
{
    [SerializeField] private int housing; 
    
    void Start() => GameManager.Instance.Housing.Value += housing;
    void OnDestroy() => GameManager.Instance.Housing.Value -= housing;
}
