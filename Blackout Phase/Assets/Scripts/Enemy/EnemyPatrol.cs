//using UnityEngine;
//using System.Collections.Generic;
//using System.Collections;

// BROKEN CODE DO NOT USE

//public class EnemyPatrol : MonoBehaviour
//{
//    [Header("Enemy Patrol Points")]
//    [SerializeField] private List<Vector2Int> patrolPoints = new List<Vector2Int>(); // the points where you want the enemy to patrol

//    private int Index = 0; // current position index

//    private bool movingForward = true; // A -> B -> C -> B -> A visual for patroling , a flag to check which direction Enemy is going

//    private EnemyInfo enemyInfo; // access enemyInfo

//    private PathFinder pathFinder; // access pathFinder

//    //private List<OverlayTile> path; // List of OverlayTiles name path

//    private IEnumerator Start()
//    {
//        //Debug.Log("EnemyPatrol Start() running!"); // debug

//        yield return new WaitUntil(() => MapManager.Instance != null && MapManager.Instance.map != null &&
//            MapManager.Instance.map.Count > 0); // wait until the map is set

//        //Debug.Log("EnemyPatrol Start() finshied waiting for map"); // debug

//        enemyInfo = GetComponent<EnemyInfo>(); // setup the stats

//        //Debug.Log($"{name} EnemyInfo.currentTile: {enemyInfo.currentTile}"); // debug

//        pathFinder = new PathFinder(); // setup a pathfind for enemy

//        //path = new List<OverlayTile>(); // setup a new overlay List

//        //Debug.Log($"{name} patrol points: {patrolPoints.Count}"); // how many patrol points are vaild

//        foreach (var point in patrolPoints) // go through each loop and find the patrol points
//        {
//            OverlayTile t = MapManager.Instance.GetTile(point);

//            Debug.Log($"Patrol point check: {point} -> tile exists: {t != null}"); // shows each point, does it exist?

//            if (t != null)
//                Debug.Log($"Tile {point} isBlocked = {t.isBlocked}"); // debug
//        }

//        Debug.Log("EnemyPatrol Started!"); // debug msg

//        // original enemypatrolPoints
//        for (int i = 0; i < patrolPoints.Count; i++)
//            Debug.Log($"Original patrolPoints[{i}] = {patrolPoints[i]}"); // debug msg

//        // since the patrol points are not the same as the tiles change it automaticlly
//        for (int i = 0; i < patrolPoints.Count; i++)
//        {
//            OverlayTile t = MapManager.Instance.GetTile(patrolPoints[i]); //set up the 

//            // if the tile exist do the loop
//            if (t != null)
//            {
//                Debug.Log($"Correcting patrol points {patrolPoints[i]} -> Real {t.gridLocation}"); // the real patrol points

//                patrolPoints[i] = new Vector2Int(t.gridLocation.x, t.gridLocation.y); // save it to the patrolPoints[]
//            }
//            else
//            {
//                Debug.LogError($"Patrol point {patrolPoints[i]} Not found in map!"); // debug msg
//            }

//        }
//    }

//    public IEnumerator EnemyPatrolStep()
//    {
//        //Debug.Log($"{name} starting patrol step"); // debug

//        if (enemyInfo.currentTile == null) // check if the current tile exsit
//        {
//            Debug.LogError($"{name} has no currentTile!");

//            yield break;
//        }

//        else if (patrolPoints.Count == 0) // check if patrol points exist 
//        {
//            Debug.LogWarning($"{name} has no patrol points!"); // debugger for testing if patrol points are empty

//            yield break;
//        }

//        Vector2Int nextPoint = patrolPoints[Index]; // next patrol point base on currentIndex, [1,2] , index0, [1,3], index1 ....

//        OverlayTile targetTile = MapManager.Instance.GetTile(nextPoint); // displays the overlay tile

//        //Debug.Log($"{name} moving toward {nextPoint}"); // debug if tile is possible to move to

//        if (targetTile == null)
//        {
//            Debug.LogError($"{name} patrol tile {nextPoint} not found"); // check if the tile exsit

//            yield break;
//        }

//        List<OverlayTile> path = pathFinder.FindPath(enemyInfo.currentTile, targetTile); // set path to currentTile to next tile

//        if (path.Count == 0 || path == null)
//        {
//            Debug.Log($"{name} Cannot reach tile {nextPoint}"); // debug if tile is possible to move to

//            //UpdatePatrolDirection(patrolPoints); // update direction even if can't move

//            yield break;
//        }

//        OverlayTile stepTile = path[0]; // set up the tiles 0, 1, 2, and only move to next tile per turn

//        //Debug.Log($"{name} moving to step tile at {stepTile.gridLocation}"); // where to where check

//        MoveToTile(stepTile); // to the next tile

//        //if (enemyInfo.currentTile == targetTile)
//        UpdatePatrolDirection(); // which one to go, only calls when enemy arrive at the patrol point

