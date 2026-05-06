// used this video as reference: https://www.youtube.com/watch?v=vkgwjRO5QL0
// add visual offset to the sprite renderers 
// Weijun

using UnityEngine;

public class UnitVisualOffset : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private Transform spriteVisual; // sprite shift offset
    [SerializeField] private Vector2 spriteOffset = new Vector2(0f, -0.25f); // the actual offset values

    private void Awake()
    {
        // if the sprite transform is found get the access of the children
        if (spriteVisual == null)
            spriteVisual = GetComponentInChildren<SpriteRenderer>()?.transform;

        // if it's found
        if (spriteVisual != null)
            spriteVisual.localPosition = new Vector3(spriteOffset.x, spriteOffset.y ,spriteVisual.localPosition.z); // set the offset of x,y ignore z

        Debug.Log($"[UVO] Offest applied to:{spriteVisual?.name} | local: {spriteVisual?.localPosition}"); // debug msg
    }


}

