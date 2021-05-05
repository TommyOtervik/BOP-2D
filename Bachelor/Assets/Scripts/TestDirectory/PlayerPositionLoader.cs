using UnityEngine;

public class PlayerPositionLoader : MonoBehaviour 
{
    // Kom du fra annen scene eller ikke? 
    [SerializeField] private GameObject Player;
    void Awake()
    {
        
    }

    void Start()
    {
        //SaveSystem.SavePlayerPosition(new PlayerPositionData(1, 1, 2, false));
        PlayerPositionData ppd = SaveSystem.LoadPlayerPositionData();
        if (ppd.WasFromTransitionScene)
        {
            Vector3 v3 = new Vector3(ppd.X, ppd.Y, ppd.Z);
            Player.transform.position = v3;

        }
    }
}
 