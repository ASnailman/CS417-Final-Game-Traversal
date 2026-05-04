using UnityEngine;

public class GoalZoneBehavior : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameStateManager.Instance.DisplayLore("GOAL REACHED! LEVEL CLEARED.");
        }
    }
}