using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CardgameDungeon.Unity.Core;
using CardgameDungeon.Unity.Network;

namespace CardgameDungeon.Unity.UI
{
    public class DeckBuilderUI : MonoBehaviour
    {
        private const int AdventurerMax = 40;
        private const int EnemyMax = 40;
        private const int RoomMax = 5;
        private const string DeckPrefKeyPrefix = "DeckBuilder.ActiveDeckId.";

        [Header("Layout")]
        [SerializeField] private Transform collectionGrid;
        [SerializeField] private Transform deckPanel;
        [SerializeField] private GameObject collectionRowPrefab;
        [SerializeField] private GameObject deckRowPrefab;

        [Header("Filters")]
        [SerializeField] private TMP_InputField searchInput;
        [SerializeField] private TMP_Dropdown typeDropdown;
        [SerializeField] private Button refreshCollectionButton;

        [Header("Counters")]
        [SerializeField] private TMP_Text adventurerCountText;
        [SerializeField] private TMP_Text enemyCountText;
        [SerializeField] private TMP_Text roomCountText;
        [SerializeField] private TMP_Text bossCountText;

        [Header("Actions")]
        [SerializeField] private Button saveButton;
        [SerializeField] private Button validateButton;
        [SerializeField] private Button clearDeckButton;
        [SerializeField] private Button backButton;

        [Header("Status")]
        [SerializeField] private TMP_Text statusText;

        private readonly List<string> _adventurerIds = new();
        private readonly List<string> _enemyIds = new();
        private readonly List<string> _roomIds = new();
        private readonly Dictionary<string, PoolEntry> _poolById = new(StringComparer.OrdinalIgnoreCase);
        private readonly List<PoolEntry> _pool = new();

        private string _bossId = string.Empty;
        private string _deckId = string.Empty;
        private bool _isBusy;
        private bool _loadedSavedDeck;

        private Guid PlayerId => GameManager.Instance.CurrentPlayerId;
        private string DeckPrefKey => $"{DeckPrefKeyPrefix}{PlayerId}";

        private sealed class PoolEntry
        {
            public string Id;
            public string Name;
            public string Type;
            public string Rarity;
            public int Cost;
            public int Available;
            public int Reserved;
        }

        private enum Bucket { Adventurer, Enemy, Room, Boss, Unknown }

        private void OnEnable()
        {
            if (saveButton != null) saveButton.onClick.AddListener(SaveDeck);
            if (validateButton != null) validateButton.onClick.AddListener(ValidateDeck);
            if (clearDeckButton != null) clearDeckButton.onClick.AddListener(ClearDeck);
            if (refreshCollectionButton != null) refreshCollectionButton.onClick.AddListener(LoadCollection);
            if (searchInput != null) searchInput.onValueChanged.AddListener(_ => RenderCollection());
            if (typeDropdown != null) typeDropdown.onValueChanged.AddListener(_ => RenderCollection());
            if (backButton != null) backButton.onClick.AddListener(() => GameManager.Instance.GoToSceneWithLoading("MainMenu"));
            LoadCollection();
        }

        private void OnDisable()
        {
            if (saveButton != null) saveButton.onClick.RemoveAllListeners();
            if (validateButton != null) validateButton.onClick.RemoveAllListeners();
            if (clearDeckButton != null) clearDeckButton.onClick.RemoveAllListeners();
            if (refreshCollectionButton != null) refreshCollectionButton.onClick.RemoveAllListeners();
            if (searchInput != null) searchInput.onValueChanged.RemoveAllListeners();
            if (typeDropdown != null) typeDropdown.onValueChanged.RemoveAllListeners();
            if (backButton != null) backButton.onClick.RemoveAllListeners();
        }

