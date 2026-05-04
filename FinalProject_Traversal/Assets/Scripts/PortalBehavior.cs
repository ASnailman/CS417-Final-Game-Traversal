using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;
using Unity.XR.CoreUtils;
using UnityEngine.Events;

public class PortalBehavior : MonoBehaviour
{
    public Transform targetTeleportLocation;
    public float facingYawOffsetDegrees = 0f;
    public GameObject effectPrefab;
    public AudioClip teleportSound;
    private AudioSource audioSource;
    private ContinuousMoveProvider moveProvider;
    private bool isTeleporting;
    public GameObject[] objectsToDisableDuringTeleport;

    public Camera portalCamera; // Camera at the destination
    public Renderer portalScreen; // Screen to display the portal view
    public UnityEvent onPortalEntered;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Set up the portal camera render texture
        if (portalCamera != null && portalScreen != null)
        {
            RenderTexture portalTexture = new RenderTexture(Screen.width, Screen.height, 24);
            portalCamera.targetTexture = portalTexture;
            portalScreen.material.mainTexture = portalTexture;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || isTeleporting)
        {
            return;
        }

        if (targetTeleportLocation == null)
        {
            Debug.LogWarning("PortalBehavior: targetTeleportLocation is not assigned.");
            return;
        }

        var rigTransform = ResolveRigTransform(other.transform);
        var xrOrigin = rigTransform.GetComponent<XROrigin>();
        var characterController = rigTransform.GetComponent<CharacterController>();
        var headTransform = ResolveHeadTransform(xrOrigin);

        if (characterController == null)
        {
            Debug.LogWarning("PortalBehavior: CharacterController not found on XR rig root. Teleport aborted.");
            return;
        }

        // Find any continuous move provider on the XR rig (e.g. DynamicMoveProvider in XRI 3.3.1).
        moveProvider = rigTransform.GetComponentInChildren<ContinuousMoveProvider>(true);

        Debug.Log($"Player entered portal. Rig '{rigTransform.name}', Move provider found: {moveProvider != null}");
        onPortalEntered?.Invoke();
        StartCoroutine(TeleportPlayer(characterController, headTransform));
    }

    private Transform ResolveRigTransform(Transform hitTransform)
    {
        var xrOrigin = hitTransform.GetComponentInParent<XROrigin>();
        if (xrOrigin != null)
        {
            return xrOrigin.transform;
        }

        return hitTransform.root;
    }

    private Transform ResolveHeadTransform(XROrigin xrOrigin)
    {
        if (xrOrigin != null && xrOrigin.Camera != null)
        {
            return xrOrigin.Camera.transform;
        }

        return Camera.main != null ? Camera.main.transform : null;
    }

    private IEnumerator TeleportPlayer(CharacterController characterController, Transform headTransform)
    {
        isTeleporting = true;

        // Disable specified GameObjects
        foreach (var obj in objectsToDisableDuringTeleport)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        // Disable movement
        if (moveProvider != null) moveProvider.enabled = false;

        // Play the teleport effect and sound
        if (effectPrefab != null)
        {
            Instantiate(effectPrefab, characterController.transform.position, Quaternion.identity);
        }
        if (teleportSound != null)
        {
            audioSource.PlayOneShot(teleportSound);
        }

        var rigTransform = characterController.transform;
        var xrOrigin = rigTransform.GetComponent<XROrigin>();

        Vector3 targetForward = GetCardinalDirection(targetTeleportLocation.forward, facingYawOffsetDegrees);

        characterController.enabled = false;
        if (xrOrigin != null)
        {
            xrOrigin.MoveCameraToWorldLocation(targetTeleportLocation.position);
            xrOrigin.MatchOriginUpCameraForward(Vector3.up, targetForward);
        }
        else
        {
            rigTransform.position = targetTeleportLocation.position;
            rigTransform.rotation = Quaternion.LookRotation(targetForward, Vector3.up);
        }
        characterController.enabled = true;

        // Wait for a frame to ensure the player is in the correct position
        yield return null;

        // Reapply the facing after XR/physics updates so the rotation persists across repeated teleports.
        characterController.enabled = false;
        if (xrOrigin != null)
        {
            xrOrigin.MatchOriginUpCameraForward(Vector3.up, targetForward);
        }
        else
        {
            rigTransform.rotation = Quaternion.LookRotation(targetForward, Vector3.up);
        }
        characterController.enabled = true;

        // Re-enable specified GameObjects
        foreach (var obj in objectsToDisableDuringTeleport)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        // Re-enable movement
        if (moveProvider != null) moveProvider.enabled = true;

        isTeleporting = false;
    }

    private static Vector3 GetCardinalDirection(Vector3 forward, float yawOffsetDegrees)
    {
        forward = Quaternion.Euler(0f, yawOffsetDegrees, 0f) * forward;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.0001f)
        {
            return Vector3.forward;
        }

        forward.Normalize();

        Vector3 east = Vector3.right;
        Vector3 west = Vector3.left;
        Vector3 north = Vector3.forward;
        Vector3 south = Vector3.back;

        Vector3 bestDirection = north;
        float bestDot = Vector3.Dot(forward, north);

        float dot = Vector3.Dot(forward, east);
        if (dot > bestDot)
        {
            bestDot = dot;
            bestDirection = east;
        }

        dot = Vector3.Dot(forward, south);
        if (dot > bestDot)
        {
            bestDot = dot;
            bestDirection = south;
        }

        dot = Vector3.Dot(forward, west);
        if (dot > bestDot)
        {
            bestDirection = west;
        }
        Debug.Log($"Best cardinal direction: {bestDirection}, dot: {bestDot}");
        return bestDirection;
    }
}