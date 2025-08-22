using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject credits;

    public void Play() => SceneManager.LoadScene(1);
    
    public void Tutorial() => SceneManager.LoadScene(2);
    
    public void OpenCredits() => credits.SetActive(true);
    public void CloseCredits() => credits.SetActive(false);
}
