// These are based on this channel on YouTube: https://www.youtube.com/@lawlessgames3844
// and some additional fixing from online sources Unity Discussion:https://discussions.unity.com/, reddit, YouTube
// I should have keep tract on the exact page but I forgot to save some of the links 
// This is just for enemy movement it's similar to the player movement's idea 
// Weijun

using UnityEngine;  // default
using System.Collections.Generic; // for the List<T> and dictionary <T, T> for pathfinding// for the List<T> and dictionary <T, T> for pathfinding
using System.Collections; // for the array list we have also IEnumerator for delay funciton calls yield returns. loading map first then do something else

public class EnemyMovement : MonoBehaviour
{
    [Header("How fast enemy moves")]
    [SerializeField] private float moveSpeed = 3f; // enemy move speed

    [Header("Centering the sprite")]
    [SerializeField] private Vector2 spriteOffset = new Vector2(0f, -1f); // offset the sprite

    private EnemyInfo enemyInfo; // access enemyInfo

    private SpriteRenderer spriteRenderer; // for enemy sprite

    private void Awake()
    {
        enemyInfo = GetComponent<EnemyInfo>(); // set up the enemyinfo

        spriteRenderer = GetComponentInChildren<SpriteRenderer>(); // set up the enemy sprite renderer

        ApplySpriteOffset(); // offset the sprite
    }

    private void ApplySpriteOffset()
    {
        // sprite not found get out
        if (spriteRenderer == null) return;

        Transform spriteTransform = spriteRenderer.transform; // move the sprite offset

        spriteTransform.localPosition = new Vector3(spriteOffset.x, spriteOffset.y, spriteTransform.localPosition.z);  //spriteOffset; // SR to the offset localtion

        Debug.Log($"[EM] Root Position: {transform.position}"); // debug msg

        Debug.Log($"[EM] Applying offset to: {spriteRenderer.name} | LocalPosition After: {spriteTransform.localPosition}"); // debug msg
    }

    public IEnumerator MoveAlong(List<OverlayTile1> path)
    {
        foreach (OverlayTile1 tile in path) // loop through all the moveable tiles in path
        {
            tile.ShowEnemyTile(); // display enemy tiles

            yield return MoveStep(tile); // delay return

            tile.HideTile(); // undo hightlight

            Debug.Log("Enemy moved visually to: " + tile.gridLocation);
            Debug.Log("EnemyInfo.currentTile says: " + enemyInfo.CurrentTile.gridLocation);
        }

        //if (path.Count == 0) 
        //    yield break;

        //yield return MoveStep(path[0]);
    }

    private IEnumerator MoveStep(OverlayTile1 tile)
    {
        // clear old tile
        //if (enemyInfo.currentTile != null)
        //    enemyInfo.currentTile.hasEnemy = false; // the tile has no enemy flag

        Vector3 targetPostion = GetUnitPositionOnTile(tile); // use this tile for both player/enemy //tile.transform.position + new Vector3(0, 0.01f, 0); // a little offset on y

        //var spriteRender = GetComponent<SpriteRenderer>(); // get enemey sprite

        while (Vector2.Distance(transform.position, targetPostion) > 0.01f) // if the distance is > 0.01f move towards the position
        {
            //transform.position = Vector2.MoveTowards(transform.position, targetPostion, moveSpeed * Time.deltaTime); // how fast it moves

            //var current = transform.position; // current sprite position

            //var target = new Vector3(targetPostion.x, targetPostion.y, current.z); // change the target position x,y but keep the z

            transform.position = Vector3.MoveTowards(transform.position, targetPostion, moveSpeed * Time.deltaTime); //current, target, (moveSpeed * Time.deltaTime)); // moving from current to target tile, how fast it moves

            //// make sure the sprite works even moving, high layer 
            if (spriteRenderer != null)
                spriteRenderer.sortingOrder = 999;

            yield return null;
        }

        enemyInfo.EnemySetTile(tile); // set enemy tile

        Debug.Log($"[EM] Enemy Root Position: {transform.position}"); // debug msg

        Debug.Log($"[EM] Tile Position: {tile.transform.position}"); // debug msg

        Debug.Log($"[EM] Enemy CurrentTile: {enemyInfo.CurrentTile.gridLocation}"); // debug msg

        //tile.hasEnemy = true; // now has enemy moved over

        transform.position = targetPostion; //new Vector3(tile.transform.position.x, tile.transform.position.y + 0.01f, // a little y offset
            //tile.transform.position.z);

        Debug.Log($"{name} moved to {enemyInfo.CurrentTile.gridLocation}"); // debug msg
    }
    
    private Vector3 GetUnitPositionOnTile(OverlayTile1 tile)
    {
        return tile.transform.position; //new Vector3(tile.transform.position.x + visualOffset.x, tile.transform.position.y + visualOffset.y, tile.transform.position.z); // use this for both player and enemy position to keep them the same
    }
}
