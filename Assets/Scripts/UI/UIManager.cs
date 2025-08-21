using Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("Prefabs")]
    [field: SerializeField] public GameObject WorkerUIPrefab { get; private set; }
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI peopleText;
    [SerializeField] private TextMeshProUGUI workingText;
    [SerializeField] private TextMeshProUGUI housingText;
    [SerializeField] private TextMeshProUGUI foodText;
    [SerializeField] private TextMeshProUGUI woodText;
    [SerializeField] private TextMeshProUGUI stoneText;
    [SerializeField] private TextMeshProUGUI metalText;
    [SerializeField] private TextMeshProUGUI waveText;
    [Space, SerializeField] private Slider happinessSlider;
    [SerializeField] private Slider hungerSlider;
    
    [Header("Shop")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Image shopButtonImage;
    [SerializeField] private GameObject shopButtonIcon;
    [Space, SerializeField] private Image demolishButtonImage;
    [SerializeField] private GameObject demolishButtonIcon;
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
    
    private bool _isDemolishing;
    public bool IsDemolishing
    {
        get => _isDemolishing;
        
        private set
        {
            _isDemolishing = value;
            UpdateShop();

            if (value) BuildingSystem.Instance.StartDemolishing();
            else BuildingSystem.Instance.StopDemolishing();
        }
    }
    
    void Awake()
    {
        GameManager.Instance.People.OnValueChanged += value => UpdateTextObject(peopleText, value, true);
        WorkerSystem.OnWorkersUpdated += () => UpdateTextObject(workingText, WorkerSystem.PeopleWorking + " Working", true);
        GameManager.Instance.Housing.OnValueChanged += value => UpdateTextObject(housingText, value);
        GameManager.Instance.Food.OnValueChanged += value => UpdateTextObject(foodText, value);
        GameManager.Instance.Wood.OnValueChanged += value => UpdateTextObject(woodText, value);
        GameManager.Instance.Stone.OnValueChanged += value => UpdateTextObject(stoneText, value);
        GameManager.Instance.Metal.OnValueChanged += value => UpdateTextObject(metalText, value);
        GameManager.Instance.Happiness.OnValueChanged += value => UpdateSlider(happinessSlider, value, false);
        GameManager.Instance.Hunger.OnValueChanged += value => UpdateSlider(hungerSlider, value, true);
        
        UpdateShop();
    }

    void Update() {
        if (GameManager.Instance.CurrentCycle >= GameManager.Instance.NightsWithoutWaves)
            UpdateTextObject(waveText, "Wave " + GameManager.Instance.NextWave);
        else
            UpdateTextObject(waveText, "T-" + (GameManager.Instance.NightsWithoutWaves - GameManager.Instance.CurrentCycle) + " Days");
    }

    public static void UpdateTextObject(TextMeshProUGUI textObject, object value, bool updateDimensions = false) {
        if (textObject == null) return;
        
        textObject.text = value.ToString();

        if (!updateDimensions) return;
        
        Vector2 preferredDimensions = textObject.GetPreferredValues();
        textObject.rectTransform.sizeDelta = new(preferredDimensions.x, textObject.rectTransform.sizeDelta.y);
    }

    private void UpdateShop()
    {
        shopPanel.SetActive(IsShopOpen);
        shopButtonImage.gameObject.SetActive(!IsDemolishing);
        shopButtonImage.sprite = IsShopOpen ? closeIconSprite : buttonSprite;
        shopButtonIcon.SetActive(!IsShopOpen);
        
        demolishButtonImage.gameObject.SetActive(!IsShopOpen);
        demolishButtonImage.sprite = IsDemolishing ? closeIconSprite : buttonSprite;
        demolishButtonIcon.SetActive(!IsDemolishing);
    }

    public void OpenShopPanel() => IsShopOpen = true;
    public void CloseShopPanel() => IsShopOpen = false;
    public void ToggleShopPanel() => IsShopOpen = !IsShopOpen;

    public void StartDemolishing() {
        IsDemolishing = true;
        IsShopOpen = false;
    }
    public void StopDemolishing() => IsDemolishing = false;
    public void ToggleDemolishing()
    {
        if (IsDemolishing)
            StopDemolishing();
        else
            StartDemolishing();
    }   


    public static void UpdateSlider(Slider slider, float percentage, bool flipColor) {
        slider.value = percentage / 100f;
        
        Image fill = slider.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();
        fill.color = flipColor
            ? TriColorLerp(Color.green, Color.yellow, Color.red, slider.value)
            : TriColorLerp(Color.red, Color.yellow, Color.green, slider.value);
    }

    private static Color TriColorLerp(Color a, Color b, Color c, float t) {
        if (t > 0.5f)
            return Color.Lerp(b, c, t * 2 - 1f);
        
        return Color.Lerp(a, b, t * 2);
    }

}
