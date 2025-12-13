// and some additional fixing from online sources Unity Discussion:https://discussions.unity.com/, reddit, YouTube
// I should have keep tract on the exact page but I forgot to save some of the links 
// this is just a sorting layer thing for the enemy sprite so it would appeared on top
// had some trouble with some sprite bug where enemy sprite sinks down into the grid map
// but everything is fixed. It was some kind of pathing bug and visual bug combined.
// Weijun

using UnityEngine; // default

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
