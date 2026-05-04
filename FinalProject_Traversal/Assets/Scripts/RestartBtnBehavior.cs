using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartBtnBehavior : MonoBehaviour
{
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}