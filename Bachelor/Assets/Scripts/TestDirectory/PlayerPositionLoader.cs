using UnityEngine;

public class PlayerPositionLoader : MonoBehaviour 
{
    [SerializeField] private GameObject Player;
    void Awake()
    {
        
    }

    void Start()
    {
        PlayerPositionData ppd = SaveSystem.LoadPlayerPositionData();
        if (ppd.WasFromTransitionScene)
        {
            Vector3 v3 = new Vector3(ppd.X, ppd.Y, ppd.Z);
            Player.transform.position = v3;

        }
    }
}
 