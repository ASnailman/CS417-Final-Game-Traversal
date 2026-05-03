using UnityEngine;

public class EnergyDoorBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private GameObject[] Sockets;
    private int SocketCount;
    public GameObject InnerDoor; // reference to the inner door object that will be opened when all sockets are filled
    public GameObject SocketParent;
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
        // Check if all sockets are filled with energy keys
        bool allSocketsFilled = true;
        for (int i = 0; i < SocketCount; i++)
        {
            Debug.Log("EnergyDoorBehavior: Checking socket " + Sockets[i].name + " for door " + gameObject.name + " (child count: " + Sockets[i].transform.childCount + ")");
            if (Sockets[i].transform.childCount == 0)
            {
                allSocketsFilled = false;
                break;
            }
        }
        // If all sockets are filled, open the door
        if (allSocketsFilled)
        {
            InnerDoor.SetActive(false); // deactivate the inner door to "open" it
            SocketParent.SetActive(false); // optionally deactivate the sockets as well
            DoorOpen = true;
        }
    }
}
