using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CardgameDungeon.Unity.Network;

namespace CardgameDungeon.Unity.Board
{
    public enum RoomState
    {
        Upcoming,
        Current,
        Cleared
    }

    public class DungeonLayout : MonoBehaviour
    {
        [Header("Room Slots")]
        [SerializeField] private Transform[] roomSlots = new Transform[6]; // 5 rooms + 1 boss
        [SerializeField] private float roomSpacing = 2.5f;

        [Header("Room Visuals")]
        [SerializeField] private GameObject roomPrefab;
        [SerializeField] private Color upcomingColor = Color.gray;
        [SerializeField] private Color currentColor = Color.yellow;
        [SerializeField] private Color clearedColor = Color.green;
        [SerializeField] private Color bossColor = Color.red;

        [Header("Indicator")]
        [SerializeField] private Transform currentRoomIndicator;
        [SerializeField] private float indicatorBobSpeed = 2f;
        [SerializeField] private float indicatorBobHeight = 0.3f;

        private readonly List<RoomVisual> roomVisuals = new List<RoomVisual>();
        private int currentRoomIndex = -1;
        private int totalRooms;

        public int CurrentRoomIndex => currentRoomIndex;
        public int TotalRooms => totalRooms;

        public void Initialize(List<DungeonRoomMatchDto> rooms)
        {
            // Clear existing visuals
            ClearRooms();

            if (rooms == null || rooms.Count == 0)
            {
                Debug.LogWarning("[DungeonLayout] No rooms to initialize.");
                return;
            }

            totalRooms = rooms.Count;

            for (int i = 0; i < rooms.Count; i++)
            {
                DungeonRoomMatchDto roomData = rooms[i];
                Transform slotParent = GetOrCreateSlot(i);

                RoomVisual visual = new RoomVisual
                {
                    RootTransform = slotParent,
                    RoomData = roomData,
                    State = roomData.isCleared ? RoomState.Cleared : RoomState.Upcoming
                };

                // Create room visual object
                if (roomPrefab != null)
                {
                    GameObject roomObj = Instantiate(roomPrefab, slotParent.position, Quaternion.identity, slotParent);
                    visual.RoomObject = roomObj;

                    // Set room name text if available
                    TextMeshPro nameText = roomObj.GetComponentInChildren<TextMeshPro>();
                    if (nameText != null)
                    {
                        nameText.text = roomData.isBossRoom
                            ? $"BOSS\n{roomData.roomName}"
                            : $"Room {roomData.order}\n{roomData.roomName}";
                    }

                    // Set room visual renderer color
                    SpriteRenderer renderer = roomObj.GetComponentInChildren<SpriteRenderer>();
                    if (renderer != null)
                    {
                        visual.Renderer = renderer;
                    }
                }

                roomVisuals.Add(visual);
                UpdateRoomVisual(i);
            }

            Debug.Log($"[DungeonLayout] Initialized with {rooms.Count} rooms.");
        }

        public void AdvanceRoom(int roomNumber)
        {
            if (roomNumber < 0 || roomNumber >= totalRooms)
            {
                Debug.LogWarning($"[DungeonLayout] Invalid room number: {roomNumber}");
                return;
            }

            // Mark all previous rooms as cleared
            for (int i = 0; i < roomNumber; i++)
            {
                if (i < roomVisuals.Count)
                {
                    roomVisuals[i].State = RoomState.Cleared;
                    UpdateRoomVisual(i);
                }
            }

            // Set current room
            if (roomNumber < roomVisuals.Count)
            {
                roomVisuals[roomNumber].State = RoomState.Current;
                UpdateRoomVisual(roomNumber);
            }

            currentRoomIndex = roomNumber;

            // Move indicator to current room
            if (currentRoomIndicator != null && roomNumber < roomVisuals.Count)
            {
                currentRoomIndicator.position = roomVisuals[roomNumber].RootTransform.position
                    + Vector3.up * 1.5f;
                currentRoomIndicator.gameObject.SetActive(true);
            }

            HighlightRoom(roomNumber);
        }

        public void HighlightRoom(int index)
        {
            for (int i = 0; i < roomVisuals.Count; i++)
            {
                RoomVisual visual = roomVisuals[i];
                if (visual.Renderer == null) continue;

                if (i == index)
                {
                    // Highlighted room gets bright color and slight scale-up
                    visual.Renderer.color = currentColor;
                    if (visual.RoomObject != null)
                    {
                        visual.RoomObject.transform.localScale = Vector3.one * 1.1f;
                    }
                }
                else
                {
                    // Non-highlighted rooms use their state color
                    UpdateRoomVisual(i);
                    if (visual.RoomObject != null)
                    {
                        visual.RoomObject.transform.localScale = Vector3.one;
                    }
                }
            }
        }

        private void Update()
        {
            // Bob the indicator up and down
            if (currentRoomIndicator != null && currentRoomIndicator.gameObject.activeSelf)
            {
                Vector3 pos = currentRoomIndicator.localPosition;
                pos.y += Mathf.Sin(Time.time * indicatorBobSpeed) * indicatorBobHeight * Time.deltaTime;
                currentRoomIndicator.localPosition = pos;
            }
        }

        private void UpdateRoomVisual(int index)
        {
            if (index < 0 || index >= roomVisuals.Count) return;

            RoomVisual visual = roomVisuals[index];
            if (visual.Renderer == null) return;

            bool isBoss = visual.RoomData != null && visual.RoomData.isBossRoom;

            visual.Renderer.color = visual.State switch
            {
                RoomState.Cleared => clearedColor,
                RoomState.Current => isBoss ? bossColor : currentColor,
                RoomState.Upcoming => isBoss ? new Color(bossColor.r, bossColor.g, bossColor.b, 0.5f) : upcomingColor,
                _ => upcomingColor
            };
        }

        private Transform GetOrCreateSlot(int index)
        {
            // Use predefined slots if available
            if (roomSlots != null && index < roomSlots.Length && roomSlots[index] != null)
            {
                return roomSlots[index];
            }

            // Auto-generate slot position horizontally
            float startX = -(totalRooms - 1) * roomSpacing * 0.5f;
            Vector3 position = transform.position + new Vector3(startX + index * roomSpacing, 0f, 0f);

            GameObject slotObj = new GameObject($"RoomSlot_{index}");
            slotObj.transform.SetParent(transform);
            slotObj.transform.position = position;

            return slotObj.transform;
        }

        private void ClearRooms()
        {
            foreach (RoomVisual visual in roomVisuals)
            {
                if (visual.RoomObject != null)
                {
                    Destroy(visual.RoomObject);
                }
            }
            roomVisuals.Clear();
            currentRoomIndex = -1;
        }

        private class RoomVisual
        {
            public Transform RootTransform;
            public GameObject RoomObject;
            public SpriteRenderer Renderer;
            public DungeonRoomMatchDto RoomData;
            public RoomState State;
        }
    }
}
