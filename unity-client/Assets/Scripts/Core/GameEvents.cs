namespace CardgameDungeon.Unity.Core
{
    // ── Card Events ──────────────────────────────────────────────────────

    public struct CardClickedEvent
    {
        public readonly Cards.CardView Card;

        public CardClickedEvent(Cards.CardView card)
        {
            Card = card;
        }
    }

    public struct CardHoverEvent
    {
        public readonly Cards.CardView Card;
        public readonly bool IsHovering;

        public CardHoverEvent(Cards.CardView card, bool isHovering)
        {
            Card = card;
            IsHovering = isHovering;
        }
    }

    public struct CardPlayedEvent
    {
        public readonly Cards.CardView Card;
        public readonly int SlotIndex;

        public CardPlayedEvent(Cards.CardView card, int slotIndex)
        {
            Card = card;
            SlotIndex = slotIndex;
        }
    }

    public struct CardPlacedEvent
    {
        public readonly Cards.CardView Card;
        public readonly int SlotIndex;
        public readonly bool IsPlayerCard;

        public CardPlacedEvent(Cards.CardView card, int slotIndex, bool isPlayerCard)
        {
            Card = card;
            SlotIndex = slotIndex;
            IsPlayerCard = isPlayerCard;
        }
    }

    public struct AllyRemovedEvent
    {
        public readonly int SlotIndex;
        public readonly bool IsPlayerCard;

        public AllyRemovedEvent(int slotIndex, bool isPlayerCard)
        {
            SlotIndex = slotIndex;
            IsPlayerCard = isPlayerCard;
        }
    }

    public struct RoomAdvancedEvent
    {
        public readonly int RoomIndex;

        public RoomAdvancedEvent(int roomIndex)
        {
            RoomIndex = roomIndex;
        }
    }

    public struct BoardClearedEvent { }
}
