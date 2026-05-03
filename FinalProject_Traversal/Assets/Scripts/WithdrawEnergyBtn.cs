using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class WithdrawEnergyBtn : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Sprite activeSprite;
    public Sprite inactiveSprite;
    public GameObject EnergyKeyPrefab; // Prefab for the energy key to spawn when withdrawing energy
    public CharacterController playerController; // Reference to the player's CharacterController for spawning energy key above the player
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
    public void SpawnEnergy()
    {
        int currentEnergy = GameStateManager.Instance.Energy;
        if (currentEnergy > 0)
        {
            GameStateManager.Instance.Energy -= 1;
            if (GameStateManager.Instance.energyUIText != null)
            {
                GameStateManager.Instance.energyUIText.text = "Energy: " + GameStateManager.Instance.Energy + "/" + GameStateManager.Instance.maxEnergy;
            }
            else
            {
                Debug.LogWarning("WithdrawEnergyBtn: energyUIText reference not set in GameStateManager.");
            }
            if (EnergyKeyPrefab != null)
            {
                Instantiate(EnergyKeyPrefab, playerController.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("WithdrawEnergyBtn: EnergyKeyPrefab reference not set in inspector.");
            }
        }
    }
}
