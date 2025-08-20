using System;
using System.Reflection;
using UnityEngine;

public class ResourceHarvester : MonoBehaviour {

    [SerializeField] private string resourceName;
    [SerializeField] private float amountPerWorkerPerMinute;

    private WorkerSystem _workerSystem;
    private float _time;
    private float _interval;
    private int _workers;
    
    void Start() 
    {
        _workerSystem = GetComponent<WorkerSystem>();
        WorkerSystem.OnWorkersUpdated += UpdateWorkers;
        
        UpdateWorkers();
    }
    
    void Update()
    {
        _time += Time.deltaTime;

        if (_time < _interval) return;
        
        Type gameManagerType = typeof(GameManager);

        // Look for the resource property
        PropertyInfo property = gameManagerType.GetProperty(resourceName, BindingFlags.Public | BindingFlags.Instance);
        object resourceObject = property.GetValue(GameManager.Instance);

        // Look for the Value property on the observer
        PropertyInfo valueProperty = resourceObject.GetType().GetProperty("Value");
        
        // Add the value
        int currentValue = (int)valueProperty.GetValue(resourceObject);
        valueProperty.SetValue(resourceObject, currentValue + 1);
        
        // Update events
        if (resourceName == "People")
            WorkerSystem.InvokeOnWorkersUpdated();
        
        // Reset time
        _time = 0;
    }

    void UpdateWorkers() {
        _workers = _workerSystem.WorkersOnBuilding;
        _interval = 60f / _workers / amountPerWorkerPerMinute;
    }
}
