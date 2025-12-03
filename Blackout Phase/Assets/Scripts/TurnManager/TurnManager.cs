using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public enum TurnState
{
    MapLoading, // loads map
    PlayerStart, // player reset AP
    PlayerAction, // spending AP
    PlayerEnd, // passing to enemy
    EnemyStart, // initialize enemies
    EnemyAction, // attack/patrol/chase
    EnemyEnd, // back to player
    GameOver, // when player dies
    UI       // UI
}

public class TurnManager : MonoBehaviour
{
    [Header("Enemies")]
    [SerializeField] private List<EnemyController> enemies = new List<EnemyController>(); // set up the enmey controller

    public static TurnManager Instance { get; private set; }  // accessor for other scripts
    public TurnState State { get; private set; } = TurnState.MapLoading; // state controls the turn using finite state, starts with loading in

    //public bool IsPlayerTurn => CurrentPhase == TurnPhase.Player; // flag to check if player's turn?

    private bool isInitialized = false; // check too see if enemies are initialized

    private void Awake()
    {
        if (Instance != null && Instance != this)  // if gameobject not found destory it, else set it to this
        {
            Destroy(gameObject);

            return;
        }

        Instance = this; // found set it up

        //isInitialized = true; // toggle flag everything is set up

        DontDestroyOnLoad(gameObject); // keeps the game object

        Debug.Log("TurnManager Awake"); // test
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => MapManager.Instance != null && 
                                         MapManager.Instance.map != null &&
                                         MapManager.Instance.map.Count > 0); // wait until the map is set

        Debug.Log("Turnmanager: Map is ready"); // debug msg

        //yield return new WaitUntil(() => AllEnemiesReady()); // wait until enemies are set up

        //CurrentPhase = TurnPhase.Player; // Palyer can not start turn

        EnemyController[] found = FindObjectsByType<EnemyController>(FindObjectsSortMode.InstanceID); // got through the list and find enemies

        enemies.AddRange(found); // add the nemeies

        Debug.Log($"TurnManager: Found {enemies.Count} enemies in scene."); // debug

        isInitialized = true; // flag everything is set up

        Debug.Log("Turnmanager initialized"); // debug msg

