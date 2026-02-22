// from https://youtu.be/CE9VOZivb3I?si=Z2N8ls_wEEI9CzB2
// Ellison
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;

    public TransitionSounds transitionSounds;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.N)) // if N is clicked
        {
            LoadNextLevel();
            transitionSounds.PlayTransitionSound();
        }
    }

    public void LoadNextLevel()
    {
        // Load the next level in the build settings
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        // Play transition animation
        transition.SetTrigger("Start");

        // Wait for the animation to finish
        yield return new WaitForSeconds(transitionTime);

        // Load next scene
        SceneManager.LoadScene(levelIndex);
    }
}
