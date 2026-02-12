// Weijun

using System.Collections;
using UnityEngine;

public class PlayerFuryVisualEffect : MonoBehaviour
{
    [Header("Player Fury Visual Settings")]
    [SerializeField] private SpriteRenderer PlayerSR; // sprite for player
    //[SerializeField] private float flashDuration; // how long each flash occours
    [SerializeField] private float flashSpeed; // how fast the flashing is 

    private Color originalSR; // the original SR color

    private void Awake()
    {
        // player Sprite Render is null find it through component chidren
        if (PlayerSR == null)
            PlayerSR = GetComponentInChildren<SpriteRenderer>();

        originalSR = PlayerSR.color; // save the original color
    }

    public void FuryFlashEffect()
    {
        StopAllCoroutines(); // stops all the other on going functions in this script

        StartCoroutine(FlashStart()); // calls the Flash function
    }

    public void StopFuryFlash()
    {
        StopAllCoroutines(); // stops all the other on going functions in this script

        PlayerSR.color = originalSR; // set the player back to original color
    }

    private IEnumerator FlashStart()
    {
        //float t = 0f; // to keep track of the flash duration

        // if the t is less than the duration keep on flashing
        while (PlayerFuryMode.Instance.inFuryMode)
        {
            PlayerSR.color = Color.red; // change player SR to red

            yield return new WaitForSeconds(flashSpeed); // delay return for the flash speed

            PlayerSR.color = originalSR; // change back to oringial color

            yield return new WaitForSeconds(flashSpeed); // delay return for the flash speed

            //t += flashSpeed * 2f; // add up the time of each frame
        }
        
    }
}
