// and some additional fixing from online sources Unity Discussion:https://discussions.unity.com/, reddit, YouTube
// I should have keep tract on the exact page but I forgot to save some of the links 
// This is also a finite state machine took inspiration from CS 456
// this time is constantly updating because turn based games needs a turn manager at all time
// since we have more states this time a switch statement is used to help navigate through out
// the different states, player start -> player action -> player end -> calls the enemy controll
// then the enemy use its own finite state machine to control the action -> enemy ends calls player again
// it just keeps on repeating again until the player dies then it goes into game over state
// Weijun

using System.Collections; // for the array list we have also IEnumerator for delay funciton calls yield returns. loading map first then do something else
using System.Collections.Generic;  // for the List<T> and dictionary <T, T> for pathfinding
using System.Net.Mail;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine; // default

// Added by Warren
using UnityEngine.SceneManagement;

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

    public EnemyInfo inComingAttackEnemy { get; private set; } // let other scripts to access it attacker

    public UnitCore inComingTargetUnit { get; private set; } // let other scripts to access it player target

    // player's reaction to enemy attacks
    public int inComingDamage { get; private set; } // let other scripts to access it, the in coming damage 

    public int inComingHitChance { get; private set; } // let other scripts to access the in coming Hit Chance

    // enemy skill data
    public SkillData incomingEnemySkill { get; private set; } // let other script to access the enemy skill

    public bool WaitForPlayerReact => State == TurnState.PlayerReaction; // set a flag for other script to access if player is in Reaction state

    //public bool IsPlayerTurn => CurrentPhase == TurnPhase.Player; // flag to check if player's turn?

    private bool playerReactionSuccessful = false; // keep track if player's reaction worked

    private bool incomingAttackerResolved = false; // check if the enemy already attacked

    private bool isInitialized = false; // check too see if enemies are initialized

    private CharacterInfo1 playerInfo; // player's info

    private PlayerHighlighter playerHighlighter; // for displaying the highlights

    //private SkillExecutor playerSkillExecutor; // accessor to the skill effect

    //private SkillAttachment skillAttachment; // accessor for the skill cooldown

    private void Awake()
    {
        if (Instance != null && Instance != this)  // if gameobject not found destory it, else set it to this
        {
            Destroy(gameObject);

            return;
        }

        Instance = this; // found set it up

        //isInitialized = true; // toggle flag everything is set up

        // Modified by Warren, needed if scene is not changed to main menu.
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name != "TitleScreen")
        {
            DontDestroyOnLoad(gameObject); // keeps the game object
        }

        Debug.Log("TurnManager Awake"); // test

        PlayerSetUp(); // bool function for player set up
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => MapManager1.Instance != null && 
                                         MapManager1.Instance.map != null &&
                                         MapManager1.Instance.map.Count > 0); // wait until the map is set

        Debug.Log("Turnmanager: Map is ready"); // debug msg

        // if the playerHighlight is not find search for it
        if (playerHighlighter == null)
            playerHighlighter = MapManager1.Instance.GetComponent<PlayerHighlighter>();

        // back up if it can find it by type
        if (playerHighlighter == null)
            playerHighlighter = FindFirstObjectByType<PlayerHighlighter>();

        //yield return new WaitUntil(() => AllEnemiesReady()); // wait until enemies are set up

        //CurrentPhase = TurnPhase.Player; // Palyer can not start turn

        //EnemyController1[] found = FindObjectsByType<EnemyController1>(FindObjectsSortMode.InstanceID); // got through the list and find enemies

        //enemies.AddRange(found); // add the nemeies

        //SetupPlayerSkillExecutor(); // set up the playerSkillExecutor

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
        // clear the player's highlights
        if (State == TurnState.PlayerAction && newState != TurnState.PlayerAction)
        {
            playerHighlighter?.ClearHighlights(); // clear all the highlights in the same playerAction turn
        }

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
                ShowPlayerPreviews(); // show highlights
                break;

            case TurnState.PlayerReaction: // player reaction to enemy attacks, counter/dodge/tank
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

                // Added by Warren, need it to make Game Over screen function properly when the player restarts the level.

                // Save the current level name before loading Game Over scene
                PlayerPrefs.SetString("LastLevel", SceneManager.GetActiveScene().name);
                PlayerPrefs.Save();
                
                // Load the Game Over scene
                SceneManager.LoadScene("GameOver");
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
        // player instance is found starts ticking down the buff timer
        if (CharacterInfo1.Instance != null)
            CharacterInfo1.Instance.TickDownBuffEffects(BuffEffectTimer.StartOnUserTurn);

        // check if the playerInfo finished loading
        if (!PlayerSetUp())
        {
            Debug.LogWarning("TurnManager => Player not ready yet! delay"); // debug msg

            StartCoroutine(WaitForPlayerReady()); // calls the delay function
            return;
        }

        SetupSkillAttachmentCDTick(playerInfo); // set up the skillAttachment for the combat unit

        //PlayerFuryMode.Instance.ResetCurrentKills(); // reset the kill counter before each turn

        playerInfo.ResetAP(); // resets the AP at the beginning of the turn

        Debug.Log("Player AP reset to: " + playerInfo.currentAP); // shows current AP at the begginer of the turn

        Debug.Log("Player Current EN: " + playerInfo.CurrentEN); // shows current EN for debug

        // current state is not playerAcution? set it to playerAction
        if (State != TurnState.PlayerAction)
            SetTurnState(TurnState.PlayerAction); // state now player action
    }

    public void ShowPlayerPreviews()
    {
        // if current state is not playerAction get out
        if (State != TurnState.PlayerAction) return;

        // if the highlight is missing get out
        if (playerHighlighter == null) return;

        // if player tile or player is not found get out
        if (playerInfo == null || playerInfo.CurrentTile == null) return;

        playerHighlighter.ShowPlayerMovementTiles(playerInfo.CurrentTile, playerInfo.GetMoveRange()); // highlight the range around the player, passing current tile and the getRange to find the correct range
    }
    private void FlashIncomingEnemyTile()
    {
        // is the enemy null if so get out
        if (inComingAttackEnemy == null) return;

        OverlayTile1 enemyTile = inComingAttackEnemy.CurrentTile; // save the enemy tile

        // check to see if enemy tile is null
        if (enemyTile == null)
            enemyTile = MapManager1.Instance.GetWorldTileFromTransform(inComingAttackEnemy.transform); // if not found use the map world position to locate the transform

        // check to see if the tile is still null
        if (enemyTile == null) return;

        playerHighlighter?.SingleTileHighlight(enemyTile, enemy: true); // highlight the enemy 
    }

    private void FlashPlayerTile()
    {
        // check to make suere player tile exist
        if (playerInfo == null || playerInfo.CurrentTile == null) return;

        playerHighlighter?.SingleTileHighlight(playerInfo.CurrentTile, enemy: false); // calls the single tile
    }

    public void ClearHighlights()
    {
        playerHighlighter?.ClearHighlights(); // clears the highlights
    }

    public void EndPlayerTurn()
    {
        if (State != TurnState.PlayerAction) return; // not player's turn get out

        // player instance is found end ticking down the buff timer
        if (CharacterInfo1.Instance != null)
            CharacterInfo1.Instance.TickDownBuffEffects(BuffEffectTimer.EndOfUserTurn);

        Debug.Log("[TM] After turn end attack: " + playerInfo.BaseAttack); // debug msg

        Debug.Log("[TM] Player turn Ended -> Enemy Phase Starting");

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
        // loop through all the enemy list
        foreach (EnemyController1 enemy in enemies)
        {
            // skip null enemy
            if (enemy == null) continue;

            EnemyInfo enemyInfo = enemy.GetComponent<EnemyInfo>(); // access the enemy

            // if the enemy is not found or dead skip
            if (enemyInfo == null || enemyInfo.IsDead()) continue;

            enemyInfo.TickDownBuffEffects(BuffEffectTimer.StartOnUserTurn); // starts the tick 
        }

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

            EnemyInfo enemyInfo = enemy.GetComponent<EnemyInfo>(); // get the info for enemy

            // check to see if enemy exist
            if (enemyInfo == null || enemyInfo.CurrentHP <= 0) continue;

            SetupSkillAttachmentCDTick(enemyInfo);
            
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

        // loop through the list of enemies 
        foreach (EnemyController1 enemy in enemies)
        {
            // skip null enemy
            if (enemy == null) continue;

            EnemyInfo enemyInfo = enemy.GetComponent<EnemyInfo>(); // access the enemy

            // if the enemy is not found or dead skip
            if (enemyInfo == null || enemyInfo.IsDead()) continue;

            enemyInfo.TickDownBuffEffects(BuffEffectTimer.EndOfUserTurn); // end the tick at the same time
        }


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

            // Added by Warren, displays victory screen.
            VictoryManager.Instance.ShowVictory();

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

    private void SetupSkillAttachmentCDTick(UnitCore combatUnit)
    {
        // check to see if the unit is null 
        if (combatUnit == null) return;
            //playerInfo = CharacterInfo1.Instance; // set up playerInfo
            
        Debug.Log($"[TM] UnitInfo found:{combatUnit != null}"); // debug msg

        SkillAttachment skillAttachment = combatUnit.GetComponent<SkillAttachment>(); // set the skillAttachment to find the unit.attachment 
        
        // check if the skill atachment is found
        if (skillAttachment == null)
        {
            Debug.Log($"[TM] SkillAttachment not found:{skillAttachment != null}"); // debug msg
            return;
        }

        Debug.Log($"[TM] SkillAttachment found:{skillAttachment != null}"); // debug msg

        skillAttachment.CooldownCountDown(); // start counting the skill.cd
    }

    // decidee if the incomingAttack is normal/skill
    private void DecideIncomingAttackType(float damgeMultiplier = 1f)
    {
        // if the enemy already attack get out
        if (incomingAttackerResolved) return;

        incomingAttackerResolved = true; // set the flag to true enemy attacked

        // check to see if attacker exist
        if (inComingAttackEnemy == null) return;

        // check if the target exst
        if (inComingTargetUnit == null) return;

        EnemyAttackCore enemyAttackCore = inComingAttackEnemy.GetComponent<EnemyAttackCore>(); // setup the attacCore

        // if the attackCore is still null 
        if (enemyAttackCore == null)
            enemyAttackCore = inComingAttackEnemy.GetComponentInChildren<EnemyAttackCore>(); // find it in the chidren

        // still null display a message
        if (enemyAttackCore == null)
        {
            Debug.Log("[TM] EnemyAttackCore not found!"); // debug msg
            return;
        }

        int orginalDmg = inComingDamage; // the original value

        int finalDamage = Mathf.RoundToInt(inComingDamage * damgeMultiplier); // recalculate the damage with the multiplier 

        Debug.Log($"[TM] Player takes Dmg:{finalDamage} by Enemy"); // debug msg

        // if the skill is not null 
        if (incomingEnemySkill != null)
            enemyAttackCore.AttackTarget(inComingTargetUnit, finalDamage, incomingEnemySkill); // skill attack

        else
            enemyAttackCore.AttackTarget(inComingTargetUnit, finalDamage); // normal attack

        inComingDamage = orginalDmg; // set it back to orignal value
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

    public void StartPlayerReaction(EnemyInfo enemyAttker, UnitCore target, int dmg, int hitChance, SkillData enemySkill = null)
    {
        // if it's reaction state get out
        if (State == TurnState.PlayerReaction)
        {
            Debug.Log("Already in reaction state!"); // debug msg
            return;
        }

        // check if enemy is dead or missing before reacting
        if (enemyAttker == null || enemyAttker.CurrentHP <= 0)
        {
            Debug.Log("Enemy is dead or missing!"); // debug msg

            ResetIncomingPlayerReaction();
            return;
        }

        // check to see if target is null
        if (target == null)
        {
            Debug.Log("Reaction target missing!"); // debug msg

            ResetIncomingPlayerReaction(); // reset everthing
            return;
        }

        inComingAttackEnemy = enemyAttker; // set up enemy attacker

        inComingTargetUnit = target; // setup the attack target

        incomingEnemySkill = enemySkill; // the enemy skill

        incomingAttackerResolved = false; // set it to false/ enemy hasn't attacked

        inComingDamage = dmg; // how much damage is from enemy

        inComingHitChance = hitChance; // what is the hit chance of enemy

        playerReactionSuccessful = false; // player hasn't react to attack yet

        playerHighlighter?.ClearHighlights(); // clears the highlights

        FlashPlayerTile(); // flashings player tile while being attacked

        FlashIncomingEnemyTile(); // highlight enemy attack tile

        SetTurnState(TurnState.PlayerReaction); // change state to player react 
    }

    public void PlayerDodgeReaction()
    {
        // if current state is not player Reaction get out
        if (State != TurnState.PlayerReaction) return;

        int playerDodgeBonus = 10; // extra 10 dodge chance

        int HitChanceAdjustment = Mathf.Clamp(inComingHitChance - playerDodgeBonus + playerInfo.EvasionRate, 5, 95); // make sure after the bonus chance it is still within 5 - 95

        bool enemyHit = HitRollCheck.HitRollPercent(HitChanceAdjustment); // use flag to check the hit chance roll

        // it hit returns true player take damage
        if (enemyHit)
        {
            Debug.Log("Player Failed to Dodge!"); // debug msg

            //CharacterInfo1.Instance.PlayerTakeDamage(inComingDamage); // player take damage

            //inComingTargetUnit.TakeDamage(inComingDamage); // take damage

            DecideIncomingAttackType(); // normal/skill attack from enemy
        }
        else
        {
            Debug.Log("Player Dodged the Attack!"); // debug msg

            // Added by Warren, displays on the screen that the player dodged the attack
            if (DamageObserver.Instance != null)
            {
                DamageObserver.Instance.ShowDodgedText(CharacterInfo1.Instance.transform.position);
            }
        }

        //playerReactionSuccessful = true; // set the flag to true, player did an reaction

        EndPlayerReaction();  // after player react flag toggle
    }

    public void PlayerTankDamageReaction()
    {
        // if current state is not player Reaction get out
        if (State != TurnState.PlayerReaction) return;

        // check to see if the damage is vaild
        if (inComingDamage <= 0)
        {
            Debug.LogWarning("[TM] No vaild incoming damage from enemy, skipping Tank damage"); // debug msg

            EndPlayerReaction(); // force to end reaction
            return;
        }

        //Debug.Log($"[TM] Player takes Dmg by Enemy"); // debug msg

        //CharacterInfo1.Instance.PlayerTakeDamage(inComingDamage); // take damage

        //inComingTargetUnit.TakeDamage(inComingDamage); // take damage

        DecideIncomingAttackType(0.8f); // normal/skill attack from enemy, 20% dmg reduction

        //playerReactionSuccessful = true; // set flage to true and player, 

        EndPlayerReaction();  // after player react flag toggle
    }
    
    public void PlayerCounterAttackReaction()
    {
        // if current state is not player Reaction get out
        if (State != TurnState.PlayerReaction) return;

        // check if the enemy attacker exist
        if (inComingAttackEnemy == null || inComingAttackEnemy.CurrentHP <= 0)
        {
            Debug.Log("Enemy is dead or missing!"); // debug msg

            EndPlayerReaction();

            LevelCleared(); // check for level clear condition

            // if the state is not clear change it to enemy action
            if (State != TurnState.ClearLevel)
                State = TurnState.EnemyAction; 

            return;
        }

        // if the enemy is found and alive decide which attack to use
        if (inComingAttackEnemy != null && inComingAttackEnemy.CurrentHP > 0)
            DecideIncomingAttackType(); // which attack to use normal/skill for enemy

        //// check if the Target is player
        if (inComingTargetUnit is CharacterInfo1 playerTarget && playerTarget.CurrentHP > 0 && inComingAttackEnemy.CurrentHP > 0)
        {
            //playerTarget.PlayerTakeDamage(inComingDamage); // player takes dmg

            PlayerCombatCheck.Instance.PlayerCounterAttack(inComingAttackEnemy, false); // call the counter attack function and pass over the enemy info

            //Debug.Log($"[TM] Counter:{playerTarget.name} took:{inComingDamage}"); // debug msg
        }

        bool counterCheck = PlayerCombatCheck.Instance.PlayerCounterAttack(inComingAttackEnemy, false); // flag for checking if the player counter is successful

        Debug.Log("[TM] counterCheck: " + counterCheck); // debug msg

        // check if the counter is successful
        if (!counterCheck)
        {
            Debug.Log("{TM] Player Counter failed, reselct skill"); // debug msg
            return; 
        }

        EndPlayerReaction();  // after player react flag toggle

        //playerReactionSuccessful = true; // set player did an reaction
    }

    public void ResetIncomingPlayerReaction()
    {
        playerReactionSuccessful = false; // toggle the raction flag

        // reset everything
        inComingAttackEnemy = null;

        inComingTargetUnit = null;

        incomingEnemySkill = null;

        incomingAttackerResolved = false;

        inComingDamage = 0;

        inComingHitChance = 0;
    }

    public IEnumerator WaitForPlayerReaction()
    {

        yield return new WaitUntil(() => playerReactionSuccessful); // wait until player reaction flag is true

        // after playerReacted enemy Skill CD start
        EnemyInfo enemy = inComingAttackEnemy; // setup the incomingAttack to enemy

        // enemy is found continue
        if (enemy != null && enemy.CurrentHP > 0)
        {
            SkillAttachment enemySkillAttachment = enemy.GetComponent<SkillAttachment>(); // find the skillAttachment on enemy

            // enemySkillAttachment is found start the set skill cooldown
            if (enemySkillAttachment != null && incomingEnemySkill != null)
            {
                enemySkillAttachment.SetSkillCooldown(incomingEnemySkill);

                Debug.Log($"[TM] Set Enemy skill CD:{incomingEnemySkill.skillCoolDown}"); // debug msg
            }
        }

        // reset everything
        inComingAttackEnemy = null;

        inComingTargetUnit = null;

        incomingEnemySkill = null;

        incomingAttackerResolved = false; 

        inComingDamage = 0;

        inComingHitChance = 0;

        LevelCleared(); // check for level condition clear before continue

        // check if the state is cleared
        if (State == TurnState.ClearLevel)
        {
            Debug.Log("[TM] Level cleared on player reaction kill"); // debug msg
            yield break;
        }

        // check to see if player is in reaction state, change to enemyAction so enemy can attack
        if (State == TurnState.PlayerReaction)
            State = TurnState.EnemyAction;

        //State = TurnState.EnemyAction; // after enemy attack player react goes back to loop, check for next enemy action
    }

    public void EndPlayerReaction()
    {
        // if player reacted returns
        if (playerReactionSuccessful) return;

        playerReactionSuccessful = true; // set the flag to true, player reacted

        playerHighlighter?.ClearHighlights(); // clear the highlights once finished reatiing

        Debug.Log("[TM] EndPlayerReaction callded!"); // debug msg
    }

    // Added by Warren: Needed to add this function because the GAME OVER screen keeps reappearing because the TurnManager game over state keeps looping.
    // This fixed the problem, it forces the reset of the player's turn state.
    public void ForceResetToPlayerTurn()
    {
        State = TurnState.PlayerAction;
        
        // Reset all enemy patrol points to default values
        ResetAllEnemyPatrolPoints();
        
        Debug.Log("TurnManager: Force reset to PlayerAction state with fresh patrol points");
    }

    // Fix: Reset all enemy patrol points by finding them in the scene
    private void ResetAllEnemyPatrolPoints()
    {
        // Find all enemies in the current scene
        EnemyController1[] allEnemies = FindObjectsByType<EnemyController1>(FindObjectsSortMode.InstanceID);
        
        if (allEnemies.Length == 0)
        {
            Debug.Log("No enemies found to reset patrol points");
            return;
        }
        
        // Find all patrol point GameObjects in the scene
        GameObject[] patrolPointObjects = GameObject.FindGameObjectsWithTag("PatrolPoint");
        
        if (patrolPointObjects.Length == 0)
        {
            Debug.LogWarning("No patrol points found in scene with tag 'PatrolPoint'");
            return;
        }
        
        // Convert patrol point positions to grid coordinates
        List<Vector2Int> allPatrolPositions = new List<Vector2Int>();
        foreach (GameObject point in patrolPointObjects)
        {
            // Convert world position to grid position
            Vector3 worldPos = point.transform.position;
            Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y));
            allPatrolPositions.Add(gridPos);
        }
        
        // Assign patrol points to each enemy
        foreach (EnemyController1 enemy in allEnemies)
        {
            if (enemy != null)
            {
                // Use the existing SetPatrolPoints method in EnemyController1
                enemy.SetPatrolPoints(allPatrolPositions, 0);
                Debug.Log($"Reset patrol points for {enemy.name}: found {allPatrolPositions.Count} points");
            }
        }
    }

    public void DeleteEnmey(EnemyController1 enemy)
    {
        Debug.Log("[TM] Delete enemy being called...."); // debug msg

        // check make sure enemy is found
        if (enemy == null) return;

        //// make sure enemy is found
        //if (enemy != null)

        EnemyInfo _enemyInfo = enemy.GetComponentInChildren<EnemyInfo>(); // get the enemy info using enemy, even the children / copies

        // make sure the enemyInfo and score manager is found
        if (_enemyInfo != null && ScoreManager.Instance != null)
        {
            bool inFuryMode = PlayerFuryMode.Instance != null && PlayerFuryMode.Instance.inFuryMode; // if the furymode is found and the player is in fury mode = true

            ScoreManager.Instance.AddEnemyKillScore(_enemyInfo.EnemyRank, inFuryMode); // add score depending on the rank
        }
        else
        {
            Debug.LogWarning("[TM] Score was not added: enemyInfo/ScoreManager missing!"); // debug msg
        }

        // make sure it's not empty
        if (PlayerFuryMode.Instance != null)
            PlayerFuryMode.Instance.EnemyKilledUpdate(); // add to kills

        enemies.Remove(enemy); // removes this enemy

        enemies.RemoveAll(e => e == null); // deletes all the null enemies

        LevelCleared(); // check if the level is complelted
    }

    public static void SetInstaceForEnemyTest(TurnManager inst)
    {
        Instance = inst; // set up the same instance for test
    }

    // Added by Warren, cleans everything up when the player wants to return to the main menu.
    private void OnDestroy()
    {
        // Clean up the instance when this object is destroyed
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Added by Warren: //
    //=================//
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // If we loaded the title screen, destroy this manager
        if (scene.name == "TitleScreen")
        {
            if (Instance == this)
            {
                Instance = null;
            }
            Destroy(gameObject);
        }
    }
    // =============== //
}

