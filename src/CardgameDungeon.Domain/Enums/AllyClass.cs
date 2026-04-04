namespace CardgameDungeon.Domain.Enums;

public enum AllyClass
{
    /// <summary>Frontline tank. Controls combat assignments, redirects damage, protects allies.</summary>
    Warrior,

    /// <summary>Opportunist. Bonus effects With Advantage, sneak attacks, evasion.</summary>
    Rogue,

    /// <summary>Arcane caster. Conditional effects that affect one or more combat groups. Necromancer variant recycles monsters from discard to deck.</summary>
    Mage,

    /// <summary>Healer/support. Restores HP, removes debuffs, anti-Undead, conditional buffs.</summary>
    Cleric,

    /// <summary>Scout. Reveals hand/deck/traps, favored enemy bonuses, high Initiative.</summary>
    Ranger,

    /// <summary>Holy warrior. Combat + support hybrid with sacrifice mechanics for powerful effects.</summary>
    Paladin,

    /// <summary>Card manipulator. Returns cards from hand to deck (top/bottom/shuffle) to control future draws and reshape card distribution.</summary>
    Bard,

    /// <summary>Martial artist. Discards or exiles from hand to cancel combats, attack twice, or disengage without opportunity attacks.</summary>
    Monk,

    /// <summary>Pact caster. Marks an enemy during initiative; if the marked enemy dies, triggers powerful effects (heal, draw, AOE damage, exile).</summary>
    Warlock,

    /// <summary>Scroll amplifier. Copies Scroll effects, recovers Scrolls from discard via discard/exile cost, empowers Scroll potency.</summary>
    Sorcerer,

    /// <summary>Rage bruiser. Highest STR/HP base. Exile cards from hand to enter Rage: +STR, damage reduction, and counts as double STR for elimination checks.</summary>
    Barbarian
}
