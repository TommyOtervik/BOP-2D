
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
                Instantiate(current.gameObject, transform.position, Quaternion.identity);
            }
        }
    }
}
