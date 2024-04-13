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
    public static IArmor CreateFromItem(IItem item)
    {
        if (item.Kind != IItem.ItemKind.Armor)
        {
            throw new ArgumentException($"Item kind {item.Kind} is not an armor.");
        }

        int maxHealth = item.ItemSpecificName switch
        {
            "NO_ARMOR" => Constant.NO_ARMOR_DEFENSE,
            "PRIMARY_ARMOR" => Constant.PRIMARY_ARMOR_DEFENSE,
            "PREMIUM_ARMOR" => Constant.PREMIUM_ARMOR_DEFENSE,
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

    public static IItem ToItem(IArmor armor, int count)
    {
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
public class Armor : IArmor
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
