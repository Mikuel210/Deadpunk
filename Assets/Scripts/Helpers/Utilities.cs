using CodeMonkey.Utils;
using Helpers;
using TMPro;
using UnityEngine;

public class Utilities : Singleton<Utilities>
{
    public static GameObject popupPrefab;
    [SerializeField] private GameObject _popupPrefab;
    
    void Awake() => popupPrefab = _popupPrefab;

    public static void Popup(string text, Vector3 position, Color? color = null, float size = 0.01f)
    {
        if (color == null) color = Color.white;
        
        GameObject canvas = Instantiate(popupPrefab, position + Vector3.up, Quaternion.identity);
        RectTransform rectTransform = canvas.GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.one * size;
        
        TextMeshProUGUI textMesh = canvas.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        textMesh.text = text;
        textMesh.color = color.Value;

        float startingTime = 0.75f;
        float time = startingTime;
        
        FunctionUpdater.Create(() => {
            // Look at camera
            Transform cameraPosition = Camera.main.transform;
            canvas.transform.LookAt(cameraPosition.forward - cameraPosition.position);
            canvas.transform.eulerAngles = new(canvas.transform.eulerAngles.x, 0, 0);
            
            // Update position and color
            rectTransform.position += new Vector3(0, Time.unscaledDeltaTime / startingTime * 10f);
            textMesh.color = new(textMesh.color.r, textMesh.color.g, textMesh.color.b, textMesh.color.a - Time.unscaledDeltaTime / startingTime);
            
            time -= Time.unscaledDeltaTime;
            if (time <= 0f)
            {
                Object.Destroy(canvas);
                return true;
            }

            return false;
        }, "WorldTextPopup");
    }
}