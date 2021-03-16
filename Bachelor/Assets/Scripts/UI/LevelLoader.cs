using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private Animator transition;
    [SerializeField] private float transitionTime = 1f;

    private const string ANIM_TRIGGER_START = "Start";

    private UnityAction tutorialToCastleListener;

    //private const int TUTORIAL_BUILD_INDEX = 0;
    //private const int CASTLE_BUILD_INDEX = 1;


    private void Awake()
    {
        tutorialToCastleListener = new UnityAction(LoadNextLevel);
        EventManager.TriggerEvent(EnumEvents.LOAD_PLAYER);
    }

    private void LoadNextLevel()
    {
        
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
        
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        // Player anim
        transition.SetTrigger(ANIM_TRIGGER_START);
        // Wait
        yield return new WaitForSeconds(transitionTime);
        // Load scene
        SceneManager.LoadScene(levelIndex);  
    }

    private void OnEnable()
    {
        EventManager.StartListening(EnumEvents.TUTORIAL_TO_CASTLE, tutorialToCastleListener);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EnumEvents.TUTORIAL_TO_CASTLE, tutorialToCastleListener);
    }

}
