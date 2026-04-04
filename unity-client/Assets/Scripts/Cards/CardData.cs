using UnityEngine;

namespace CardgameDungeon.Unity.Cards
{
    public enum CardType
    {
        Ally,
        Equipment,
        Monster,
        Trap,
        DungeonRoom,
        Boss
    }

    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Unique
    }

    public enum Race
    {
        Human, Elf, Dwarf, Orc, Halfling, Dragonborn, Tiefling,
        Goblin, Undead, Demon, Beast, Construct, Elemental, Giant, Aberration, Dragon
    }

    public enum AllyClass
    {
        Warrior, Rogue, Mage, Cleric, Ranger, Paladin, Bard
    }

    public enum EquipmentSlot
    {
        // Gear — 1 per slot per ally
        Weapon, Armor, Shield, Helmet, Boots, Accessory,
        // Consumables — no limit, single-use
        Scroll, Potion, Balm, Bomb, Totem
    }

    [CreateAssetMenu(fileName = "NewCard", menuName = "CardgameDungeon/Cards/CardData")]
    public class CardData : ScriptableObject
    {
        [Header("Base Card Properties")]
        [SerializeField] private string cardId;
        [SerializeField] private string cardName;
        [SerializeField] private CardType cardType;
        [SerializeField] private Rarity rarity;
        [SerializeField] private int cost;

        public string CardId => cardId;
        public string CardName => cardName;
        public CardType CardType => cardType;
        public Rarity Rarity => rarity;
        public int Cost => cost;

        public void SetBaseData(string id, string name, CardType type, Rarity rar, int c)
        {
            cardId = id;
            cardName = name;
            cardType = type;
            rarity = rar;
            cost = c;
        }
    }

    [CreateAssetMenu(fileName = "NewAllyCard", menuName = "CardgameDungeon/Cards/AllyCardData")]
    public class AllyCardData : CardData
    {
        [Header("Ally Properties")]
        [SerializeField] private AllyClass allyClass;
        [SerializeField] private Race race;
        [SerializeField] private int strength;
        [SerializeField] private int hitPoints;
        [SerializeField] private int initiative;
        [SerializeField] private int treasure;
        [SerializeField] private bool isAmbusher;
        [SerializeField, TextArea] private string effect;

        public AllyClass AllyClass => allyClass;
        public Race Race => race;
        public int Strength => strength;
        public int HitPoints => hitPoints;
        public int Initiative => initiative;
        public int Treasure => treasure;
        public bool IsAmbusher => isAmbusher;
        public string Effect => effect;
    }

    [CreateAssetMenu(fileName = "NewEquipmentCard", menuName = "CardgameDungeon/Cards/EquipmentCardData")]
    public class EquipmentCardData : CardData
    {
        [Header("Equipment Properties")]
        [SerializeField] private EquipmentSlot slot;
        [SerializeField] private int strengthModifier;
        [SerializeField] private int hitPointsModifier;
        [SerializeField] private int initiativeModifier;
        [SerializeField, TextArea] private string effect;

        public EquipmentSlot Slot => slot;
        public int StrengthModifier => strengthModifier;
        public int HitPointsModifier => hitPointsModifier;
        public int InitiativeModifier => initiativeModifier;
        public string Effect => effect;
        public bool IsConsumable => slot >= EquipmentSlot.Scroll;
    }

    [CreateAssetMenu(fileName = "NewMonsterCard", menuName = "CardgameDungeon/Cards/MonsterCardData")]
    public class MonsterCardData : CardData
    {
        [Header("Monster Properties")]
        [SerializeField] private Race race;
        [SerializeField] private int strength;
        [SerializeField] private int hitPoints;
        [SerializeField] private int initiative;
        [SerializeField] private int treasure;
        [SerializeField, TextArea] private string effect;

        public Race Race => race;
        public int Strength => strength;
        public int HitPoints => hitPoints;
        public int Initiative => initiative;
        public int Treasure => treasure;
        public string Effect => effect;
    }

    [CreateAssetMenu(fileName = "NewTrapCard", menuName = "CardgameDungeon/Cards/TrapCardData")]
    public class TrapCardData : CardData
    {
        [Header("Trap Properties")]
        [SerializeField] private int damage;
        [SerializeField, TextArea] private string effect;

        public int Damage => damage;
        public int HitPoints => damage; // alias for consistency
        public string Effect => effect;
    }

    [CreateAssetMenu(fileName = "NewDungeonRoomCard", menuName = "CardgameDungeon/Cards/DungeonRoomCardData")]
    public class DungeonRoomCardData : CardData
    {
        [Header("Dungeon Room Properties")]
        [SerializeField] private int order;
        [SerializeField] private int monsterCostBudget;
        [SerializeField, TextArea] private string effect;

        public int Order => order;
        public int MonsterCostBudget => monsterCostBudget;
        public string Effect => effect;
    }

    [CreateAssetMenu(fileName = "NewBossCard", menuName = "CardgameDungeon/Cards/BossCardData")]
    public class BossCardData : CardData
    {
        [Header("Boss Properties")]
        [SerializeField] private Race race;
        [SerializeField] private int strength;
        [SerializeField] private int hitPoints;
        [SerializeField] private int initiative;
        [SerializeField, TextArea] private string effect;

        public Race Race => race;
        public int Strength => strength;
        public int HitPoints => hitPoints;
        public int Initiative => initiative;
        public string Effect => effect;
    }
}
