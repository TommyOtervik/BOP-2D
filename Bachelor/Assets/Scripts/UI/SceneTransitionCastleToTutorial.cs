using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class SceneTransitionCastleToTutorial : MonoBehaviour
{
    private const string PLAYER_NAME = "Player";
    private const string LEVEL_NAME = "Tutorial_Level";
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Equals(PLAYER_NAME))
        {
             //LevelLoader.SetLevelName("Tutorial_Level");
             EventManager.TriggerEvent(EnumEvents.CASTLE_TO_TUTORIAL);
             SaveSystem.SavePlayerPosition(new PlayerPositionData(219.37f, -0.07f, 0, true));
             SceneManager.LoadScene(LEVEL_NAME);
        }
    }
}


