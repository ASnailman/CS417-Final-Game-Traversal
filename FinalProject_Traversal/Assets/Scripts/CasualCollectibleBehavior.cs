using UnityEngine;

public class CasualCollectibleBehavior : MonoBehaviour
{
    public float floatingRotationSpeed = 30f; // Speed at which the collectible rotates (degrees per second)
    public float floatingAmplitude = 0.01f; // Amplitude of the floating motion
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool isCollected = false;
    public GameObject selfObject;
    public string loreMessage;
    void Start()
    {
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener((args) => OnTriggerEnter(args.interactableObject.transform.GetComponent<Collider>()));
        }
        else
        {
            Debug.LogWarning("CasualCollectibleBehavior: No XRGrabInteractable component found on " + gameObject.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCollected)
        {
            // Rotate the collectible around the Y-axis
            transform.Rotate(Vector3.up, floatingRotationSpeed * Time.deltaTime);

            // Create a floating motion using a sine wave
            float newY = Mathf.Sin(Time.time) * floatingAmplitude;
            Vector3 newPosition = new Vector3(transform.position.x, transform.position.y + newY, transform.position.z);
            transform.position = newPosition;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("CasualCollectibleBehavior: OnTriggerEnter called for " + gameObject.name + " with collider " + other.name);
        isCollected = true; // Mark the collectible as collected
        if (selfObject.CompareTag("Star"))
        {
            GameStateManager.Instance.StarsCollected++;
            GameStateManager.Instance.currentScore += 500;
        }
        else if (selfObject.CompareTag("EnergyCube"))
        {
            GameStateManager.Instance.Energy++;
            GameStateManager.Instance.currentScore += 500;
        }
        else if (selfObject.CompareTag("Lore"))
        {
            GameStateManager.Instance.DisplayLore(loreMessage);
        }
        else
        {
            Debug.LogWarning("CasualCollectibleBehavior: Collectible " + gameObject.name + " has unrecognized tag " + gameObject.tag);
        }
        Debug.Log("CasualCollectibleBehavior: Collectible " + gameObject.name + " collected by player.");
        Destroy(gameObject); // Remove the collectible from the scene
    }
}
