using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionCastleToTutorial : MonoBehaviour
{

    private const string PLAYER_NAME = "Player";
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Equals(PLAYER_NAME))
        {
            LevelLoader.SetLevelName("Tutorial_Level");
            EventManager.TriggerEvent(EnumEvents.CASTLE_TO_TUTORIAL);
        }
    }
}


