using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable))]
public class SwitchPokeRotator : MonoBehaviour
{
    [Tooltip("The visual part that should rotate (eg 'switch_visual').")]
    public Transform rotatingPart;

    [Tooltip("Optional: the interactable part to also rotate (defaults to the GameObject this script is on).")]
    public Transform interactablePart;

    [Tooltip("Identifier for this lever (used to update GameStateManager).")]
    public string leverID;

    [Tooltip("Pivot point to rotate around. If empty, uses rotatingPart.parent (or rotatingPart if no parent).")]
    public Transform pivot;

    [Tooltip("Rotation axis in pivot's local space.")]
    public Vector3 axis = Vector3.right;

    [Tooltip("Angle (degrees) to rotate when flicked on/off.")]
    public float angleDegrees = 30f;

    [Tooltip("Time in seconds to animate the flick.")]
    public float animationDuration = 0.12f;

    readonly List<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable> m_Interactables = new List<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();

    bool m_IsOn = false;
    bool m_IsAnimating = false;

    void Awake()
    {
        CacheInteractables();
    }

    void OnEnable()
    {
        for (int i = 0; i < m_Interactables.Count; i++)
        {
            var interactable = m_Interactables[i];
            if (interactable == null)
                continue;

            interactable.activated.AddListener(OnActivated);
            interactable.selectEntered.AddListener(OnSelectEntered);
        }
    }

    void OnDisable()
    {
        for (int i = 0; i < m_Interactables.Count; i++)
        {
            var interactable = m_Interactables[i];
            if (interactable == null)
                continue;

            interactable.activated.RemoveListener(OnActivated);
            interactable.selectEntered.RemoveListener(OnSelectEntered);
        }
    }

    void Start()
    {
        // If this component was duplicated in-scene and serialized refs point to another instance,
        // rebind to children under this switch instance.
        if (rotatingPart != null && !rotatingPart.IsChildOf(transform))
            rotatingPart = null;
        if (interactablePart != null && !interactablePart.IsChildOf(transform))
            interactablePart = null;
        if (pivot != null && !pivot.IsChildOf(transform))
            pivot = null;

        if (rotatingPart == null)
        {
            // try to auto-find common child names
            var t = transform.Find("switch_visual");
            if (t == null) t = transform.Find("switch_interactable");
            if (t == null) t = transform.Find("interactable");
            if (t != null) rotatingPart = t;
        }

        if (pivot == null && rotatingPart != null)
        {
            pivot = rotatingPart.parent != null ? rotatingPart.parent : rotatingPart;
        }

        if (interactablePart == null)
        {
            var t = transform.Find("switch_interactable");
            if (t == null) t = transform.Find("interactable");
            if (t != null) interactablePart = t;
        }
    }

    void CacheInteractables()
    {
        m_Interactables.Clear();

        var local = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        if (local != null)
            m_Interactables.Add(local);

        var children = GetComponentsInChildren<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>(true);
        for (int i = 0; i < children.Length; i++)
        {
            var childInteractable = children[i];
            if (childInteractable == null)
                continue;

            if (!m_Interactables.Contains(childInteractable))
                m_Interactables.Add(childInteractable);
        }
    }

    void OnActivated(ActivateEventArgs args)
    {
        // keep existing behavior: animated rotation on activation
        if (m_IsAnimating || rotatingPart == null || pivot == null)
            return;

        var targetAngle = m_IsOn ? -angleDegrees : angleDegrees;
        StartCoroutine(AnimateRotateAround(pivot.position, pivot.TransformDirection(axis.normalized), targetAngle, animationDuration));
        m_IsOn = !m_IsOn;
        UpdateGameState();
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        // Treat select (poke contact) as animated activation
        if (m_IsAnimating || rotatingPart == null || pivot == null)
            return;

        var targetAngle = m_IsOn ? -angleDegrees : angleDegrees;
        StartCoroutine(AnimateRotateAround(pivot.position, pivot.TransformDirection(axis.normalized), targetAngle, animationDuration));
        m_IsOn = !m_IsOn;
        UpdateGameState();
    }

    IEnumerator AnimateRotateAround(Vector3 pivotWorldPos, Vector3 axisWorld, float totalAngle, float duration)
    {
        m_IsAnimating = true;
        float elapsed = 0f;
        float sign = Mathf.Sign(totalAngle);
        float remaining = Mathf.Abs(totalAngle);
        while (elapsed < duration)
        {
            float t = Time.deltaTime;
            elapsed += t;
            float step = (Time.deltaTime / duration) * Mathf.Abs(totalAngle);
            // clamp for final step
            if (step > remaining) step = remaining;
            rotatingPart.RotateAround(pivotWorldPos, axisWorld, sign * step);
            if (interactablePart != null && interactablePart != rotatingPart)
                interactablePart.RotateAround(pivotWorldPos, axisWorld, sign * step);
            remaining -= step;
            yield return null;
        }
        // final correction to ensure exact rotation
        if (remaining > 0.0001f)
        {
            rotatingPart.RotateAround(pivotWorldPos, axisWorld, sign * remaining);
            if (interactablePart != null && interactablePart != rotatingPart)
                interactablePart.RotateAround(pivotWorldPos, axisWorld, sign * remaining);
        }

        m_IsAnimating = false;
    }

    void UpdateGameState()
    {
        // Update GameStateManager with the current lever state
        if (string.IsNullOrEmpty(leverID))
            return;

        var gameStateManager = GameStateManager.Instance;
        if (gameStateManager != null)
        {
            gameStateManager.UpdateLeverState(leverID, m_IsOn ? 1 : 0);
            Debug.Log("SwitchPokeRotator: Updated GameStateManager with leverID " + leverID + " state " + (m_IsOn ? 1 : 0));
        }
    }
}
