using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class WithdrawEnergyBtn : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Sprite activeSprite;
    public Sprite inactiveSprite;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int currentEnergy = GameStateManager.Instance.Energy;
        if (currentEnergy == 0)
        {
            GetComponent<Button>().interactable = false;
                if (inactiveSprite != null)
                {
                    GetComponent<Image>().sprite = inactiveSprite;
                }
                else
                {
                    Debug.LogWarning("WithdrawEnergyBtn: Inactive sprite not assigned in inspector.");
                }
        }
        else
        {
            GetComponent<Button>().interactable = true;
            if (activeSprite != null)
            {
                GetComponent<Image>().sprite = activeSprite;
            }
            else
            {
                Debug.LogWarning("WithdrawEnergyBtn: Active sprite not assigned in inspector.");
            }
        }
    }
}
