using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class EnergyDoorBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private GameObject[] Sockets;
    private int SocketCount;
    private int filledSockets = 0;
    public GameObject InnerDoor; // reference to the inner door object that will be opened when all sockets are filled
    public GameObject SocketParent;

    public string RequiredKeyType = "EnergyKey"; // the tag that identifies the type of key required to fill the sockets
    private bool DoorOpen = false;
    void Start()
    {
        if (SocketParent != null)
        {
            for (int i = 0; i < SocketParent.transform.childCount; i++)
            {
                if (SocketParent.transform.GetChild(i).gameObject.CompareTag("EnergySocket"))
                {
                    SocketCount++;
                }
            }
            Sockets = new GameObject[SocketCount];
            int currentIndex = 0;
            for (int i = 0; i <  SocketParent.transform.childCount; i++)
            {
                if (SocketParent.transform.GetChild(i).gameObject.CompareTag("EnergySocket"))
                {                    
                    Sockets[currentIndex] = SocketParent.transform.GetChild(i).gameObject;
                    UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socketInteractor = Sockets[currentIndex].GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
                    socketInteractor.selectEntered.AddListener((args) => OnTriggerEnter(args.interactableObject.transform.GetComponent<Collider>()));
                    socketInteractor.selectExited.AddListener((args) => OnTriggerExit(args.interactableObject.transform.GetComponent<Collider>()));
                    currentIndex++;
                    Debug.Log("EnergyDoorBehavior: Found socket " + Sockets[currentIndex - 1].name + " for door " + gameObject.name);
                } // not all children of SocketParent may be sockets, so we check the tag before adding to the array
            }
        }
        else
        {
            Debug.LogError("EnergyDoorBehavior: SocketParent reference not set in inspector.");
        }
        Debug.Log("EnergyDoorBehavior: Found " + SocketCount + " sockets for door " + gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        if (DoorOpen)
            return; // if door is already open, no need to check sockets
        // Check if all sockets are filled with energy keys.
        bool allSocketsFilled = filledSockets >= SocketCount;
        //Debug.Log("EnergyDoorBehavior: Filled sockets " + filledSockets + "/" + SocketCount + " for door " + gameObject.name);
        // If all sockets are filled, open the door
        if (allSocketsFilled)
        {
            ConsumeSocketKeys();
            InnerDoor.SetActive(false); // deactivate the inner door to "open" it
            SocketParent.SetActive(false); // optionally deactivate the sockets as well
            DoorOpen = true;
        }
    }

    void ConsumeSocketKeys()
    {
        if (Sockets == null)
            return;

        for (int i = 0; i < Sockets.Length; i++)
        {
            var socket = Sockets[i];
            if (socket == null)
                continue;

            var socketInteractor = socket.GetComponent<XRSocketInteractor>();
            if (socketInteractor == null || !socketInteractor.hasSelection)
                continue;

            for (int j = socketInteractor.interactablesSelected.Count - 1; j >= 0; j--)
            {
                var selectedInteractable = socketInteractor.interactablesSelected[j];
                if (selectedInteractable == null)
                    continue;

                var selectedObject = selectedInteractable.transform != null ? selectedInteractable.transform.gameObject : null;
                if (selectedObject == null || !selectedObject.CompareTag(RequiredKeyType))
                    continue;

                Destroy(selectedObject);
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(RequiredKeyType))
        {
            Debug.Log("EnergyDoorBehavior: " + RequiredKeyType + " " + other.gameObject.name + " entered socket parent " + SocketParent.name);
            filledSockets++;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(RequiredKeyType))
        {
            Debug.Log("EnergyDoorBehavior: " + RequiredKeyType + " " + other.gameObject.name + " exited socket parent " + SocketParent.name);
            filledSockets--;
        }
    }
}
