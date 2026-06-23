using UnityEngine;

namespace EmeToDia.Gameplay
{
    // 다이아 과제 씬의 진행 흐름을 연결하는 매니저입니다.
    // 실제 획득, 표시, 사용, 버리기 역할은 각 전용 컴포넌트와 Model이 나눠 맡습니다.
    public sealed class DaniTechPromotionManager : MonoBehaviour, IGameInitializable
    {
        [SerializeField] private DaniTechInventoryUI _inventoryUI;
        [SerializeField] private DaniTechPlayerHUD _playerHUD;
        [SerializeField] private DaniTechFeedbackLogView _feedbackLogView;
        [SerializeField] private DaniTechAddressableItemSpawner _itemSpawner;

        private GameManager _gameManager;
        private int _sortModeIndex;
        private bool _isInitialized;

        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
        }

        private void Update()
        {
            if (_isInitialized == false)
            {
                return;
            }

            UpdatePlayerModel();
            ReadPromotionInput();
        }

        public void SetGameManager(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            ConnectViews();
            SpawnInitialItems();
            Log("E 키로 월드 아이템을 획득하고 Tab으로 인벤토리를 여세요.");
            _isInitialized = true;
        }

        public bool TryCollectItem(DaniTechItemPickup itemPickup)
        {
            if (itemPickup == null || _gameManager == null)
            {
                return false;
            }

            GameDataManager gameDataManager = _gameManager.GetGameDataManager();
            DaniTechItemData itemData = gameDataManager.GetDaniTechItem(itemPickup.ItemId);
            if (itemData == null)
            {
                Log("알 수 없는 아이템입니다: " + itemPickup.ItemId);
                return false;
            }

            int addedAmount;
            bool isAdded = _gameManager.DaniTechInventoryModel.TryAddItem(itemData, itemPickup.Amount, out addedAmount);
            if (isAdded == false)
            {
                Log("인벤토리 슬롯이 가득 차서 " + itemData.DisplayName + "을 획득하지 못했습니다.");
                return false;
            }

            itemPickup.Collect();
            Log(itemData.DisplayName + " x" + addedAmount + " 획득. 인벤토리에 추가되었습니다.");
            return true;
        }

        public void SelectInventorySlot(int slotIndex)
        {
            if (_gameManager == null)
            {
                return;
            }

            _gameManager.DaniTechInventoryModel.TrySelectSlot(slotIndex);
        }

        public void UseSelectedItem()
        {
            if (_gameManager == null)
            {
                return;
            }

            string message;
            bool isUsed = _gameManager.DaniTechInventoryModel.TryUseSelectedItem(
                _gameManager.DaniTechPlayerModel,
                _gameManager.GetGameDataManager(),
                Time.time,
                out message);

            Log(message);
            if (isUsed && _itemSpawner != null)
            {
                _itemSpawner.PlayUseEffect();
            }
        }

        public void DropSelectedItem()
        {
            if (_gameManager == null)
            {
                return;
            }

            string itemId;
            int amount;
            bool isDropped = _gameManager.DaniTechInventoryModel.TryDropSelectedItem(out itemId, out amount);
            if (isDropped == false)
            {
                Log("버릴 선택 아이템이 없습니다.");
                return;
            }

            DaniTechItemData itemData = _gameManager.GetGameDataManager().GetDaniTechItem(itemId);
            if (_itemSpawner != null)
            {
                _itemSpawner.SpawnDroppedItem(itemId, amount);
            }

            string displayName = itemData == null ? itemId : itemData.DisplayName;
            Log(displayName + " 1개를 버렸습니다. 수량 감소가 반영되었습니다.");
        }

        public void SortInventoryByName()
        {
            if (_gameManager == null)
            {
                return;
            }

            _gameManager.DaniTechInventoryModel.SortByName(_gameManager.GetGameDataManager());
            Log("인벤토리를 이름 기준으로 정렬했습니다.");
        }

        public void SortInventoryByType()
        {
            if (_gameManager == null)
            {
                return;
            }

            _gameManager.DaniTechInventoryModel.SortByType(_gameManager.GetGameDataManager());
            Log("인벤토리를 타입 기준으로 정렬했습니다.");
        }

        public void SortInventoryByAcquiredSequence()
        {
            if (_gameManager == null)
            {
                return;
            }

            _gameManager.DaniTechInventoryModel.SortByAcquiredSequence();
            Log("인벤토리를 획득 순서 기준으로 정렬했습니다.");
        }

        public DaniTechItemData GetItemData(string itemId)
        {
            if (_gameManager == null)
            {
                return null;
            }

            return _gameManager.GetGameDataManager().GetDaniTechItem(itemId);
        }

        public float GetCooldownRemainingSeconds(string itemId)
        {
            if (_gameManager == null)
            {
                return 0f;
            }

            return _gameManager.DaniTechInventoryModel.GetCooldownRemainingSeconds(itemId, Time.time);
        }

        private void ConnectViews()
        {
            if (_gameManager == null)
            {
                _gameManager = GameManager.Inst;
            }

            if (_inventoryUI != null)
            {
                _inventoryUI.Initialize(this, _gameManager.DaniTechInventoryModel);
                _inventoryUI.Close();
            }

            if (_playerHUD != null)
            {
                _playerHUD.Initialize(
                    _gameManager.DaniTechPlayerModel,
                    _gameManager.DaniTechInventoryModel,
                    _gameManager.GetGameDataManager());
            }

            if (_feedbackLogView != null)
            {
                _feedbackLogView.Clear();
            }

            if (_itemSpawner != null)
            {
                _itemSpawner.SetPromotionManager(this);
            }
        }

        private void SpawnInitialItems()
        {
            if (_itemSpawner == null)
            {
                return;
            }

            _itemSpawner.SpawnInitialItems();
        }

        private void UpdatePlayerModel()
        {
            if (_gameManager == null)
            {
                return;
            }

            _gameManager.DaniTechPlayerModel.UpdateStatus(Time.deltaTime);
        }

        private void ReadPromotionInput()
        {
            if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I))
            {
                if (_inventoryUI != null)
                {
                    _inventoryUI.Toggle();
                }
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                UseSelectedItem();
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                DropSelectedItem();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                SortByNextMode();
            }

            ReadSlotHotkeys();
        }

        private void ReadSlotHotkeys()
        {
            for (int i = 0; i < DaniTechInventoryModel.InventorySlotCount && i < 9; i++)
            {
                KeyCode keyCode = (KeyCode)((int)KeyCode.Alpha1 + i);
                if (Input.GetKeyDown(keyCode))
                {
                    SelectInventorySlot(i);
                }
            }
        }

        private void SortByNextMode()
        {
            _sortModeIndex++;
            if (_sortModeIndex > 2)
            {
                _sortModeIndex = 0;
            }

            if (_sortModeIndex == 0)
            {
                SortInventoryByName();
                return;
            }

            if (_sortModeIndex == 1)
            {
                SortInventoryByType();
                return;
            }

            SortInventoryByAcquiredSequence();
        }

        private void Log(string message)
        {
            if (_feedbackLogView != null)
            {
                _feedbackLogView.Log(message);
            }

            Debug.Log("[DaniTechPromotion] " + message);
        }
    }
}
