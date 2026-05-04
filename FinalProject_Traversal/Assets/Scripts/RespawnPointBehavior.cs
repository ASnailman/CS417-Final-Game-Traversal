using UnityEngine;
using TMPro;
using System.Collections.Generic;
public class RespawnPointBehavior : MonoBehaviour
{
    public Transform respawnPoint;
    public CharacterController playerObject;
    public bool isCurrentRespawnPoint = false;
    public int respawnPointID;
    public Material activeMaterial;
    public Material inactiveMaterial;
    public float respawnTriggerTime = 3f;
    public TextMeshProUGUI playerUIText; // Reference to the UI text element to display messages to the player
    private Collider[] childColliders; // Array to hold the colliders of the child objects of the respawn point
    private Renderer[] childRenderers;
    private float timeInTrigger = 0f; // When player stands on the respawn point, this variable will start counting up to respawnTriggerTime. If player leaves before it reaches respawnTriggerTime, it resets to 0.
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private TextMeshProUGUI lblSpawnPoint, lblHint, lblID;
    void Start()
    {
        childColliders = GetComponentsInChildren<Collider>();
        childRenderers = GetComponentsInChildren<Renderer>();
        Debug.Log("Respawn point has " + childColliders.Length + " child colliders.");
        if (playerObject == null)
        {
            playerObject = FindFirstObjectByType<CharacterController>();
        }
        lblSpawnPoint = transform.Find("RespawnPointCanvas/LblSpawnPoint").GetComponent<TextMeshProUGUI>();
        lblHint = transform.Find("RespawnPointCanvas/LblHint").GetComponent<TextMeshProUGUI>();
        lblID = transform.Find("RespawnPointCanvas/LblID").GetComponent<TextMeshProUGUI>();
        if (lblSpawnPoint == null || lblHint == null || lblID == null)
        {
            Debug.LogError("One or more UI text elements are missing in the RespawnPointCanvas.");
        }
        else
        {
            lblSpawnPoint.text = "Spawnpoint";
            lblSpawnPoint.color = Color.black;
            lblHint.text = "Stand for " + respawnTriggerTime + " seconds to set spawn point.";
            lblHint.color = Color.black;
            lblID.text = respawnPointID.ToString();
            lblID.color = Color.white;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!isCurrentRespawnPoint)
        {
            detectStandingOnRespawnPoint();
        }
        
    }
    void detectStandingOnRespawnPoint()
    {
        Transform respawnRoot = transform;  
        if (respawnPoint != null && respawnPoint.GetComponentsInChildren<Collider>().Length > 0)
        {
            respawnRoot = respawnPoint;
        }

        if (playerObject == null)
        {
            return;
        }

        Collider playerCollider = playerObject.GetComponent<Collider>();
        if (playerCollider == null)
        {
            return;
        }

        bool isStandingOnChildTile = false;
        for (int i = 0; i < childColliders.Length; i++)
        {
            Collider childCollider = childColliders[i];
            if (childCollider == null || childCollider.transform == respawnRoot)
            {
                continue;
            }

            if (childCollider.bounds.Intersects(playerCollider.bounds))
            {
                isStandingOnChildTile = true;
                Debug.Log("Player is standing on child tile: " + childCollider.gameObject.name);
                break;
            }
        }

        if (isStandingOnChildTile)
        {
            timeInTrigger += Time.deltaTime;
            if (playerUIText != null)
            {
                playerUIText.text = "Setting Spawnpoint..." + Mathf.CeilToInt((respawnTriggerTime - timeInTrigger)*10)/10f + "s left";
            }
            if (timeInTrigger >= respawnTriggerTime)
            {
                // Set this respawn point as the current one
                isCurrentRespawnPoint = true;
                // Change the material to indicate it's active
                for (int i = 0; i < childRenderers.Length; i++)
                {
                    Renderer childRenderer = childRenderers[i];
                    if (childRenderer != null)
                    {
                        childRenderer.material = activeMaterial;
                    }
                }
                // Optionally, you can also reset the timeInTrigger to prevent multiple triggers
                timeInTrigger = 0f;
                if (playerUIText != null)
                {
                    playerUIText.text = "Respawn point set!";
                    // Clear message after 2 seconds
                    Invoke("ClearPlayerUIText", 1f);
                }
                if (lblHint != null)
                {
                    lblHint.text = "This is your current respawn point.";
                    lblHint.color = Color.black;
                }
                if (lblID != null)
                {
                    lblID.text = respawnPointID.ToString();
                    lblID.color = Color.black;
                }
                GameStateManager.Instance.setCurrentRespawnPoint(respawnPointID);
            }
        }
        else
        {
            if (timeInTrigger > 0f && playerUIText != null)
            {
                playerUIText.text = "Cancelled.";
                playerUIText.color = Color.red;
                // Clear message after 2 seconds
                Invoke("ClearPlayerUIText", 1f);
            }
            timeInTrigger = 0f;
        }
    }
    public void setInactive()
    {
        isCurrentRespawnPoint = false;
        for (int i = 0; i < childRenderers.Length; i++)
        {
            Renderer childRenderer = childRenderers[i];
            if (childRenderer != null)
            {
                childRenderer.material = inactiveMaterial;
            }
        }
        if (lblHint != null)
        {
            lblHint.text = "Stand for " + respawnTriggerTime + " seconds to set spawn point.";
            lblHint.color = Color.black;
        }
        if (lblID != null)
        {
            lblID.text = respawnPointID.ToString();
            lblID.color = Color.white;
        }
        if (lblSpawnPoint != null)
        {
            lblSpawnPoint.text = "Spawnpoint";
            lblSpawnPoint.color = Color.black;
        }
    }
    void ClearPlayerUIText()
    {
        if (playerUIText != null)
        {
            playerUIText.text = "";
            playerUIText.color = Color.green;
        }
    }
}
