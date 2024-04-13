namespace GameServer.GameLogic;

/// <summary>
/// Additional properties of an armor (if it changes to an item).
/// </summary>
public record ArmorProperties
{
    public int CurrentHealth { get; init; }
}

/// <summary>
/// Factory for converting between items and armors.
/// </summary>
public class ArmorFactory
{
    /// <summary>
    /// Create armor from an item.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static Armor CreateFromItem(IItem item)
    {
        if (item.Kind != IItem.ItemKind.Armor)
        {
            throw new ArgumentException($"Item kind {item.Kind} is not an armor.");
        }

        int maxHealth = item.ItemSpecificName switch
        {
            "PRIMARY_ARMOR" => Constant.PRIMARY_ARMOR_DEFENSE,
            "PREMIUM_ARMOR" => Constant.PREMIUM_ARMOR_DEFENSE,
            "NO_ARMOR" => throw new ArgumentException("NO_ARMOR cannot be converted to armor."),
            _ => throw new ArgumentException($"Item specific id {item.ItemSpecificName} is not valid for armor.")
        };

        if (item.AdditionalProperties is null)
        {

            return new Armor(item.ItemSpecificName, maxHealth);
        }
        else if (item.AdditionalProperties is ArmorProperties properties)
        {
            return new Armor(item.ItemSpecificName, maxHealth, properties.CurrentHealth);
        }
        else
        {
            throw new ArgumentException(
                $"Additional properties {item.AdditionalProperties.GetType().Name} is not valid for armor."
            );
        }
    }

    public static IItem ToItem(Armor armor, int count)
    {
        if (armor.ItemSpecificName == "NO_ARMOR")
        {
            throw new ArgumentException("NO_ARMOR cannot be converted to item.");
        }

        return new Item(IItem.ItemKind.Armor, armor.ItemSpecificName, count)
        {
            AdditionalProperties = new ArmorProperties
            {
                CurrentHealth = armor.Health
            }
        };

    }
}

/// <summary>
/// Armor can be worn by a player to protect them from damage.
/// </summary>
public class Armor
{
    public string ItemSpecificName { get; }
    public int Health { get; private set; }
    public int MaxHealth { get; }

    public Armor(string itemSpecificName, int maxHealth, int? currentHealth = null)
    {
        ItemSpecificName = itemSpecificName;
        MaxHealth = maxHealth;
        Health = currentHealth ?? maxHealth;
    }

    public int Hurt(int Damage)
    {
        if (Health > Damage)
        {
            Health -= Damage;
            return 0;
        }
        else
        {
            Health = 0;
            return (Damage - Health);
        }
    }
}