// old version of playerStart()
//// if the playerInfo not found set it up
//if (playerInfo == null)
//    PlayerSetUp();
//    //playerInfo = GetComponent<CharacterInfo1>();

//// if the skill attachment is empty set it up
//if (skillAttachment == null)
//SetupSkillAttachment(playerInfo); // set up the playerSkillExecutor

//// if skill effect accessor set up call it 
//if (skillAttachment != null)
//{
//    Debug.Log($"[TM] Tracking cooldown"); // debug msg

//    //skillAttachment.CooldownCountDown(); // skill coold down goes down
//}
//else
//    Debug.Log($"[TM] SkillAttachmebt not found!"); // debug msg

//private void SetupPlayerSkillExecutor()
//{
//    // check to see if playerInfo is null if so set up
//    if (playerInfo == null)
//        playerInfo = CharacterInfo1.Instance; // set up playerInfo 

//    Debug.Log($"[TM] playerInfo found:{playerInfo != null}"); // debug msg

//    // if the playerInfo is found
//    if (playerInfo != null)
//    {
//        playerSkillExecutor = playerInfo.GetComponent<SkillExecutor>(); // set the playerSkillExcutor to the skill executor
//    }

//    Debug.Log($"[TM] playerSkillExecutor found:{playerSkillExecutor != null}"); // debug msg
//}

//public void DeleteEnemy(EnemyController1 enemy)
//{
//    // enemy is not found get out
//    if (enemy == null) return;

//    // if the enemy removal list is not empty, remove them
//    if (!pendingEnemyRemovals.Contains(enemy))
//        pendingEnemyRemovals.Add(enemy); // add them to removal list
//}


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


