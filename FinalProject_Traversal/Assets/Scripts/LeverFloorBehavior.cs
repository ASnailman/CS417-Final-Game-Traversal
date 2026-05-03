using UnityEngine;
using TMPro;
public class LeverFloorBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string leverID;
    public TextMeshProUGUI floorLabel;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (floorLabel != null)
        {
            GameStateManager.LeverState currentState = GameStateManager.Instance.GetLeverState(leverID);
            if (currentState != null)
            {
                floorLabel.text = leverID +"="+ currentState.state.ToString();
            }
            else
            {
                floorLabel.text = leverID + " not found";
                Debug.LogWarning("LeverFloorBehavior: No matching lever state found for leverID " + leverID);
            }
        }
    }
}
