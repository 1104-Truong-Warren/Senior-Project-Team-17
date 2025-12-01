using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public enum TurnPhase
{
    Player,  // player
    Enemy,   // enemies
    UI       // UI
}

public class TurnManager : MonoBehaviour
{
    [Header("Enemies")]
    [SerializeField] private List<EnemyController> enemies = new List<EnemyController>(); // set up the enmey controller

    public static TurnManager Instance { get; private set; }  // accessor for other scripts
    public TurnPhase CurrentPhase { get; private set; } = TurnPhase.Player; // player's turn

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

        CurrentPhase = TurnPhase.Player; // Palyer can not start turn

        EnemyController[] found = FindObjectsByType<EnemyController>(FindObjectsSortMode.InstanceID); // got through the list and find enemies

        enemies.AddRange(found); // add the nemeies

        Debug.Log($"TurnManager: Found {enemies.Count} enemies in scene."); // debug

        isInitialized = true; // flag everything is set up

        Debug.Log("Turnmanager initialized"); // debug msg
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
        if (CurrentPhase == TurnPhase.Player && Input.GetKeyDown(KeyCode.Space))  // End player's turn by using space, can but change to UI instead
            EndPlayerTurn();
    }

    private bool AllEnemiesReady()
    {
        foreach (var enemy in enemies) // use a loop to check if enemies ar setup
        {
            if (enemy == null) continue; // if not found skip

            else if (!enemy.Initialized) return false; // if not initialized set it to false
        }

        return true;// else true
    }

    public void RegisterEnemy(EnemyController enemy)
    {
        if (!enemies.Contains(enemy)) // enemies not found add them
        {
            enemies.Add(enemy); // add enemies

            Debug.Log($"TurnManager: Registerd enemy {enemy.name}"); // debug
        }
    }

    public void EndPlayerTurn()
    {
        if (CurrentPhase != TurnPhase.Player) return; // not player's turn get out

        Debug.Log("Player turn Ended -> Enemy Phase Starting");

        StartCoroutine(EnemyPhase()); // Enemy's phase
    }

    private IEnumerator EnemyPhase()
    {
        CurrentPhase = TurnPhase.Enemy; // currently enemy's phase

        Debug.Log("Enemy Phase Start"); // debug

        foreach (EnemyController enemy in enemies)   // each enemies take a turn, if not found continue next
        {
            if (enemy == null) continue; // keeps running even if enemy not found
            
                Debug.Log($"TurnManager: Enemy taking turn -> {enemy.name}"); // which enemy

                yield return StartCoroutine(enemy.TakeTurn()); // each enemy 

                yield return new WaitForSeconds(0.1f); // another delay
                             
        }
        Debug.Log("All enemies completed their turns -> Player turn Starting"); // debug

        CurrentPhase = TurnPhase.Player; // back to player's phase after enmey's turn
    }

    // Add this to your UI script
    
    //public void OnEndTurnButton()
    //{
    //    TurnManager.Instance.EndPlayerTurn(); // ends player's turn
    //}
}