        public void LoadCollection()
        {
            if (_isBusy) return;
            SetBusy(true);
            SetStatus("Carregando coleção...");
            GameManager.Instance.Api.GetCollection(this, PlayerId, response =>
            {
                BuildPool(response?.cards);
                BindTypeFilter();
                ReconcileSelection();
                if (!_loadedSavedDeck)
                {
                    _loadedSavedDeck = true;
                    LoadSavedDeck();
                    return;
                }
                RenderAll();
                SetBusy(false);
                SetStatus($"Coleção pronta: {_pool.Count} cartas únicas.");
            }, error => { SetBusy(false); SetStatus($"Erro ao carregar coleção: {error}"); });
        }

        private void LoadSavedDeck()
        {
            var persisted = PlayerPrefs.GetString(DeckPrefKey, string.Empty);
            if (!Guid.TryParse(persisted, out var deckGuid))
            {
                _deckId = string.Empty;
                RenderAll();
                SetBusy(false);
                SetStatus("Nenhum deck salvo encontrado.");
                return;
            }

            _deckId = persisted;
            GameManager.Instance.Api.GetDeck(this, deckGuid, deck =>
            {
                ApplyDeck(deck);
                RenderAll();
                SetBusy(false);
                SetStatus("Deck salvo carregado.");
            }, error =>
            {
                _deckId = string.Empty;
                PlayerPrefs.DeleteKey(DeckPrefKey);
                PlayerPrefs.Save();
                RenderAll();
                SetBusy(false);
                SetStatus($"Falha ao carregar deck salvo: {error}");
            });
        }

        private void BuildPool(List<OwnedCardDto> cards)
        {
            _poolById.Clear();
            _pool.Clear();
            if (cards == null) return;

            foreach (var group in cards.Where(c => c != null && !string.IsNullOrWhiteSpace(c.cardId)).GroupBy(c => c.cardId))
            {
                var first = group.First();
                var entry = new PoolEntry
                {
                    Id = first.cardId,
                    Name = first.cardName ?? "Unknown",
                    Type = first.cardType ?? "Unknown",
                    Rarity = first.rarity ?? "Unknown",
                    Cost = first.cost,
                    Available = group.Count(x => !x.isReserved),
                    Reserved = group.Count(x => x.isReserved)
                };
                _pool.Add(entry);
                _poolById[entry.Id] = entry;
            }
        }

        private void BindTypeFilter()
        {
            if (typeDropdown == null) return;
            var current = typeDropdown.options.Count > 0 ? typeDropdown.options[typeDropdown.value].text : "All";
            var options = _pool.Select(x => x.Type).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToList();
            options.Insert(0, "All");
            typeDropdown.ClearOptions();
            typeDropdown.AddOptions(options);
            var idx = options.FindIndex(x => x.Equals(current, StringComparison.OrdinalIgnoreCase));
            typeDropdown.value = idx >= 0 ? idx : 0;
            typeDropdown.RefreshShownValue();
        }

        private void ApplyDeck(DeckResponse deck)
        {
            _adventurerIds.Clear();
            _enemyIds.Clear();
            _roomIds.Clear();
            _bossId = string.Empty;
            if (deck?.adventurerCards != null) _adventurerIds.AddRange(deck.adventurerCards.Where(x => x != null).Select(x => x.id));
            if (deck?.enemyCards != null) _enemyIds.AddRange(deck.enemyCards.Where(x => x != null).Select(x => x.id));
            if (deck?.dungeonRooms != null) _roomIds.AddRange(deck.dungeonRooms.Where(x => x != null).Select(x => x.id));
            _bossId = deck?.boss?.id ?? string.Empty;
            ReconcileSelection();
        }

        private void ReconcileSelection()
        {
            _adventurerIds.RemoveAll(id => !CanKeep(id, Bucket.Adventurer));
            _enemyIds.RemoveAll(id => !CanKeep(id, Bucket.Enemy));
            _roomIds.RemoveAll(id => !CanKeep(id, Bucket.Room));
            if (!string.IsNullOrWhiteSpace(_bossId) && !CanKeep(_bossId, Bucket.Boss)) _bossId = string.Empty;
        }

        private bool CanKeep(string id, Bucket bucket)
        {
            return _poolById.TryGetValue(id, out var entry) && entry.Available > 0 && Resolve(entry.Type) == bucket;
        }

