using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildPanelManager : MonoBehaviour
{
    [Serializable]
    public class ButtonBuildingSOPair
    {
        public Button button;
        public BuildingSO buildingSO;
    }

    [SerializeField] private List<ButtonBuildingSOPair> buttonBuildingPairs;
    
    void Start()
    {
        foreach (ButtonBuildingSOPair buttonBuildingPair in buttonBuildingPairs)
        {
            Button button = buttonBuildingPair.button;
            BuildingSO buildingSO = buttonBuildingPair.buildingSO;
            
            // Register build on click
            button.onClick.AddListener(() =>
            {
                BuildingSystem.Instance.StartBuilding(buildingSO);
                UIManager.Instance.CloseShopPanel();
            });

            foreach (ResourceAmountPair resourceAmountPair in buildingSO.cost)
            {
                // Update resources
                Transform textTransform = button.transform.Find(resourceAmountPair.resource.name + "Text");
                if (textTransform == null) continue;
                
                textTransform.GetComponent<TextMeshProUGUI>().text = resourceAmountPair.amount.ToString();
            }
        }
    }
}
