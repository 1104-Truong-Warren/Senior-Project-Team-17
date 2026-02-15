// and some additional fixing from online sources Unity Discussion:https://discussions.unity.com/, reddit, YouTube
// I should have keep tract on the exact page but I forgot to save some of the links 
// This is also a finite state machine took inspiration from CS 456
// this time is constantly updating because turn based games needs a turn manager at all time
// since we have more states this time a switch statement is used to help navigate through out
// the different states, player start -> player action -> player end -> calls the enemy controll
// then the enemy use its own finite state machine to control the action -> enemy ends calls player again
// it just keeps on repeating again until the player dies then it goes into game over state
// Weijun

using UnityEngine; // default
using System.Collections; // for the array list we have also IEnumerator for delay funciton calls yield returns. loading map first then do something else
using System.Collections.Generic;  // for the List<T> and dictionary <T, T> for pathfinding
using Unity.VisualScripting;
using System.Runtime.InteropServices.WindowsRuntime;

public enum TurnState
{
    MapLoading, // loads map
    PlayerSpawn, // let player spawn start point
    PlayerStart, // player reset AP
    PlayerAction, // spending AP
    PlayerReaction, // reaction to enemy attacks
    PlayerEnd, // passing to enemy
    EnemyStart, // initialize enemies
    EnemyAction, // attack/patrol/chase
    EnemyEnd, // back to player
    ClearLevel, // passes the level
    GameOver, // when player dies
    UI       // UI
}

public class TurnManager : MonoBehaviour
{
    [Header("Enemies")]
    [SerializeField] private List<EnemyController1> enemies = new List<EnemyController1>(); // set up the enmey controller

    private readonly List<EnemyController1> pendingEnemyRemovals = new List<EnemyController1>(); // for the dead enemies

    public static TurnManager Instance { get; private set; }  // accessor for other scripts
    public TurnState State { get; private set; } = TurnState.MapLoading; // state controls the turn using finite state, starts with loading in

    public EnemyInfo inComingAttackEnemy { get; private set; } // let otehr scripts to access it

    // player's reaction to enemy attacks
    public int inComingDamage { get; private set; } // let other scripts to access it, the in coming damage 

    public int inComingHitChance { get; private set; } // let other scripts to access the in coming Hit Chance

    public bool WaitForPlayerReact => State == TurnState.PlayerReaction; // set a flag for other script to access if player is in Reaction state

    //public bool IsPlayerTurn => CurrentPhase == TurnPhase.Player; // flag to check if player's turn?

    private bool playerReactionSuccessful = false; // keep track if player's reaction worked

    private bool isInitialized = false; // check too see if enemies are initialized

    private CharacterInfo1 playerInfo; // player's info

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

