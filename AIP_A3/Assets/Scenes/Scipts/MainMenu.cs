using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Master");
    }

    public void QuitGame()
    {Application.Quit();}
}

