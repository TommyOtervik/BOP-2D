
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public LootTable thisLoot;
    
    protected virtual void MakeLoot()
    {
        if (thisLoot != null)
        {
            Pickup current = thisLoot.LootPickup();
            if (current != null)
            {
                Instantiate(current.gameObject, new Vector2(transform.position.x + 1f, transform.position.y + 0), Quaternion.identity);
            }
        }
    }
}
