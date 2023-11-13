namespace Data;

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

public enum CardRarity
{
    COMMON,
    UNCOMMON,
    RARE,
    EPIC,
    LEGENDARY
}

public enum CardType
{
    MOVEMENT,
    CLASS,
    ACTION,
    PACKAGE
}

public enum DamageType
{
    DIVINE,
    DEMONIC,
    FIRE,
    POISON,
    LIGHTNING,
    SHARP,
    BLUNT,
    PIERCING
}

public enum TargetType
{
    SINGLE,
    SELF,
    MULTI_TWO,
    MULTI_THREE,
    MULTI_FOUR,
    ALL // All enemies and yourself
}

public enum DrawEffect
{
    DRAW,
    RETURN,
    DISCARD,
    GRAB // Implementation as stretch goal
}

public enum Buff
{
    RESISTANCE,
    ARMOR,
    CRIT_DMG_INCREASE,
    CRIT_CHANCE_INCREASE,
    TRUE_SIGHT,
}

public enum Debuff
{
    POISONED,
    CRIPPLED,
    TRAUMATIZED,
    SHOCKED,
    EXPOSED,
}

public struct DamageDice
{
    public int d4;
    public int d6;
    public int d8;
    public int d10;
    public int flat; // Flat addition, as in "2d6 + *5*"

    public override string ToString()
    {
        string final = "";

        if (d4 > 0)
            final += d4 + "d4";
        if (d6 > 0)
            final += d6 + "d6";
        if (d8 > 0)
            final += d8 + "d8";
        if (d10 > 0)
            final += d10 + "d10";

        if (flat != 0)
        {
            if (flat > 0)
                final += " + " + flat;
            else
                final += " - " + Math.Abs(flat);
        }

        return final;
    }
}

public partial class CardData : Resource
{
    [Export] private String _name;
    // Types and rarity
    [Export] private CardType _type;
    [Export] private CardRarity _rarity;
    [Export] private int _actionPointCost;
    [Export] private int _movementPointCost;

    public String Name => _name;
    public CardRarity Rarity => _rarity;
    public CardType Type => _type;
    public int ActionPointCost => _actionPointCost;
    public int MovementPointCost => _movementPointCost;

    [Export] private TargetType _target;
    public TargetType Target => _target;


    // Damage and damage types
    [Export] private Godot.Collections.Array<DamageType> _damageTypes = new();
    [Export] private Godot.Collections.Array<String> _damageDice = new();
    public Dictionary<DamageType, DamageDice> Damage;

    // Draw effects (draw/remove cards to deck; always affects player regardless of TargetType)
    [Export] private Godot.Collections.Array<DrawEffect> _drawEffects = new();
    [Export] private Godot.Collections.Array<int> _drawQuantities = new();
    public Dictionary<DrawEffect, int> DrawEffects;

    // Buff type/duration
    [Export] private Godot.Collections.Array<Buff> _buffs = new();
    [Export] private Godot.Collections.Array<int> _buffDuration = new(); // Turn based, not seconds
    public Dictionary<Buff, int> Buffs;

    // Debuff type/duration
    [Export] private Godot.Collections.Array<Debuff> _debuffs = new();
    [Export] private Godot.Collections.Array<int> _debuffDuration = new(); // Turn based, not seconds
    public Dictionary<Debuff, int> Debuffs;

    private int _uid;
    public int UID => _uid;
    private static int _usedIds = 0;

    public CardData()
    {
        _uid = _usedIds++;
    }

    public void Activate()
    {
        Damage = new();
        DrawEffects = new();
        Buffs = new();
        Debuffs = new();

        int i = 0;
        DamageDice dice;
        int dieQuantity;
        int dieDenomination;
        int flatModifier;


        foreach (DamageType t in _damageTypes)
        {
            dice = new();

            string[] parts = _damageDice[i].Split("d");
            dieQuantity = int.Parse(parts[0]);

            if (parts[1].Contains("+"))
            {
                parts = parts[1].Split("+");
                dieDenomination = int.Parse(parts[0].Trim());
                flatModifier = int.Parse(parts[1].Trim());
            }
            else if (parts[1].Contains("-"))
            {
                parts = parts[1].Split("-");
                dieDenomination = int.Parse(parts[0].Trim());
                flatModifier = int.Parse(parts[1].Trim());
                flatModifier = -flatModifier;
            }
            else
            {
                dieDenomination = int.Parse(parts[1].Trim());
                flatModifier = 0;
            }

            switch (dieDenomination)
            {
                case 4:
                    dice.d4 = dieQuantity;
                    break;
                case 6:
                    dice.d6 = dieQuantity;
                    break;
                case 8:
                    dice.d8 = dieQuantity;
                    break;
                case 10:
                    dice.d10 = dieQuantity;
                    break;
            }

            dice.flat = flatModifier;

            Damage.Add(t, dice);

            i++;
        }

        _damageTypes.Clear();
        _damageDice.Clear();

        i = 0;
        foreach (DrawEffect e in _drawEffects)
        {
            DrawEffects.Add(e, _drawQuantities[i++]);
        }

        _drawEffects.Clear();
        _drawQuantities.Clear();

        i = 0;
        foreach (Buff b in _buffs)
        {
            Buffs.Add(b, _buffDuration[i++]);
        }

        _buffs.Clear();
        _buffDuration.Clear();

        i = 0;
        foreach (Debuff d in _debuffs)
        {
            Debuffs.Add(d, _debuffDuration[i++]);
        }

        _debuffs.Clear();
        _debuffDuration.Clear();
    }

    // Operator overrides for easy comparisons of card data. Don't worry about the implementation here unless you really think it's bugged.
    public override bool Equals(object obj)
    {
        if (obj is not CardData)
            return false;
        return _uid == ((CardData)obj).UID;
    }

    public static bool operator ==(CardData c1, CardData c2)
    {
        if ((object)c1 == null)
            return (object)c2 == null;
        return c1.Equals(c2);
    }

    public static bool operator !=(CardData c1, CardData c2)
    {
        return !(c1 == c2);
    }

    public override int GetHashCode()
    {
        return _uid.GetHashCode();
    }

    public override string ToString()
    {
        return "Card " + _name;
    }

}

