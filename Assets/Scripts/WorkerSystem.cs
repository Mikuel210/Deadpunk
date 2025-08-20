using System;
using Helpers;
using UnityEngine;

public class WorkerSystem : MonoBehaviour
{
    public static int PeopleWorking { get; private set; }
    public int WorkersOnBuilding { get; private set; }
    [field: SerializeField] public int MaximumWorkersOnBuilding { get; private set; }
    
    public static event Action OnWorkersUpdated;

    void Start() {
        GameObject workerUIPrefab = UIManager.Instance.WorkerUIPrefab;
        Instantiate(workerUIPrefab, transform);
    }

    void OnDestroy() {
        PeopleWorking -= WorkersOnBuilding;
        OnWorkersUpdated?.Invoke();
    }

    public bool CanAddWorker() {
        if (PeopleWorking >= GameManager.Instance.People) return false;
        if (WorkersOnBuilding >= MaximumWorkersOnBuilding) return false;

        return true;
    }
    public void AddWorker() {
        if (!CanAddWorker()) return;
        
        WorkersOnBuilding++;
        PeopleWorking++;
        
        OnWorkersUpdated?.Invoke();
    }
   
    public bool CanRemoveWorker() => WorkersOnBuilding > 0;
    public void RemoveWorker() {
        if (!CanRemoveWorker()) return;
        
        WorkersOnBuilding--;
        PeopleWorking--;
        
        OnWorkersUpdated?.Invoke();
    }
    
    
    public static void InvokeOnWorkersUpdated() => OnWorkersUpdated?.Invoke();
}
