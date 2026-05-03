using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
public class WithdrawEnergyBtn : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Sprite activeSprite;
    public Sprite inactiveSprite;
    public GameObject EnergyKeyPrefab; // Prefab for the energy key to spawn when withdrawing energy
    public CharacterController playerController; // Reference to the player's CharacterController for spawning energy key above the player
    [SerializeField] private Transform rightControllerSpawnPoint;
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
                StartCoroutine(SpawnEnergyKeyOnRightHand());
            }
            else
            {
                Debug.LogWarning("WithdrawEnergyBtn: EnergyKeyPrefab reference not set in inspector.");
            }
        }
    }

    IEnumerator SpawnEnergyKeyOnRightHand()
    {
        var spawnPoint = GetRightControllerSpawnPoint();
        if (spawnPoint == null)
        {
            Debug.LogWarning("WithdrawEnergyBtn: Could not find right controller spawn point. Falling back to player controller.");
            spawnPoint = playerController != null ? playerController.transform : transform;
        }

        var spawnPosition = spawnPoint.position + spawnPoint.forward * 0.12f + spawnPoint.up * 0.03f;
        var spawnRotation = spawnPoint.rotation;
        var spawnedKey = Instantiate(EnergyKeyPrefab, spawnPosition, spawnRotation);

        var grabInteractable = spawnedKey.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable == null)
        {
            grabInteractable = spawnedKey.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        }

        if (spawnedKey.GetComponent<Collider>() == null)
        {
            spawnedKey.AddComponent<SphereCollider>();
        }

        var rigidbody = spawnedKey.GetComponent<Rigidbody>();
        if (rigidbody == null)
        {
            rigidbody = spawnedKey.AddComponent<Rigidbody>();
        }

        rigidbody.useGravity = false;
        rigidbody.isKinematic = false;

        yield return null;

        var interactionManager = FindObjectOfType<XRInteractionManager>();
        var interactor = GetRightHandInteractor();
        if (interactionManager != null && interactor != null)
        {
            interactionManager.SelectEnter((UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)interactor, (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)grabInteractable);
        }
        else
        {
            Debug.LogWarning("WithdrawEnergyBtn: Could not auto-grab energy key because the right interactor or interaction manager was not found.");
        }
    }

    Transform GetRightControllerSpawnPoint()
    {
        if (rightControllerSpawnPoint != null)
        {
            return rightControllerSpawnPoint;
        }

        var directFind = GameObject.Find("Right Controller");
        if (directFind != null)
        {
            rightControllerSpawnPoint = directFind.transform;
            return rightControllerSpawnPoint;
        }

        var rigRoot = GameObject.Find("XR Origin (XR Rig)");
        if (rigRoot != null)
        {
            var cameraOffset = rigRoot.transform.Find("Camera Offset");
            if (cameraOffset != null)
            {
                var rightController = cameraOffset.Find("Right Controller");
                if (rightController != null)
                {
                    rightControllerSpawnPoint = rightController;
                    return rightControllerSpawnPoint;
                }
            }
        }

        return null;
    }

    UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor GetRightHandInteractor()
    {
        var spawnPoint = GetRightControllerSpawnPoint();
        if (spawnPoint == null)
            return null;

        var interactors = spawnPoint.GetComponentsInChildren<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor>(true);
        for (int i = 0; i < interactors.Length; i++)
        {
            if (interactors[i] is UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor)
            {
                return interactors[i];
            }
        }

        return interactors.Length > 0 ? interactors[0] : null;
    }
}
