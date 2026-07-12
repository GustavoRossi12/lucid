using UnityEngine;

[CreateAssetMenu(menuName = "Collectible Item")]
public class CollectibleData : ScriptableObject
{
    public enum Rarity { Common, Uncommon, Rare, Epic }

    public string itemName;
    public Sprite icon;
    public Rarity rarity;

    public static Color GetRarityColor(Rarity r)
    {
        switch (r)
        {
            case Rarity.Common:    return Color.white;
            case Rarity.Uncommon:  return new Color(0.3f, 1f, 0.3f);
            case Rarity.Rare:      return new Color(0.2f, 0.6f, 1f);
            case Rarity.Epic:      return new Color(1f, 0.5f, 0f);
            default:               return Color.white;
        }
    }
}
