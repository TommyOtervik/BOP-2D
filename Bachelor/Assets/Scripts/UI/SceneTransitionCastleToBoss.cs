using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionCastleToBoss : MonoBehaviour
{
    private const string PLAYER_NAME = "Player";

    private const string LEVEL_NAME = "Boss_Level";
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Equals(PLAYER_NAME))
        {
            //LevelLoader.SetLevelName("Boss_Level");
            EventManager.TriggerEvent(EnumEvents.CASTLE_TO_BOSS);
            SaveSystem.SavePlayerPosition(new PlayerPositionData(39.12f, -44.12f, 0, true));
            SceneManager.LoadScene(LEVEL_NAME);
        }
    }
}