        SetTurnState(TurnState.PlayerAction); // starting the state Player
    }

    //private void Update()
    //{
    //    if (!isInitialized) return; // not initialized return

    //    if (CurrentPhase == TurnPhase.Player && Input.GetKeyDown(KeyCode.Space)) // space ends the player's turn, if is player's turn
    //        EndPlayerTurn();

    //    //Debug.Log($"{name} turn ended.");
    //}

    private void Update()
    {
        // get out if is not set up
        if (!isInitialized) return;

        // spaces ends player's turn
        if (State == TurnState.PlayerAction && Input.GetKeyDown(KeyCode.Space))  // End player's turn by using space, can but change to UI instead
            SetTurnState(TurnState.PlayerEnd); //EndPlayerTurn(); // ends player's turn
    }

    // use finite state to control the turn
    public void SetTurnState(TurnState newState)
    {
        Debug.Log($"TurnManger => State shift:{State} => {newState}"); // display the sate changes

        State = newState; // current state to a new state

        // now by using a switch statement we can link each state to the correct function call
        switch (State)
        {
            case TurnState.PlayerStart: // player starts the turn, reset AP
                PlayerTurnStart(); // start
                break;

            case TurnState.PlayerAction: // since the mouse controlls the turn do nothing, once AP used end, or manully end
                break; 

            case TurnState.PlayerEnd: // player turn ended -> calls enemy turn to start
                StartCoroutine(EnemyTurnStart()); // continue
                break;

            case TurnState.EnemyStart: // since enemies are initials display a msg => goes to EnemyAction
                Debug.Log("Enemies Ready!");
                //SetTurnState(TurnState.EnemyAction); // state changed to enemyAction
                break;

            case TurnState.EnemyAction: // starting the enemy action chase/attk/patrol
                StartCoroutine(EnemyTurnAction()); // contine
                break;

            case TurnState.EnemyEnd: // new cycle enemy turn ends => player's turn
                SetTurnState(TurnState.PlayerStart); // cycle starts
                break;

            case TurnState.GameOver: // if player died/didn't meet requirements 
                Debug.Log("GAME OVER!");
                break;              
        }

        // if player is died but the state is not in game over 
        if (State != TurnState.GameOver && CharacterInfo.Instance.hp <= 0)
        {
            State = TurnState.GameOver; // change it to Game over state

            Debug.Log("TurnManager: Player died => GAME OVER...."); // debug

            return;
        }
    }

    //private bool AllEnemiesReady()
    //{
    //    foreach (var enemy in enemies) // use a loop to check if enemies ar setup
    //    {
    //        if (enemy == null) continue; // if not found skip

    //        else if (!enemy.Initialized) return false; // if not initialized set it to false
    //    }

    //    return true;// else true
    //}

    public void RegisterEnemy(EnemyController enemy)
    {
        if (!enemies.Contains(enemy)) // enemies not found add them
        {
            enemies.Add(enemy); // add enemies

            Debug.Log($"TurnManager: Registerd enemy {enemy.name}"); // debug
        }
    }

    private void PlayerTurnStart()
    {
        CharacterInfo.Instance.ResetAP(); // resets the AP at the beginning of the turn

        Debug.Log("Player AP reset to: " + CharacterInfo.Instance.currentAP); // shows current AP at the begginer of the turn

        // current state is not playerAcution? set it to playerAction
        if (State != TurnState.PlayerAction)
            SetTurnState(TurnState.PlayerAction); // state now player action
    }

    public void EndPlayerTurn()
    {
        if (State != TurnState.PlayerAction) return; // not player's turn get out

        Debug.Log("Player turn Ended -> Enemy Phase Starting");

        // before ending player's turn check if it died
        if (CharacterInfo.Instance.hp <= 0)
        {
            SetTurnState(TurnState.GameOver); // if player died game over state
            return; // get out
        }

        //StartCoroutine(EnemyPhase()); // Enemy's phase

        SetTurnState(TurnState.PlayerEnd); // set to player end state 
    }

    public void PlayerSpendAP(int amount)
    {
        CharacterInfo.Instance.ApUsed(amount); // instead of directly accessing link the spend through this function

        Debug.Log($"Player Spent {amount}AP, Remaining: {CharacterInfo.Instance.currentAP}"); // spend AP, AP left

        CheckPlayerAP(); // check if player still have AP left
    }

    public void CheckPlayerAP()
    {
        // if currently not player's turn get out
        if (State != TurnState.PlayerAction) return; 

        // if the player AP this turn is 0 end turn
        if (CharacterInfo.Instance.currentAP <= 0)
        {
            Debug.Log("Player is out of AP, ending your end.");

            //EndPlayerTurn(); // force to end the player's turn

            SetTurnState(TurnState.PlayerEnd); // if the AP = 0 player turn ends state change
        }
    }

    private IEnumerator EnemyTurnStart()
    {
        //SetTurnState(TurnState.EnemyStart); // state is now enemy start

        Debug.Log("Enemy turn Start"); // debug msg

        yield return new WaitForSeconds(0.2f);  // 0.2 seconds delay

        SetTurnState(TurnState.EnemyAction); // state to enemy action
    }

    private IEnumerator EnemyTurnAction()
    {
        //CurrentPhase = TurnPhase.Enemy; // currently enemy's phase

        //Debug.Log("Enemy Phase Start"); // debug

        foreach (EnemyController enemy in enemies)   // each enemies take a turn, if not found continue next
        {
            if (enemy == null) continue; // if enemy is not found skip ingore
            
            Debug.Log($"TurnManager: Enemy taking turn -> {enemy.name}"); // which enemy

            yield return StartCoroutine(enemy.TakeTurn()); // each enemy 
            yield return new WaitForSeconds(0.1f); // another delay   
            
        }
        Debug.Log("All enemies completed their turns -> Player turn Starting"); // debug

        SetTurnState(TurnState.EnemyEnd); // enemy turn ended

        //CurrentPhase = TurnPhase.Player; // back to player's phase after enmey's turn

        //PlayerTurnStart(); // resets the AP

        //yield break; 
    }

    // Add this to your UI script
    
    //public void OnEndTurnButton()
    //{
    //    TurnManager.Instance.EndPlayerTurn(); // ends player's turn
    //}
}


