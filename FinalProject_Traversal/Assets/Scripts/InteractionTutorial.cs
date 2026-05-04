using UnityEngine;

public class InteractionTutorial : MonoBehaviour
{
    public GameObject targetUIText;
    private bool hasShown = false;

    void Start()
    {
        // Ensure it starts hidden
        if (targetUIText != null) targetUIText.SetActive(false);
    }

    public void TriggerTutorial()
    {
        if (!hasShown && targetUIText != null)
        {
            hasShown = true;
            targetUIText.SetActive(true);
            Invoke("HideTutorial", 2f); // Hides exactly after 2 seconds
        }
    }

    private void HideTutorial()
    {
        if (targetUIText != null) targetUIText.SetActive(false);
    }
}