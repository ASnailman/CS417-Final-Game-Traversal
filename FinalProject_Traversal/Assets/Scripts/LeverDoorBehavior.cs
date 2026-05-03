using UnityEngine;
using TMPro;
public class LeverDoorBehavior : MonoBehaviour
{
    public string leverID;
    public int requiredLeverState; // only active when leverID.state == requiredLeverState
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TextMeshProUGUI[] DoorLabels;
    void Start()
    {
        foreach (TextMeshProUGUI label in DoorLabels)
        {
            if (label != null)
            {
                label.text = leverID + requiredLeverState.ToString();
            }
             else
            {
                Debug.LogError("One or more DoorLabels are not assigned in the inspector.");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