        private void RenderAll()
        {
            RenderCollection();
            RenderDeck();
            UpdateCounters();
        }

        private void RenderCollection()
        {
            if (collectionGrid == null) return;
            foreach (Transform child in collectionGrid) Destroy(child.gameObject);

            var search = searchInput != null ? searchInput.text?.Trim() ?? string.Empty : string.Empty;
            var type = typeDropdown != null && typeDropdown.options.Count > 0 ? typeDropdown.options[typeDropdown.value].text : "All";
            if (type.Equals("All", StringComparison.OrdinalIgnoreCase)) type = string.Empty;

            var entries = _pool.Where(e =>
                    (string.IsNullOrWhiteSpace(search) || e.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0) &&
                    (string.IsNullOrWhiteSpace(type) || e.Type.Equals(type, StringComparison.OrdinalIgnoreCase)))
                .OrderBy(e => e.Type).ThenBy(e => e.Name);

            var any = false;
            foreach (var entry in entries)
            {
                any = true;
                var canAdd = CanAdd(entry);
                var text = $"{entry.Name} | {entry.Type} | {entry.Rarity} | C {entry.Cost} | {entry.Available} disp";
                CreateRow(collectionGrid, collectionRowPrefab, text, "Add", canAdd ? () => AddCard(entry.Id) : null);
            }
            if (!any) CreateRow(collectionGrid, collectionRowPrefab, "Nenhuma carta para os filtros.", "", null);
        }

        private bool CanAdd(PoolEntry entry)
        {
            if (entry.Available <= 0) return false;
            if (IsSelected(entry.Id)) return false;
            return Resolve(entry.Type) switch
            {
                Bucket.Adventurer => _adventurerIds.Count < AdventurerMax,
                Bucket.Enemy => _enemyIds.Count < EnemyMax,
                Bucket.Room => _roomIds.Count < RoomMax,
                Bucket.Boss => string.IsNullOrWhiteSpace(_bossId),
                _ => false
            };
        }

        private void AddCard(string cardId)
        {
            if (!_poolById.TryGetValue(cardId, out var entry)) return;
            if (!CanAdd(entry)) { SetStatus($"Não foi possível adicionar {entry.Name}."); return; }

            switch (Resolve(entry.Type))
            {
                case Bucket.Adventurer: _adventurerIds.Add(cardId); break;
                case Bucket.Enemy: _enemyIds.Add(cardId); break;
                case Bucket.Room: _roomIds.Add(cardId); break;
                case Bucket.Boss: _bossId = cardId; break;
            }
            RenderAll();
            SetStatus($"{entry.Name} adicionado.");
        }

        private void RenderDeck()
        {
            if (deckPanel == null) return;
            foreach (Transform child in deckPanel) Destroy(child.gameObject);

            RenderBucket("Adventurers", _adventurerIds);
            RenderBucket("Enemies", _enemyIds);
            RenderBucket("Rooms", _roomIds);

            CreateRow(deckPanel, deckRowPrefab, $"=== Boss ({(string.IsNullOrWhiteSpace(_bossId) ? 0 : 1)}) ===", "", null);
            if (string.IsNullOrWhiteSpace(_bossId)) CreateRow(deckPanel, deckRowPrefab, "Empty", "", null);
            else if (_poolById.TryGetValue(_bossId, out var boss))
                CreateRow(deckPanel, deckRowPrefab, $"{boss.Name} | {boss.Rarity} | C {boss.Cost}", "Remove", () => RemoveCard(_bossId));
        }

        private void RenderBucket(string title, List<string> ids)
        {
            CreateRow(deckPanel, deckRowPrefab, $"=== {title} ({ids.Count}) ===", "", null);
            if (ids.Count == 0) { CreateRow(deckPanel, deckRowPrefab, "Empty", "", null); return; }
            foreach (var id in ids.OrderBy(x => _poolById.TryGetValue(x, out var e) ? e.Name : x))
            {
                if (!_poolById.TryGetValue(id, out var entry)) continue;
                CreateRow(deckPanel, deckRowPrefab, $"{entry.Name} | {entry.Type} | {entry.Rarity} | C {entry.Cost}", "Remove", () => RemoveCard(id));
            }
        }

