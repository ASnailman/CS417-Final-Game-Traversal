using UnityEngine;
using Unity.XR.CoreUtils;

public class ReturnToSpawnBtnBehavior : MonoBehaviour
{
    public CharacterController playerController; // Reference to the player's CharacterController for teleporting the player to the spawn point
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ReturnToSpawn()
    {
        int currentRespawnPointID = GameStateManager.Instance.currentrespawnPointID;
        GameObject respawnPoint = GameStateManager.Instance.GetRespawnPoint(currentRespawnPointID);
        if (respawnPoint != null)
        {
            var respawnBehavior = respawnPoint.GetComponent<RespawnPointBehavior>();
            Transform targetTransform = respawnBehavior != null && respawnBehavior.respawnPoint != null
                ? respawnBehavior.respawnPoint
                : respawnPoint.transform;

            var xrOrigin = playerController != null ? playerController.GetComponentInParent<XROrigin>() : null;

            playerController.enabled = false; // Disable the CharacterController before teleporting
            if (xrOrigin != null)
            {
                xrOrigin.MoveCameraToWorldLocation(targetTransform.position);
            }
            else
            {
                playerController.transform.position = targetTransform.position; // Teleport the player to the respawn point
            }
            playerController.enabled = true; // Re-enable the CharacterController after teleporting
            Debug.Log("ReturnToSpawnBtnBehavior: Player teleported to respawn point " + currentRespawnPointID + " at position " + targetTransform.position);
        }
        else
        {
            Debug.LogWarning("ReturnToSpawnBtnBehavior: No respawn point found for currentrespawnPointID " + currentRespawnPointID);
        }

    }
}
