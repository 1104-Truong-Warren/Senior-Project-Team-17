using UnityEngine;

public class PlayerReactionKeyInput : MonoBehaviour
{
    [Header("Player Reaction Key settings")]
    [SerializeField] private KeyCode dodgeKey; // what to press for player dodge'
    [SerializeField] private KeyCode takeHitKey; // what to press
    [SerializeField] private KeyCode counterAttkKey; // what to press to counter attack

    private void Update()
    {
        // if the turnManager state is not playerReaction state get out
        if (TurnManager.Instance.State != TurnState.PlayerReaction) return;

        // dodge key pressed call dodge function
        if (Input.GetKeyDown(dodgeKey))
            TurnManager.Instance.PlayerDodgeReaction();

        // take dmg call tank dmg function
        else if (Input.GetKeyDown(takeHitKey))
            TurnManager.Instance.PlayerTankDamageReaction();

        // counter attack call counter attack function
        else if (Input.GetKeyDown(counterAttkKey))
            TurnManager.Instance.PlayerCounterAttackReaction();
           
    }
}
