using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SceneTransition : MonoBehaviour
{
    private const string PLAYER_NAME = "Player";
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Equals(PLAYER_NAME))
        {
            LevelLoader.SetLevelName("Castle_Level");
            EventManager.TriggerEvent(EnumEvents.TUTORIAL_TO_CASTLE);
        }
        
    }
}
