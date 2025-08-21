using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textObject;
    [SerializeField] private Image plusButton;
    [SerializeField] private Image minusButton;
    
    private WorkerSystem _workerSystem;

    void Awake() => _workerSystem = transform.parent.GetComponent<WorkerSystem>();

    void Start() {
        transform.position = transform.parent.Find("Mesh").position + new Vector3(0, 15, 0);
        
        WorkerSystem.OnWorkersUpdated += UpdateUI;
        UpdateUI();
    }

    private void UpdateUI() {
        // Update text
        string text = _workerSystem.WorkersOnBuilding + " / " + _workerSystem.MaximumWorkersOnBuilding;
        UIManager.UpdateTextObject(textObject, text, true);
        
        // Update buttons
        if (plusButton) plusButton.color = _workerSystem.CanAddWorker() ? Color.white : Color.red;
        if (minusButton) minusButton.color = _workerSystem.CanRemoveWorker() ? Color.white : Color.red;
    }

    public void AddWorker() => _workerSystem.AddWorker();
    public void RemoveWorker() => _workerSystem.RemoveWorker();
}
