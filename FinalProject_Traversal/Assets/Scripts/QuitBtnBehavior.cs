using UnityEngine;

public class QuitBtnBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void QuitGame()
    {
        if (Application.isEditor)
        {
            // If we're running in the Unity editor, stop playing
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            // If we're running in a built application, quit the game
            Application.Quit();
        }
    }
}
