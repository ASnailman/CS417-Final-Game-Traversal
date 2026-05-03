using UnityEngine;
using TMPro;
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject Levers; // parent object containing all levers in the scene
    public GameObject Doors;
    public GameObject RespawnPointsParent; // parent object containing all respawn points in the scene
    public int currentrespawnPointID = 0;
    private GameObject[] respawnPoints;
    private GameObject[] doors;
    public class LeverState
    {
        public string leverID;
        public int state;
    }
    public LeverState[] leverStates;
    public int Energy = 0;
    public int maxEnergy = 5;
    public TextMeshProUGUI energyUIText; // Reference to the UI text element to display energy to the player

    void Start()
    {
        leverStates = new LeverState[Levers.transform.childCount];
        for (int i = 0; i < Levers.transform.childCount; i++)
        {
            var lever = Levers.transform.GetChild(i).GetComponent<SwitchPokeRotator>();
            if (lever != null)
            {
                leverStates[i] = new LeverState { leverID = lever.leverID, state = 0 };
            }
            else
            {
                Debug.LogWarning("GameStateManager: SwitchPokeRotator component not found on child " + i);
            }
        }
        foreach (var leverState in leverStates)
        {
            Debug.Log("Initialized lever state: " + leverState.leverID + " = " + leverState.state);
        }
        for (int i=0; i < Doors.transform.childCount; i++)
        {
            var door = Doors.transform.GetChild(i).gameObject;
            if (door != null)
            {
                // ensure doors array is initialized
                if (doors == null)
                    doors = new GameObject[Doors.transform.childCount];
                doors[i] = door;
            }
            else
            {
                Debug.LogWarning("GameStateManager: Door GameObject not found on child " + i);
            }
        }
        for (int i=0; i < RespawnPointsParent.transform.childCount; i++)
        {
            var respawnPoint = RespawnPointsParent.transform.GetChild(i).gameObject;
            if (respawnPoint != null)
            {
                // ensure respawnPoints array is initialized
                if (respawnPoints == null)
                    respawnPoints = new GameObject[RespawnPointsParent.transform.childCount];
                respawnPoints[i] = respawnPoint;
            }
            else
            {
                Debug.LogWarning("GameStateManager: Respawn point GameObject not found on child " + i);
            }
        }
        Debug.Log("GameStateManager initialized with " + leverStates.Length + " levers, " + doors.Length + " doors, and " + respawnPoints.Length + " respawn points.");
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i=0; i < doors.Length; i++)
        {
            var door = doors[i];
            var leverDoorBehavior = door.GetComponent<LeverDoorBehavior>();
            if (leverDoorBehavior != null)
            {
                var leverState = GetLeverState(leverDoorBehavior.leverID);
                if (leverState != null)
                {
                    //Debug.Log("Checking door " + door.name + " with leverID " + leverDoorBehavior.leverID + " (state " + leverState.state + ") against required state " + leverDoorBehavior.requiredLeverState);
                    if (leverState.state != leverDoorBehavior.requiredLeverState)
                    {
                        // Activate the door (e.g., open it)
                        door.SetActive(false); // Replace with animation if needed
                    }
                    else
                    {
                        // Deactivate the door (e.g., close it)
                        door.SetActive(true); // Replace with animation if needed
                    }
                }
                else
                {
                    Debug.LogWarning("GameStateManager: No matching lever state found for door " + door.name);
                }
            }
            else
            {
                Debug.LogWarning("GameStateManager: LeverDoorBehavior component not found on door " + door.name);
            }
        }
        if (energyUIText != null)
        {
            energyUIText.text = "Energy: " + Energy + "/" + maxEnergy;
        }
    }

    void SetLeverState(string leverID, int newState)
    {
        var leverState = System.Array.Find(leverStates, ls => ls.leverID == leverID);
        if (leverState != null)
        {
            leverState.state = newState;
            Debug.Log("Updated lever state: " + leverID + " = " + newState);
        }
        else
        {
            Debug.LogWarning("GameStateManager: No matching lever state found for leverID " + leverID);
        }
    }
    public LeverState GetLeverState(string leverID)
    {
        var leverState = System.Array.Find(leverStates, ls => ls.leverID == leverID);
        if (leverState != null)
        {
            return leverState;
        }
        else
        {
            Debug.LogWarning("GameStateManager: No matching lever state found for leverID " + leverID);
            return null;
        }
    }
    public void UpdateLeverState(string leverID, int state)
    {
        SetLeverState(leverID, state);
    }
    public void setCurrentRespawnPoint(int respawnPointID)
    {
        if (respawnPointID < 0 || respawnPointID >= respawnPoints.Length)
        {
            Debug.LogWarning("GameStateManager: Invalid respawn point ID " + respawnPointID);
            return;
        }
        currentrespawnPointID = respawnPointID;
        Debug.Log("Current respawn point set to ID " + currentrespawnPointID);
        for (int i = 0; i < respawnPoints.Length; i++)
        {
            var respawnBehavior = respawnPoints[i].GetComponent<RespawnPointBehavior>();
            if (respawnBehavior != null)
            {
                if (respawnBehavior.respawnPointID != currentrespawnPointID)
                {
                    respawnBehavior.setInactive();
                }
            }
            else
            {
                Debug.LogWarning("GameStateManager: RespawnPointBehavior component not found on respawn point " + respawnPoints[i].name);
            }
        }
    }
}
