using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Pontos")]
    public int points;

    [Header("Colecionáveis")]
    public CollectibleData[] collectiblePool;
    public int rollCost = 50;
    public List<CollectibleData> collected = new List<CollectibleData>();

    public void AddPoints(int amount)
    {
        points += amount;
    }

    public CollectibleData RollCollectible()
    {
        if (points < rollCost) return null;

        var available = new List<CollectibleData>();
        foreach (var item in collectiblePool)
        {
            if (item != null && !collected.Contains(item))
                available.Add(item);
        }

        if (available.Count == 0) return null;

        points -= rollCost;
        var picked = available[Random.Range(0, available.Count)];
        collected.Add(picked);
        return picked;
    }
}