        private void RemoveCard(string id)
        {
            if (_adventurerIds.Remove(id) || _enemyIds.Remove(id) || _roomIds.Remove(id) || string.Equals(_bossId, id, StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(_bossId, id, StringComparison.OrdinalIgnoreCase)) _bossId = string.Empty;
                RenderAll();
                SetStatus("Carta removida.");
            }
        }

        private bool IsSelected(string id) => _adventurerIds.Contains(id) || _enemyIds.Contains(id) || _roomIds.Contains(id) || string.Equals(_bossId, id, StringComparison.OrdinalIgnoreCase);
        private static Bucket Resolve(string type) => type switch
        {
            "Ally" or "Equipment" => Bucket.Adventurer,
            "Monster" or "Trap" => Bucket.Enemy,
            "DungeonRoom" => Bucket.Room,
            "Boss" => Bucket.Boss,
            _ => Bucket.Unknown
        };

        public void SaveDeck()
        {
            if (_isBusy) return;
            if (!IsComplete(out var err)) { SetStatus(err); return; }
            if (!Guid.TryParse(_bossId, out _)) { SetStatus("Boss inválido."); return; }

            var create = new CreateDeckRequest
            {
                playerId = PlayerId.ToString(),
                adventurerCardIds = _adventurerIds.ToList(),
                enemyCardIds = _enemyIds.ToList(),
                dungeonRoomIds = _roomIds.ToList(),
                bossId = _bossId
            };

            SetBusy(true);
            if (string.IsNullOrWhiteSpace(_deckId))
            {
                GameManager.Instance.Api.CreateDeck(this, create, response => OnSaved(response?.id), error => OnSaveError(error, "criar"));
                return;
            }

            if (!Guid.TryParse(_deckId, out var deckGuid)) { SetBusy(false); SetStatus("DeckId inválido."); return; }
            var update = new UpdateDeckRequest { adventurerCardIds = create.adventurerCardIds, enemyCardIds = create.enemyCardIds, dungeonRoomIds = create.dungeonRoomIds, bossId = create.bossId };
            GameManager.Instance.Api.UpdateDeck(this, deckGuid, update, response => OnSaved(response?.id), error => OnSaveError(error, "atualizar"));
        }

        private void OnSaved(string responseDeckId)
        {
            _deckId = string.IsNullOrWhiteSpace(responseDeckId) ? _deckId : responseDeckId;
            if (!string.IsNullOrWhiteSpace(_deckId))
            {
                PlayerPrefs.SetString(DeckPrefKey, _deckId);
                PlayerPrefs.Save();
            }
            SetBusy(false);
            SetStatus($"Deck salvo: {_deckId}");
        }

        private void OnSaveError(string error, string action) { SetBusy(false); SetStatus($"Erro ao {action} deck: {error}"); }

        public void ValidateDeck()
        {
            if (_isBusy) return;
            if (!Guid.TryParse(_deckId, out var deckGuid)) { SetStatus("Salve o deck antes de validar."); return; }
            SetBusy(true);
            GameManager.Instance.Api.ValidateDeck(this, deckGuid, response =>
            {
                SetBusy(false);
                if (response?.isValid == true) { SetStatus("Deck válido."); return; }
                var details = response?.errors != null && response.errors.Count > 0 ? string.Join(" | ", response.errors) : "sem detalhes";
                SetStatus($"Deck inválido: {details}");
            }, error => { SetBusy(false); SetStatus($"Erro ao validar deck: {error}"); });
        }

        public void ClearDeck()
        {
            if (_isBusy) return;
            _adventurerIds.Clear(); _enemyIds.Clear(); _roomIds.Clear(); _bossId = string.Empty; _deckId = string.Empty;
            PlayerPrefs.DeleteKey(DeckPrefKey); PlayerPrefs.Save();
            RenderAll();
            SetStatus("Deck limpo.");
        }

