using UnityEngine;

public class IsSorted : MonoBehaviour
{
    private SpriteRenderer spriteRenderer; // for the enemy

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // setup the sprite render
    }

    private void LateUpdate()
    {
        spriteRenderer.sortingOrder = -(int)(transform.position.y * 100) + 10000; //  y to highter behind to make it stable close to camera
    }
}
