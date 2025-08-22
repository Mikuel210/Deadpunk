using System;
using System.Collections.Generic;
using System.Linq;
using DayNightCycle;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour {

    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI text;

    [Space, SerializeField] private BuildingSO huntersHut;
    [SerializeField] private BuildingSO woodWorkshop;
    [SerializeField] private BuildingSO explorationBeacon;
    [SerializeField] private BuildingSO mediumHouse;
    [SerializeField] private BuildingSO stoneMine;
    [SerializeField] private BuildingSO sentry;

    private int _currentIndex; 

    private List<string> _messages = new() {
        "Welcome, mayor. At last, you have arrived. Our settlement is fragile, and the dead will find us soon.",
        "We lack the strength to withstand them. To survive, we must build, gather, and prepare. The people look to you for guidance.",
        "Food must come first. Open the shop in the bottom-right corner, select the Hunters' Hut from the Resources tab, and place it. Right-click when you're done.\n(Use WASD to move your view, and the scroll wheel to zoom)",
        "Good. Now assign workers. Click the plus above the Hut and set five survivors to the task. They'll keep us fed for now.",
        "Our supplies won’t last. We need wood. Build a Wood Workshop near the trees, then assign five workers to gather timber.",
        "More hands mean more progress. Construct an Exploration Beacon, reassign workers from other jobs, and send them to search for survivors. Grow our numbers to at least 15.",
        "Remove workers from the Beacon to stop the search.",
        "The newcomers will need shelter. Build a Medium House from the People tab, so our community can endure the cold nights.",
        "Stone is vital for building sturdier defenses. Place a Stone Mine near the rocks, and send five workers to harvest rubble.",
        "The dead draw closer. Build Wooden Supports from the Defense I tab, and mount Sentries on them. Place at least four, guarding every direction.",
        "At last, shield the town with wooden walls. Hurry, the dead are right around the corner."
    };

    void Start() {
        ShowMessage();
        UpdateText();
    }

    void Update() {
        switch (_currentIndex) {
            case 0:
            case 1:
            case 2:
                break;
            
            case 3:
                if (IsBuildingBuilt(huntersHut))
                    ShowMessage();

                break;
            
            case 4:
                if (BuildingHasWorkers(huntersHut, 5))
                    ShowMessage();

                break;
            
            case 5:
                if (BuildingHasWorkers(woodWorkshop, 5))
                    ShowMessage();

                break;
            
            case 6:
                if (GameManager.Instance.People >= 15)
                    ShowMessage();

                break;
            
            case 7:
                if (BuildingHasWorkers(explorationBeacon, 0))
                    ShowMessage();
                
                break;
            
            case 8:
                if (IsBuildingBuilt(mediumHouse))
                    ShowMessage();

                break;
            
            case 9:
                if (BuildingHasWorkers(stoneMine, 5))
                    ShowMessage();

                break;
            
            case 10:
                if (IsBuildingBuilt(sentry, 4))
                    ShowMessage();

                break;

        }
    }

    private bool IsBuildingBuilt(BuildingSO buildingType, int amount = 1) {
        List<Building> buildings = GameObject.FindGameObjectsWithTag("Building")
            .Select(e => e.GetComponent<Building>())
            .ToList();

        int amountFound = 0;

        foreach (Building building in buildings) {
            if (building.BuildingSO == buildingType) amountFound++;
            if (amountFound >= amount) break;
        }

        return amountFound >= amount && !BuildingSystem.Instance.PlacingBuilding;
    }

    private bool BuildingHasWorkers(BuildingSO building, int amount) {
        bool output = GameObject.FindGameObjectsWithTag("Building")
            .Select(e => e.GetComponent<Building>())
            .Where(e => e.BuildingSO == building)
            .Where(e => e.GetComponent<WorkerSystem>() != null)
            .Select(e => e.GetComponent<WorkerSystem>().WorkersOnBuilding)
            .Any(e => e == amount);

        return output;
    }
    

    public void Continue() {
        switch (_currentIndex) {
            case 0:
            case 1:
                break;
            
            case 10:
                TimeManager.Instance.TimeSettings.timeMultiplier = 500;
                HideMessage();
                break;
            
            default:
                HideMessage();
                break;
        }

        Advance();
    }

    void ShowMessage() {
        panel.SetActive(true);
    }

    void Advance() {
        _currentIndex++;
        UpdateText();
    }

    void UpdateText() {
        if (_currentIndex >= _messages.Count) return;
        text.text = _messages[_currentIndex];
    }

    void HideMessage() {
        panel.SetActive(false);
    }
}
