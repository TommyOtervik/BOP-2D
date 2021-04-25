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
    private UnityAction castleToTutorialListener;

    private static string levelNameChange;

    //private const int TUTORIAL_BUILD_INDEX = 0;
    //private const int CASTLE_BUILD_INDEX = 1;


    private void Awake()
    {
        tutorialToCastleListener = new UnityAction(LoadLevel);
        castleToTutorialListener = new UnityAction(LoadLevel);

        EventManager.TriggerEvent(EnumEvents.LOAD_PLAYER);
    }

    public static void SetLevelName(string name)
    {
        levelNameChange = name;
    }

    private void LoadLevel()
    {
        // Castle_Level
        // Tutorial_Level
        
        StartCoroutine(LoadLevelE(levelNameChange));
    }

    IEnumerator LoadLevelE(string sceneName)
    {
        // Player anim
        transition.SetTrigger(ANIM_TRIGGER_START);
        // Wait
        yield return new WaitForSeconds(transitionTime);
        // Load scene
        SceneManager.LoadScene(sceneName);
    }

    private void OnEnable()
    {
        EventManager.StartListening(EnumEvents.TUTORIAL_TO_CASTLE, tutorialToCastleListener);
        EventManager.StartListening(EnumEvents.CASTLE_TO_TUTORIAL, castleToTutorialListener);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EnumEvents.TUTORIAL_TO_CASTLE, tutorialToCastleListener);
        EventManager.StopListening(EnumEvents.CASTLE_TO_TUTORIAL, castleToTutorialListener);
    }

}
