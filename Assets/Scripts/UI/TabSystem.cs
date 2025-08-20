using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabSystem : MonoBehaviour
{
    [SerializeField] private List<Button> tabButtons;
    [SerializeField] private List<GameObject> tabPanels;
    
    void Start()
    {
        for (int i = 0; i < tabButtons.Count; i++)
        {
            Button button = tabButtons[i];
            GameObject tab = tabPanels[i];
            
            button.onClick.AddListener(() =>
            {
                foreach (GameObject panel in tabPanels)
                    panel.SetActive(panel == tab);
            });
        }
        
        for (int i = 0; i < tabPanels.Count; i++)
        {
            GameObject panel = tabPanels[i];
            panel.SetActive(i == 0);
        }
    }
}
