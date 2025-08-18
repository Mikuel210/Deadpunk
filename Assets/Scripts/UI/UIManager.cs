using Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private TextMeshProUGUI peopleText;
    [SerializeField] private TextMeshProUGUI housingText;
    [SerializeField] private TextMeshProUGUI foodText;
    [SerializeField] private TextMeshProUGUI woodText;
    [SerializeField] private TextMeshProUGUI stoneText;
    
    [Header("Shop")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Image shopButtonImage;
    [SerializeField] private GameObject shopButtonIcon;
    [Space, SerializeField] private Sprite buttonSprite;
    [SerializeField] private Sprite closeIconSprite;

    private bool _isShopOpen;
    public bool IsShopOpen
    {
        get => _isShopOpen;
        
        private set
        {
            _isShopOpen = value;
            UpdateShop();
        }
    }
    
    void Start()
    {
        GameManager.Instance.People.OnValueChanged += value => UpdateTextObject(peopleText, value, true);
        GameManager.Instance.Housing.OnValueChanged += value => UpdateTextObject(housingText, value);
        GameManager.Instance.Food.OnValueChanged += value => UpdateTextObject(foodText, value);
        GameManager.Instance.Wood.OnValueChanged += value => UpdateTextObject(woodText, value);
        GameManager.Instance.Stone.OnValueChanged += value => UpdateTextObject(stoneText, value);
        
        UpdateShop();
    }

    private void UpdateTextObject(TextMeshProUGUI textObject, object value, bool updateDimensions = false)
    {
        textObject.text = value.ToString();

        if (!updateDimensions) return;
        
        Vector2 preferredDimensions = textObject.GetPreferredValues();
        textObject.rectTransform.sizeDelta = new(preferredDimensions.x, textObject.rectTransform.sizeDelta.y);
    }

    private void UpdateShop()
    {
        shopPanel.SetActive(IsShopOpen);
        shopButtonImage.sprite = IsShopOpen ? closeIconSprite : buttonSprite;
        shopButtonIcon.SetActive(!IsShopOpen);
    }

    public void OpenShopPanel() => IsShopOpen = true;
    public void CloseShopPanel() => IsShopOpen = false;
    public void ToggleShopPanel() => IsShopOpen = !IsShopOpen;
}
