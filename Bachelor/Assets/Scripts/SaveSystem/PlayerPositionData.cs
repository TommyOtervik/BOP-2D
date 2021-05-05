[System.Serializable]
public class PlayerPositionData
{
    private float x;
    private float y;
    private float z = 0;
    private bool wasFromTransitionScene;


    public PlayerPositionData(float x, float y, float z, bool wasFromTransitionScene)
    {
        this.x = x;
        this.y = y;
        this.z = 0;
        this.wasFromTransitionScene = wasFromTransitionScene;
    }

    public PlayerPositionData(PlayerPositionData ppd)
    {
        this.x = ppd.x;
        this.y = ppd.y;
        this.z = 0;
        this.wasFromTransitionScene = ppd.wasFromTransitionScene;
    }

    public float X
    {
        get => x;
        set => x = value;
    }

    public float Y
    {
        get => y;
        set => y = value;
    }

    public float Z
    {
        get => z;
        set => z = value;
    }

    public bool WasFromTransitionScene
    {
        get => wasFromTransitionScene;
        set => wasFromTransitionScene = value;
    }
}