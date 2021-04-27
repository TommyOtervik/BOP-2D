using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionCastleToBoss : MonoBehaviour
{
    private const string PLAYER_NAME = "Player";
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Equals(PLAYER_NAME))
        {
            LevelLoader.SetLevelName("Boss_Level");
            EventManager.TriggerEvent(EnumEvents.CASTLE_TO_BOSS);
        }
    }
}
