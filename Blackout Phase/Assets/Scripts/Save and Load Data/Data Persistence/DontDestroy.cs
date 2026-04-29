// Warren

// The purpose of this script is it allows the GameObject's data (such as the player) to not be resetted after going to the next scene.
// It allows the data to persist throughout each level.

// Source: https://youtu.be/hzdADY2LkJU?si=8p36BHRwNmFUezxC

using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    private static GameObject[] persistentObjects = new GameObject[4];
    public int objectIndex;

    void Awake()
    {
        if (persistentObjects[objectIndex] == null)
        {
            persistentObjects[objectIndex] = gameObject;
            DontDestroyOnLoad(gameObject);
        }

        else if (persistentObjects[objectIndex] != gameObject)
        {
            Destroy(gameObject);
        }
    }
}
