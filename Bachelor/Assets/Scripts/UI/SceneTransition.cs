using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    private const string PLAYER_NAME = "Player";
    private const string LEVEL_NAME = "Castle_level";

    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Equals(PLAYER_NAME))
        {
            LevelLoader.SetLevelName(LEVEL_NAME);
            EventManager.TriggerEvent(EnumEvents.TUTORIAL_TO_CASTLE);
        }
        
    }
}
