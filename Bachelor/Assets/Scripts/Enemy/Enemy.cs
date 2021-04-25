
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public LootTable thisLoot;
    
    protected void MakeLoot()
    {
        if (thisLoot != null)
        {
            Pickup current = thisLoot.LootPickup();
            if (current != null)
            {
                Instantiate(current.gameObject, new Vector2(transform.position.x + 0.5f, transform.position.y + 3), Quaternion.identity);
            }
        }
    }
}
