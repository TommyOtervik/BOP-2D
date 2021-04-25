using UnityEngine;

[System.Serializable]
public class Loot
{
    public Pickup thisLoot;
    public int lootChance;
}

[CreateAssetMenu]
public class LootTable : ScriptableObject
{
    public Loot[] loot;

    public Pickup LootPickup()
    {
        int cumulativeProbability = 0;
        int currentProbability = Random.Range(0, 100);
        for (int i = 0; i < loot.Length; i++)
        {
            cumulativeProbability += loot[i].lootChance;
            if (currentProbability <= cumulativeProbability)
            {
                return loot[i].thisLoot;
            }
        }

        return null;
    }
}
