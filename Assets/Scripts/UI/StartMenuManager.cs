using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour
{

    public void Play() => SceneManager.LoadScene(1);
    
    public void Tutorial() => SceneManager.LoadScene(2);
}
