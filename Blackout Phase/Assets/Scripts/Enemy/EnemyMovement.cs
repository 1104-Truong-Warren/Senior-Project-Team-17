using UnityEngine; 
using System.Collections.Generic; // for the List<T> and dictionary <T, T> for pathfinding// for the List<T> and dictionary <T, T> for pathfinding
using System.Collections; // for the array list we have also IEnumerator for delay funciton calls yield returns. loading map first then do something else

public class EnemyMovement : MonoBehaviour
{
    [Header("How fast enemy moves")]
    [SerializeField] private float moveSpeed = 3f; // enemy move speed

    private EnemyInfo enemyInfo; // access enemyInfo

    private void Awake()
    {
        enemyInfo = GetComponent<EnemyInfo>(); // setup the enemyinfo
    }

    public IEnumerator MoveAlong(List<OverlayTile1> path)
    {
        foreach (OverlayTile1 tile in path) // loop through all the moveable tiles in path
        {
            tile.ShowEnemyTile(); // display enemy tiles

            yield return MoveStep(tile); // delay return

            tile.HideTile(); // undo hightlight

            Debug.Log("Enemy moved visually to: " + tile.gridLocation);
            Debug.Log("EnemyInfo.currentTile says: " + enemyInfo.currentTile.gridLocation);
        }

        //if (path.Count == 0) 
        //    yield break;

        //yield return MoveStep(path[0]);
    }

    private IEnumerator MoveStep(OverlayTile1 tile)
    {
        if (enemyInfo.currentTile != null)
            enemyInfo.currentTile.hasEnemy = false; // the tile has no enemy flag

        Vector3 targetPostion = tile.transform.position + new Vector3(0, 0.01f, 0); // a little offset on y

        //var spriteRender = GetComponent<SpriteRenderer>(); // get enemey sprite

        while (Vector2.Distance(transform.position, targetPostion) > 0.01f) // if the distance is > 0.01f move towards the position
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPostion, moveSpeed * Time.deltaTime); // how fast it moves

            //// make sure the sprite works even moving 
            //if (spriteRender != null)
            //    spriteRender.sortingOrder = 999;

            yield return null;


            enemyInfo.EnemySetTile(tile); // set enemy tile

            tile.hasEnemy = true; // now has enemy moved over

            transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.01f, // a little y offset
                tile.transform.position.z);
        }
    }
}
