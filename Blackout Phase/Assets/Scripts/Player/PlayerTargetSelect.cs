// used this video for reference on how to make a target system URL:https://www.youtube.com/watch?v=h9oEhVqGptU
// Weijun

using NUnit.Framework;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerTargetSelect : MonoBehaviour
{
    [Header("Player Target Settings")]
    [SerializeField] private KeyCode lockOnKey; // key to attack
    [SerializeField] private KeyCode previousKey; // option  for going back 
    [SerializeField] private KeyCode nextKey;     // option for going next
    [SerializeField] private KeyCode cancelKey;   // option to cancel the lockon key
    [SerializeField] private KeyCode confirmKey; // key for comfirming the attack

    [Header("Targeting Settings")]
    [SerializeField] private bool enemyInRange = true; // enemy has to be in range for the attack
    [SerializeField] private bool keepTargetAfterAttk = false; // keeps on the same target after attack

    private readonly List<EnemyInfo> enemyCandidates = new List<EnemyInfo>(); // enemy list for reading

    private int targetIndex = -1; // keep count on the enemy index

    private EnemyInfo currentTarget; // which enemy is selected

    private OverlayTile1 highlightTile; // once enemy is selected highlights the tile for visual

    public bool isTargeting => currentTarget != null; // flag accessor, if current target exist

    public EnemyInfo CurrentTarget => currentTarget; // accessor for the current target

    private void Update()
    {
        // if the turn manager state is not player action get out, not player's turn exist
        if (TurnManager.Instance.State != TurnState.PlayerAction) return;

        // player pressed the attack key 
        if (Input.GetKeyDown(lockOnKey))
        {
            // if the target is not found
            if (currentTarget == null)
                LockOnTarget();

            //else
            //{
            //    ConfirmAttack(); // confrims attack

            //    // defalut is f, so always clears after
            //    if (!keepTargetAfterAttk)
            //        ClearTarget();
            //}
    
            return;
        }

        // if input is cancel clears the target lockon
        if (Input.GetKeyDown(cancelKey))
        {
            ClearTarget();
            return;
        }

        // current enemy target exist and key press is confirm attack
        if (currentTarget != null && Input.GetKeyDown(confirmKey))
        {
            ConfirmAttack(); // calls the confirm function

            //  // defalut is f, so always clears after
            if (!keepTargetAfterAttk)
                ClearTarget();
        } 

        // if the enemy is found, use previous/next to change targets
        if (currentTarget != null)
        {
            // if key is previous go back 1
            if (Input.GetKeyDown(previousKey))
            {
                EnemyCycleKey(-1);

                Debug.Log("Targeted Changed (Previous) Confirmed!"); // dubg msg 
            }
               
            // if key is next move on to next 1
            else if (Input.GetKeyDown(nextKey))
            {
                EnemyCycleKey(+1);

                Debug.Log("Targeted Changed (Next) Confirmed!"); // dubg msg 
            }
           
        }
    }

    private int GetPlayerRange()
    {
        var player = CharacterInfo1.Instance; // copies player stats

        return (player != null) ? player.BaseRange : 1; // returns the player's baseRange if playerInfo is found        
    }

    private void EnemyCycleKey(int cycleDir)
    {
        RefershEnemyCandidates(); // function to check which enemies 

        // if enemies not found get out
        if (enemyCandidates.Count == 0)
        {
            Debug.Log("No enmies to cycle through!"); // debug msg

            ClearTarget(); // undo the highlight
            return;
        }

        // if current target is not found or enemyCandidates also not found relockon
        if (currentTarget == null || !enemyCandidates.Contains(currentTarget))
        {
            targetIndex = 0; // reset enemy index

            SetEnemyTarget(enemyCandidates[targetIndex]); // relockon to to the closest target
            return;
        }

        targetIndex = enemyCandidates.IndexOf(currentTarget); // find the current index in case list changed ordering

        // if index is less than 0, defalut reset to 0 filters negatives
        if (targetIndex < 0)
            targetIndex = 0; 

        targetIndex = (targetIndex + cycleDir) % enemyCandidates.Count; // update the index to current + 1 % of total, next one

        // if index is still less than 0, add total enemies count to index, filters negatives
        if (targetIndex < 0)
            targetIndex += enemyCandidates.Count;

        SetEnemyTarget(enemyCandidates[targetIndex]); // call function set enmey target
    }



    private void LockOnTarget()
    {
        RefershEnemyCandidates(); // function to check which enemies 

        // if enemies not found get out
        if (enemyCandidates.Count == 0)
        {
            Debug.Log("No enmies to target!"); // debug msg
            return;
        }

        targetIndex = 0; // reset to the frist

        SetEnemyTarget(enemyCandidates[targetIndex]); // pass over which enemy will be lockon
    }

    private void RefershEnemyCandidates()
    {
        enemyCandidates.Clear(); // reset them before refresh

        var player = CharacterInfo1.Instance; // copy player's stats

        // if the player info is not found get out
        if (player == null || player.CurrentTile == null) return;

        int pAttkRange = GetPlayerRange(); // get the player's attack range

        EnemyInfo[] enemies = FindObjectsByType<EnemyInfo>(FindObjectsSortMode.None); // find all the enemy objects, sorting is not needed

        // if enemies arries is 0, no enemies display message
        if (enemies.Length == 0)
        {
            Debug.Log("Enemies Not Found!"); // debug msg
            return;
        }

        //EnemyInfo closest = null; // value to hold the closest enemy

        //int closestDistance = int.MaxValue; // give it the highest value to hold

        // loop goes through all the enemies to find the enemy candidates
        foreach (var enemy in enemies)
        {
            // if enemy is null keep on going
            if (enemy == null) continue;

            //// if enemycurrent tile is found enemy tile is equal to the world position in MapManager current location
            //var enemyTile = enemy.currentTile != null ? enemy.currentTile : MapManager1.Instance.GetWorldTilePosition(enemy.transform.position);

            OverlayTile1 enemyTile = GetEnemyTile(enemy); // find the enemy's tile

            // keep on going if tile is null
            if (enemyTile == null) continue;

            int distance = Manhattan(player.CurrentTile.gridLocation, enemyTile.gridLocation); // use manhattan to calculate the distance between player/enemy

            // only lock on if enemy is in range
            //if (enemyInRange && distance > player.BaseRange) continue;

            // keep on going if distance is bigger than player attack range or enemy not in range                                                              if current distance less than the closest range sawap them
            if (distance > pAttkRange && enemyInRange) continue;
            //{
            //    closestDistance = distance; // swap the closest range

            //    closest = enemy; // set it to that closest enemy
            //}

            //// if the closest enemy doesn't exist display a message
            //if (closest == null)
            //{
            //    Debug.Log("Enemies are not in range!"); // debug msg
            //    return;
            //}

            //SetEnemyTarget(closest); // set up the closest enemjy target

            //Debug.Log($"Locked on to:{closest.name}"); // debug msg

            enemyCandidates.Add(enemy); // add the ones that matches our goal
        }

        // compare the enemies and find the closest one
        enemyCandidates.Sort((a, b) =>
        {
            int distToA = DistanceToPlayer(a); // find the distance of a

            int distToB = DistanceToPlayer(b); // find the distance of b

            int compare = distToA.CompareTo(distToB); // compare the distance of A/B

            // if one is bigger return that value
            if (compare != 0) return compare;

            return a.GetInstanceID().CompareTo(b.GetInstanceID()); // return the closest result
        });
    }

    private int DistanceToPlayer(EnemyInfo enemy)
    {
        var player = CharacterInfo1.Instance; // copy over the player stats

        // if playyer is not found and tile is missing return a max int 
        if (player == null || player.CurrentTile == null) return int.MaxValue;

        var enemyTile = GetEnemyTile(enemy); // find the enemy tile using function

        // if enemy tile is missing also return max int
        if (enemyTile == null) return int.MaxValue;

        return Manhattan(player.CurrentTile.gridLocation, enemyTile.gridLocation); // using the player tile and enemy tile to find the distance
    }

    private OverlayTile1 GetEnemyTile(EnemyInfo enemy)
    {
        // if enemy not found return null
        if (enemy == null) return null;

        // if enemy currentile is found use it
        if (enemy.currentTile != null) return enemy.currentTile;

        //Collider2D eCollider = enemy.GetComponentInChildren<Collider2D>(); // get enemy prefab collider2d

        //// if enemy collider is not null use it from center
        //Vector2 ePosition = eCollider != null ? (Vector2)eCollider.bounds.center : (Vector2)enemy.transform.position;

        return MapManager1.Instance.GetWorldTileFromTransform(enemy.transform); // else use the map world point to find it
    }

    private void ConfirmAttack()
    {
        // current target doesn't exist get out
        if (currentTarget == null) return;

        PlayerCombatCheck.Instance.PlayerAttackCheck(currentTarget); // attack the locked on target

        ClearAttackTargetHighlight(); // clear the target after (clear the target highlight
    }

    private void SetEnemyTarget(EnemyInfo enemy)
    {
        ClearAttackTargetHighlight(); // before lock on target make sure to clear the previous one (highlight)

        currentTarget = enemy; // set up the target to enemy

        // enemy tile set to the current enemy world position if it is found
        OverlayTile1 enemyTile = GetEnemyTile(enemy); // use the function to find tile

        // enemy tile is found highlight the tile
        if (enemyTile != null)
        {
            highlightTile = enemyTile; // highlights the enemy tile

            highlightTile.ShowEnemyTile(); // displays the tiles
        }

        Debug.Log($"Targetting:{enemy.name}"); // debug msg
    }

    private void ClearAttackTargetHighlight()
    {
        // if the highlight tile is exist hide it
        if (highlightTile != null)
            highlightTile.HideTile();

        highlightTile = null; // clear the highlight tile

        //currentTarget = null; // clear the current enemy target 
    }

    private void ClearTarget()
    {
        ClearAttackTargetHighlight(); // first clear the highlight

        currentTarget = null; // reset target

        targetIndex = -1; // reset index

        enemyCandidates.Clear(); // reset enemies
    }

    private int Manhattan(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); // returns the player/enemy distance
    }
}
