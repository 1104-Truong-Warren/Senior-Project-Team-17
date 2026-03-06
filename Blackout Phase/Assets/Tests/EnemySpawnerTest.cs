// refrence from Elison's test code
// Weijun

using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools; // needed for the UnityTest

public class EnemySpawnerTest
{
    // set up all the in GameObjecs for test
    private GameObject mapGameObj;
    private GameObject turnMGameObj;
    private GameObject enemySpawnerGameObj;
    private GameObject tileGameObj;

    // accessor for other scripts
    private MapManager1 map;
    private TurnManager turnManager;
    private EnemySpwawan enemySpawner;

    // enemy prefab/spawn position
    private GameObject enemyPrefab;

    private Vector2Int spawnPosition = new Vector2Int(3, 2); // for spawn test position

    //=============================
    // Test setup for enemy spawn
    // ============================
    [SetUp]
    public void SetUp()
    {
        // creat a new GameObj and assign it to the MapManager and give map the access
        mapGameObj = new GameObject("MapManager_Test");

        map = mapGameObj.AddComponent<MapManager1>();

        MapManager1.SetInstanceForTest(map);

        // creat a new GameObj and assign it to the TurnManager and give turnManager the access
        turnMGameObj = new GameObject("TurnManager_Test");

        turnManager = turnMGameObj.AddComponent<TurnManager>();

        TurnManager.SetInstaceForEnemyTest(turnManager);

        // creat a new GameObj and assign it to the OverlayTile and give tile the access
        tileGameObj = new GameObject("TilePosition_Test");

        var tile = tileGameObj.AddComponent<OverlayTile1>();

        //  get the grid location from the spawn position in (x,y,) ignore z
        tile.gridLocation = new Vector3Int(spawnPosition.x, spawnPosition.y, 0);

        // the world position on the map
        tileGameObj.transform.position = new Vector3(10f, 20f, 0f);

        // use map to access the world tile position
        map.map = new Dictionary<Vector2Int, OverlayTile1>();

        map.map[spawnPosition] = tile;

        // enemy prefabe for test
        enemyPrefab = new GameObject("EnemyPrefab_Test");

        enemyPrefab.AddComponent<EnemyController1>();

        // Get enemyInfo through the children<>
        var chidren = new GameObject("EnemyInfoChildren");

        chidren.transform.SetParent(enemyPrefab.transform);

        chidren.AddComponent<EnemyInfo>();

        // enemy spawner
        enemySpawnerGameObj = new GameObject("EnemySpaner_Test");

        enemySpawner = enemySpawnerGameObj.AddComponent<EnemySpwawan>();

        // set up the enemy stats
        var stats = ScriptableObject.CreateInstance<EnemyStatsScripObj>();

        stats.maxHP = 50; // HP just for the test

        // pass the importants info to set up the spawner
        enemySpawner.ConfigureationTest(enemyPrefab, stats, spawnPosition, new List<Vector2Int> { spawnPosition});
    }

    //=============================
    // Destory the GameObjs
    // ============================
    [TearDown]
    public void TearDown()
    {
        // deletes the in gameObjs
        Object.DestroyImmediate(enemySpawnerGameObj);

        Object.DestroyImmediate(enemyPrefab);
        
        Object.DestroyImmediate(tileGameObj);
        
        Object.DestroyImmediate(mapGameObj);
        
        Object.DestroyImmediate(turnMGameObj);
    }

    //=============================
    // Run the test 
    // ============================
    [UnityTest]
    public IEnumerator SpawnAndCreateEnemy()
    {
        yield return null; // instantiate begins 

        // test to see which gameObj is null
        Assert.IsNotNull(MapManager1.Instance, "MapManager is null (haven't awaken)");

        Assert.IsNotNull(MapManager1.Instance.map, "Map.map is null");

        Assert.IsNotNull(enemyPrefab, "enemyPrefab is null");

        Assert.IsNotNull(TurnManager.Instance, "TurnManager is null");

        enemySpawner.Spawn_ForTest(); // calls the spawn test after ready

        yield return null; // wait

        // test to see if spawn position is working
        var tile = map.GetTile(spawnPosition);

        Assert.IsNotNull(tile);

        Assert.IsTrue(tile.hasEnemy, "Tile hasEnemy after Enemy Spawn");

        // test to see if enemyInfo is created after spawn
        var enemyInfo = Object.FindFirstObjectByType<EnemyInfo>();

        Assert.IsNotNull(enemyInfo, "EnemyInfo Exist after spawn");

        // check to see if the expected position matches
        Vector3 expectedPosiiton = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.01f, tile.transform.position.z);

        Assert.That(Vector3.Distance(enemyInfo.transform.root.position, expectedPosiiton), Is.LessThan(0.05f),
            "Enemy spawn at the tile position (small y offset)");
    }
}