//        yield return new WaitForSeconds(0.3f); // wait for x.x seconds before return

//        //if (stepTile == targetTile) // check to see if enemy reached the patrol point
//        //{

//        //}
//    }

//    private void MoveToTile(OverlayTile targetTile)
//    {
//        if (enemyInfo.currentTile != null) // if current tile exist 
//            enemyInfo.currentTile.hasEnemy = false; // clears the old tile date toggle flag to flase


//        //transform.position = new Vector3(targetTile.transform.position.x, // updates the enemy position
//        //    targetTile.transform.position.y + 0.0001f, // a little off set so we can see the prefab
//        //    targetTile.transform.position.z);

//        transform.position = new Vector3(targetTile.transform.position.x,
//            targetTile.transform.position.y + 0.0001f,
//            transform.position.z); // x keep y goes offset a little, don't change z

//        targetTile.hasEnemy = true; // after the update reset the flag

//        enemyInfo.EnemySetTile(targetTile); // set enemy to new tile

//        //enemyInfo.currentTile.gridLocation = new Vector3Int(targetTile.gridLocation.x, // keeps x,y but not z
//        //    targetTile.gridLocation.y, 0);

//        Debug.Log($"{name} moved to {targetTile.gridLocation}"); // debug

//        //GetComponent<SpriteRenderer>().sortingOrder = targetTile.GetComponent<SpriteRenderer>().sortingOrder; // resort the layers so it can display properly

//        //SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>(); // set up the sprteRender

//        //if (spriteRenderer != null) // if spriterender is found
//        //{
//        //    spriteRenderer.sortingOrder = targetTile.GetComponent<SpriteRenderer>().sortingOrder; // sort them in order
//        //}

//        //Debug.Log($"{name} move to tile at {targetTile.gridLocation}"); // where to where
//    }

//    private void UpdatePatrolDirection()
//    {
//        if (movingForward) // flag to check which way enemy is going A->B->C or C->B->A
//        {
//            Index++; // move to next indext patrol position

//            if (Index >= patrolPoints.Count) // check to see if enemy is doing this C->B->A reversing patrol
//            {
//                Index = patrolPoints.Count - 2; // if index goes over 3 or equal, and we counted total of 3 counts, 3 - 2, we are back to p[1] , C->B


//                movingForward = false; // toggles flag

//                //Index--; // go backwards
//                //if (patrolPoints.Count > 1) // only do it when it's bigger than 1 wrong never goes back to index[0]
//                //{
//                //    reversing = true; // toggles flag

//                //    Index = patrolPoints.Count - 2; // if index goes over 3, and we counted total of 3 counts, 3 - 2, we are back to p[1] , C->B
//                //}
//            }
//        }
//        else
//        {
//            Index--; // if reversing goes back 1

//            if (Index < 0) // if currentIndex is < 0 means we reach A, reset reversing to F, and reset index A->B
//            {
//                Index = 1; // goes back from start;

//                movingForward = true;

//                //Index++; // move forward
//            }
//        }

//        // Debug.Log($"{name} next patrol index: {Index}, reversing: {reversing}"); // check next patrol index and reversing?
//    }

//public Vector2Int GetNextPatrolPoint(List<Vector2Int> patrolPoints)
//{
//    UpdatePatrolIndex(); // update the patrolPoint at all time
//    return patrolPoints[patrolIndex];
//}

//private void UpdatePatrolIndex()
//{
//    if (movingForward) // flag to check which way enemy is going A->B->C or C->B->A
//    {
//        patrolIndex++; // move to next indext patrol position

//        if (patrolIndex >= patrolPoints.Count) // check to see if enemy is doing this C->B->A reversing patrol
//        {
//            patrolIndex = patrolPoints.Count - 2; // if index goes over 3 or equal, and we counted total of 3 counts, 3 - 2, we are back to p[1] , C->B


//            movingForward = false; // toggles flag

//            //Index--; // go backwards
//            //if (patrolPoints.Count > 1) // only do it when it's bigger than 1 wrong never goes back to index[0]
//            //{
//            //    reversing = true; // toggles flag

//            //    Index = patrolPoints.Count - 2; // if index goes over 3, and we counted total of 3 counts, 3 - 2, we are back to p[1] , C->B
//            //}
//        }
//    }
//    else
//    {
//        patrolIndex--; // if reversing goes back 1

//        if (patrolIndex < 0) // if currentIndex is < 0 means we reach A, reset reversing to F, and reset index A->B
//        {
//            patrolIndex = 1; // goes back from start;

//            movingForward = true;

//            //Index++; // move forward
//        }
//    }
//}

////    public Vector2Int GetNextPatrolPoint(List<Vector2Int> patrolPoints)
////    {
////        UpdatePatrolDirection(); // update the patrolPoint at all time
////        return patrolPoints[Index];
////    }
////}