        private bool IsComplete(out string error)
        {
            if (_adventurerIds.Count != AdventurerMax) { error = $"Adventurers: {_adventurerIds.Count}/{AdventurerMax}"; return false; }
            if (_enemyIds.Count != EnemyMax) { error = $"Enemies: {_enemyIds.Count}/{EnemyMax}"; return false; }
            if (_roomIds.Count != RoomMax) { error = $"Rooms: {_roomIds.Count}/{RoomMax}"; return false; }
            if (string.IsNullOrWhiteSpace(_bossId)) { error = "Boss não selecionado."; return false; }
            error = null; return true;
        }

        private void UpdateCounters()
        {
            if (adventurerCountText != null) adventurerCountText.text = $"Adventurers: {_adventurerIds.Count}/{AdventurerMax}";
            if (enemyCountText != null) enemyCountText.text = $"Enemies: {_enemyIds.Count}/{EnemyMax}";
            if (roomCountText != null) roomCountText.text = $"Rooms: {_roomIds.Count}/{RoomMax}";
            if (bossCountText != null) bossCountText.text = $"Boss: {(string.IsNullOrWhiteSpace(_bossId) ? 0 : 1)}/1";
        }

        private void CreateRow(Transform parent, GameObject prefab, string text, string action, Action onClick)
        {
            var row = prefab != null ? Instantiate(prefab, parent) : CreateFallbackRow(parent);
            var label = row.GetComponentInChildren<TMP_Text>() ?? row.GetComponent<TMP_Text>();
            if (label != null) label.text = text;
            var button = row.GetComponentInChildren<Button>() ?? row.GetComponent<Button>();
            if (button == null) return;
            button.onClick.RemoveAllListeners();
            if (onClick != null) button.onClick.AddListener(() => onClick.Invoke());
            button.interactable = onClick != null && !_isBusy;
            var buttonText = button.GetComponentInChildren<TMP_Text>();
            if (buttonText != null) buttonText.text = action;
        }

        private static GameObject CreateFallbackRow(Transform parent)
        {
            var row = new GameObject("DeckRow", typeof(RectTransform), typeof(Image), typeof(HorizontalLayoutGroup));
            row.transform.SetParent(parent, false);
            row.GetComponent<Image>().color = new Color(0.14f, 0.11f, 0.08f, 0.7f);
            var layout = row.GetComponent<HorizontalLayoutGroup>();
            layout.spacing = 8;
            layout.padding = new RectOffset(8, 8, 4, 4);

            var textGo = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI), typeof(LayoutElement));
            textGo.transform.SetParent(row.transform, false);
            textGo.GetComponent<LayoutElement>().flexibleWidth = 1f;
            textGo.GetComponent<TextMeshProUGUI>().fontSize = 20;

            var buttonGo = new GameObject("Button", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
            buttonGo.transform.SetParent(row.transform, false);
            buttonGo.GetComponent<LayoutElement>().preferredWidth = 110;
            buttonGo.GetComponent<LayoutElement>().preferredHeight = 30;
            buttonGo.GetComponent<Image>().color = new Color(0.35f, 0.27f, 0.13f, 0.95f);
            var bTextGo = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            bTextGo.transform.SetParent(buttonGo.transform, false);
            var rect = bTextGo.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = Vector2.zero; rect.offsetMax = Vector2.zero;
            bTextGo.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
            bTextGo.GetComponent<TextMeshProUGUI>().fontSize = 18;
            return row;
        }

        private void SetBusy(bool busy)
        {
            _isBusy = busy;
            if (saveButton != null) saveButton.interactable = !busy;
            if (validateButton != null) validateButton.interactable = !busy;
            if (clearDeckButton != null) clearDeckButton.interactable = !busy;
            if (refreshCollectionButton != null) refreshCollectionButton.interactable = !busy;
        }

        private void SetStatus(string message)
        {
            if (statusText != null) statusText.text = message;
            Debug.Log($"[DeckBuilderUI] {message}");
        }
    }
}