        PlayerSetUp(); // bool function for player set up
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => MapManager1.Instance != null && 
                                         MapManager1.Instance.map != null &&
                                         MapManager1.Instance.map.Count > 0); // wait until the map is set

        Debug.Log("Turnmanager: Map is ready"); // debug msg

        //yield return new WaitUntil(() => AllEnemiesReady()); // wait until enemies are set up

        //CurrentPhase = TurnPhase.Player; // Palyer can not start turn

        //EnemyController1[] found = FindObjectsByType<EnemyController1>(FindObjectsSortMode.InstanceID); // got through the list and find enemies

        //enemies.AddRange(found); // add the nemeies

        Debug.Log($"TurnManager: Found {enemies.Count} enemies in scene."); // debug

        isInitialized = true; // flag everything is set up

        Debug.Log("Turnmanager initialized"); // debug msg

        SetTurnState(TurnState.PlayerSpawn); // Let Player Spawn
    }

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
            case TurnState.PlayerSpawn: // player spawn before Turn starts
                Debug.Log("Player pick a spawn point");
                break;

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

            case TurnState.ClearLevel: // when player clears the level
                Debug.Log("Next level Ready!");
                // level manger load new level from here
                break;

            case TurnState.GameOver: // if player died/didn't meet requirements 
                Debug.Log("GAME OVER!");
                break;              
        }

        // if player is died but the state is not in game over 
        if (State != TurnState.GameOver && playerInfo != null && playerInfo.CurrentHP <= 0)
        {
            State = TurnState.GameOver; // change it to Game over state

            Debug.Log("TurnManager: Player died => GAME OVER...."); // debug

            return;
        }
    }

    public void RegisterEnemy(EnemyController1 enemy)
    {
        // if enemy is null get out
        if (enemy == null) return; 

        if (!enemies.Contains(enemy)) // enemies not found add them
        {
            enemies.Add(enemy); // add enemies

            Debug.Log($"TurnManager: Registerd enemy {enemy.name}"); // debug
        }
        else
        {
            Debug.Log($"TurnManager: Duplicated enemy! {enemy.name}"); // debug 
        }
    }

    private void PlayerTurnStart()
    {
        // check if the playerInfo finished loading
        if (!PlayerSetUp())
        {
            Debug.LogWarning("TurnManager => Player not ready yet! delay"); // debug msg

            StartCoroutine(WaitForPlayerReady()); // calls the delay function
            return;
        }

        //PlayerFuryMode.Instance.ResetCurrentKills(); // reset the kill counter before each turn

        playerInfo.ResetAP(); // resets the AP at the beginning of the turn

        Debug.Log("Player AP reset to: " + playerInfo.currentAP); // shows current AP at the begginer of the turn

        Debug.Log("Player Current EN: " + playerInfo.CurrentEN); // shows current EN for debug

        // current state is not playerAcution? set it to playerAction
        if (State != TurnState.PlayerAction)
            SetTurnState(TurnState.PlayerAction); // state now player action
    }

    public void EndPlayerTurn()
    {
        if (State != TurnState.PlayerAction) return; // not player's turn get out

        Debug.Log("Player turn Ended -> Enemy Phase Starting");

        // before ending player's turn check if it died
        if (playerInfo.CurrentHP <= 0)
        {
            SetTurnState(TurnState.GameOver); // if player died game over state
            return; // get out
        }

        //StartCoroutine(EnemyPhase()); // Enemy's phase

        SetTurnState(TurnState.PlayerEnd); // set to player end state 
    }

    public void PlayerSpendAP(int amount)
    {
        // if the fury mode is active use this instead of AP points
        if (PlayerFuryMode.Instance.inFuryMode)
        {
            PlayerFuryMode.Instance.FuryModeGoingDown(); // use the RageMode turns
            return;
        }

        playerInfo.ApUsed(amount); // instead of directly accessing link the spend through this function

        Debug.Log($"Player Spent {amount}AP, Remaining: {playerInfo.currentAP}"); // spend AP, AP left

        CheckPlayerAP(); // check if player still have AP left
    }

    public void CheckPlayerAP()
    {
        // if currently not player's turn get out
        if (State != TurnState.PlayerAction) return; 

        // if the player AP this turn is 0 end turn
        if (playerInfo.currentAP <= 0)
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

        var enemyList = enemies.ToArray(); //new List<EnemyController1>(enemies); // svae enemies into a list so it doesn't break the loop

        foreach (EnemyController1 enemy in enemyList)   // each enemies take a turn, if not found continue next
        {
            if (enemy == null) continue; // if enemy is not found skip ingore

            //if (!enemies.Contains(enemy)) continue; // skip the enemies we kill this turn
            
            Debug.Log($"TurnManager: Enemy taking turn -> {enemy.name}"); // which enemy

            yield return StartCoroutine(enemy.TakeTurn()); // each enemy 
            yield return new WaitForSeconds(0.1f); // another delay   

            // if the turn is player Reaction let player make a decision
            if (State == TurnState.PlayerReaction)
            {
                Debug.Log("Player reaction!"); // debug msg
                yield return StartCoroutine(WaitForPlayerReaction());
            }
            
        }
        Debug.Log("All enemies completed their turns -> Player turn Starting"); // debug

        // if enemy remove list is bigger than 0, remove them
        if (pendingEnemyRemovals.Count > 0)
        {
            // use a loop to go through the enemy rmoval list
            foreach (var enemy in pendingEnemyRemovals)
                enemies.Remove(enemy); // remove each enemy

            pendingEnemyRemovals.Clear(); // reset the list
        }

        LevelCleared(); // check if all enemies are defeated

        // if the current state is cleared break
        if (State == TurnState.ClearLevel) yield break; 

        SetTurnState(TurnState.EnemyEnd); // enemy turn ended

        //CurrentPhase = TurnPhase.Player; // back to player's phase after enmey's turn

        //PlayerTurnStart(); // resets the AP

        //yield break; 
    }

    private void LevelCleared()
    {
        Debug.Log($"Enemies left before:{enemies.Count}"); // debug msg

        enemies.RemoveAll(E  => E == null); // remove all null enemies, dead enemies are destroyed when killed

        Debug.Log($"Enemies left after:{enemies.Count}"); // debug msg

        //  enemies check if they are equal to 0 level cleared
        if (enemies.Count == 0)
        {
            Debug.Log("Level Cleared!!!!");

            SetTurnState(TurnState.ClearLevel); // set it to level cleared
            return;
        }
    }

    private bool PlayerSetUp()
    {
        //playerInfo = GetComponent<CharacterInfo1>(); // set up playerInfo 

        // if the playerInfo was found return 1
        if (playerInfo != null) return true;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player1"); // use gameobject to find the player's tag 

        // if player obj found set it up to playerInfo
        if (playerObj != null)
            playerInfo = playerObj.GetComponent<CharacterInfo1>();

        return playerInfo != null; // check if playerInfo is still null
    }

    private IEnumerator WaitForPlayerReady()
    {
        yield return new WaitUntil(PlayerSetUp); // wait until the Player is set up

        PlayerTurnStart(); // calls the start function again after delay
    }
    // Add this to your UI script
    
    //public void OnEndTurnButton()
    //{
    //    TurnManager.Instance.EndPlayerTurn(); // ends player's turn
    //}

    public void StartPlayerReaction(EnemyInfo enemyAttker, int dmg, int hitChance)
    {
        inComingAttackEnemy = enemyAttker; // set up enemy attacker

        inComingDamage = dmg; // how much damage is from enemy

        inComingHitChance = hitChance; // what is the hit chance of enemy

        playerReactionSuccessful = false; // player hasn't react to attack yet

        SetTurnState(TurnState.PlayerReaction); // change state to player react 
    }

    public void PlayerDodgeReaction()
    {
        // if current state is not player Reaction get out
        if (State != TurnState.PlayerReaction) return;

        bool enemyHit = HitRollCheck.HitRollPercent(inComingHitChance); // use flag to check the hit chance roll

        // it hit returns true player take damage
        if (enemyHit)
        {
            Debug.Log("Player Failed to Dodge!"); // debug msg

            CharacterInfo1.Instance.PlayerTakeDamage(inComingDamage); // player take damage
        }
        else
        {
            Debug.Log("Player Dodged the Attack!"); // debug msg
        }

        //playerReactionSuccessful = true; // set the flag to true, player did an reaction

        EndPlayerReaction();  // after player react flag toggle
    }

    public void PlayerTankDamageReaction()
    {
        // if current state is not player Reaction get out
        if (State != TurnState.PlayerReaction) return;

        Debug.Log("Player takes Dmg by Enemy"); // debug msg

        CharacterInfo1.Instance.PlayerTakeDamage(inComingDamage); // take damage

        //playerReactionSuccessful = true; // set flage to true and player, 

        EndPlayerReaction();  // after player react flag toggle
    }
    
    public void PlayerCounterAttackReaction()
    {
        // if current state is not player Reaction get out
        if (State != TurnState.PlayerReaction) return;

        // check if the enemy attacker exist
        if (inComingAttackEnemy == null)
        {
            Debug.LogWarning("No incoming enemy to counter attack!"); // debug msg

            playerReactionSuccessful = true; // player did an reaction
            return;
        }

        PlayerCombatCheck.Instance.PlayerCounterAttack(inComingAttackEnemy); // call the counter attack function and pass over the enemy info

        EndPlayerReaction();  // after player react flag toggle

        //playerReactionSuccessful = true; // set player did an reaction
    }

    
    public IEnumerator WaitForPlayerReaction()
    {

        yield return new WaitUntil(() => playerReactionSuccessful); // wait until player reaction flag is true
     

        // reset everything
        inComingAttackEnemy = null;

        inComingDamage = 0;

        inComingHitChance = 0;

        //State = TurnState.EnemyAction; // after enemy attack player react goes back to loop, check for next enemy action
    }

    public void EndPlayerReaction()
    {
        playerReactionSuccessful = true; // set the flag to true, player reacted
    }

    // Added by Warren: Needed to add this function because the GAME OVER screen keeps reappearing because the TurnManager game over state keeps looping.
    // This fixed the problem, it forces the reset of the player's turn state.
    public void ForceResetToPlayerTurn()
    {
        State = TurnState.PlayerAction;
        Debug.Log("TurnManager: Force reset to PlayerAction state");
    }

    public void DeleteEnemy(EnemyController1 enemy)
    {
        // enemy is not found get out
        if (enemy == null) return;

        // if the enemy removal list is not empty, remove them
        if (!pendingEnemyRemovals.Contains(enemy))
            pendingEnemyRemovals.Add(enemy); // add them to removal list
    }

    public void DeleteEnmey(EnemyController1 enemy)
    {
        PlayerFuryMode.Instance.EnemyKilledUpdate(); // add to kills

        enemies.Remove(enemy); // removes this enemy

        enemies.RemoveAll(e => e == null); // deletes all the null enemies
    }

}

//private void Update()
//{
//    if (!isInitialized) return; // not initialized return

//    if (CurrentPhase == TurnPhase.Player && Input.GetKeyDown(KeyCode.Space)) // space ends the player's turn, if is player's turn
//        EndPlayerTurn();

//    //Debug.Log($"{name} turn ended.");
//}

// Add this to your UI script

//public void OnEndTurnButton()
//{
//    TurnManager.Instance.EndPlayerTurn(); // ends player's turn
//}

//private bool AllEnemiesReady()
//{
//    foreach (var enemy in enemies) // use a loop to check if enemies ar setup
//    {
//        if (enemy == null) continue; // if not found skip

//        else if (!enemy.Initialized) return false; // if not initialized set it to false
//    }

//    return true;// else true
//}


