using System;
using System.Collections.Generic;

namespace EmeToDia.Gameplay
{
    // 다이아 과제 씬 인벤토리 Instance Data입니다.
    // 슬롯 저장, 선택, 정렬, 사용, 버리기 규칙만 담당하고 UI를 직접 알지 않습니다.
    [Serializable]
    public sealed class DaniTechInventoryModel
    {
        public const int InventorySlotCount = 12;

        private DaniTechItemModel[] _inventorySlots = new DaniTechItemModel[InventorySlotCount];
        private readonly Dictionary<string, float> _cooldownEndTimeByItemId = new Dictionary<string, float>();
        private int _selectedSlotIndex = -1;
        private int _acquiredSequenceGenerator;

        public event Action OnInventoryChanged;

        public int SelectedSlotIndex
        {
            get
            {
                return _selectedSlotIndex;
            }
        }

        public void InitializeDefault()
        {
            InitializeSlotArray();
            _cooldownEndTimeByItemId.Clear();
            _selectedSlotIndex = -1;
            _acquiredSequenceGenerator = 0;
            NotifyInventoryChanged();
        }

        public DaniTechItemModel GetItem(int slotIndex)
        {
            if (IsSlotIndexValid(slotIndex) == false)
            {
                return null;
            }

            return _inventorySlots[slotIndex];
        }

        public DaniTechItemModel GetSelectedItem()
        {
            return GetItem(_selectedSlotIndex);
        }

        public float GetCooldownRemainingSeconds(string itemId, float currentTime)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                return 0f;
            }

            if (_cooldownEndTimeByItemId.ContainsKey(itemId) == false)
            {
                return 0f;
            }

            return Math.Max(0f, _cooldownEndTimeByItemId[itemId] - currentTime);
        }

        public bool TrySelectSlot(int slotIndex)
        {
            if (IsSlotIndexValid(slotIndex) == false)
            {
                return false;
            }

            _selectedSlotIndex = slotIndex;
            NotifyInventoryChanged();
            return true;
        }

        public bool TryAddItem(DaniTechItemData itemData, int amount, out int addedAmount)
        {
            addedAmount = 0;
            if (itemData == null || amount <= 0)
            {
                return false;
            }

            int remainingAmount = amount;
            AddToExistingStacks(itemData, ref remainingAmount, ref addedAmount);
            AddToEmptySlots(itemData, ref remainingAmount, ref addedAmount);

            if (addedAmount <= 0)
            {
                return false;
            }

            if (_selectedSlotIndex < 0)
            {
                SelectFirstOwnedSlot();
            }

            NotifyInventoryChanged();
            return true;
        }

        public bool TryUseSelectedItem(
            DaniTechPlayerModel playerModel,
            GameDataManager gameDataManager,
            float currentTime,
            out string message)
        {
            message = string.Empty;
            DaniTechItemModel selectedItem = GetSelectedItem();
            if (selectedItem == null || selectedItem.IsEmpty)
            {
                message = "선택된 아이템이 없습니다.";
                return false;
            }

            DaniTechItemData itemData = gameDataManager.GetDaniTechItem(selectedItem.ItemId);
            if (itemData == null)
            {
                message = "아이템 데이터가 없어 사용할 수 없습니다: " + selectedItem.ItemId;
                return false;
            }

            float cooldownRemainingSeconds = GetCooldownRemainingSeconds(selectedItem.ItemId, currentTime);
            if (cooldownRemainingSeconds > 0f)
            {
                message = itemData.DisplayName + " 쿨타임이 " + cooldownRemainingSeconds.ToString("0.0") + "초 남았습니다.";
                return false;
            }

            string failMessage;
            if (playerModel.CanUseItem(itemData, out failMessage) == false)
            {
                message = failMessage;
                return false;
            }

            playerModel.ApplyItem(itemData);
            selectedItem.RemoveAmount(1);
            if (itemData.HasCooldown)
            {
                _cooldownEndTimeByItemId[itemData.Id] = currentTime + itemData.CooldownSeconds;
            }

            message = itemData.DisplayName + " 사용 완료. 상태 변화가 즉시 반영되었습니다.";
            SelectFirstOwnedSlotIfSelectedEmpty();
            NotifyInventoryChanged();
            return true;
        }

        public bool TryDropSelectedItem(out string itemId, out int amount)
        {
            itemId = string.Empty;
            amount = 0;

            DaniTechItemModel selectedItem = GetSelectedItem();
            if (selectedItem == null || selectedItem.IsEmpty)
            {
                return false;
            }

            itemId = selectedItem.ItemId;
            amount = 1;
            selectedItem.RemoveAmount(1);
            SelectFirstOwnedSlotIfSelectedEmpty();
            NotifyInventoryChanged();
            return true;
        }

        public void SortByName(GameDataManager gameDataManager)
        {
            SortItems((left, right) =>
            {
                string leftName = GetSortName(gameDataManager, left);
                string rightName = GetSortName(gameDataManager, right);
                return string.Compare(leftName, rightName, StringComparison.Ordinal);
            });
        }

        public void SortByType(GameDataManager gameDataManager)
        {
            SortItems((left, right) =>
            {
                string leftType = GetSortType(gameDataManager, left);
                string rightType = GetSortType(gameDataManager, right);
                int typeResult = string.Compare(leftType, rightType, StringComparison.Ordinal);
                if (typeResult != 0)
                {
                    return typeResult;
                }

                return string.Compare(GetSortName(gameDataManager, left), GetSortName(gameDataManager, right), StringComparison.Ordinal);
            });
        }

        public void SortByAcquiredSequence()
        {
            SortItems((left, right) => left.AcquiredSequence.CompareTo(right.AcquiredSequence));
        }

        private void AddToExistingStacks(DaniTechItemData itemData, ref int remainingAmount, ref int addedAmount)
        {
            for (int i = 0; i < _inventorySlots.Length; i++)
            {
                DaniTechItemModel slotItem = _inventorySlots[i];
                if (slotItem == null || slotItem.IsEmpty)
                {
                    continue;
                }

                if (slotItem.ItemId != itemData.Id)
                {
                    continue;
                }

                int emptyCount = itemData.MaxStackCount - slotItem.Amount;
                int addCount = Math.Min(emptyCount, remainingAmount);
                if (addCount <= 0)
                {
                    continue;
                }

                slotItem.AddAmount(addCount);
                remainingAmount -= addCount;
                addedAmount += addCount;

                if (remainingAmount <= 0)
                {
                    return;
                }
            }
        }

        private void AddToEmptySlots(DaniTechItemData itemData, ref int remainingAmount, ref int addedAmount)
        {
            for (int i = 0; i < _inventorySlots.Length; i++)
            {
                DaniTechItemModel slotItem = _inventorySlots[i];
                if (slotItem == null || slotItem.IsEmpty == false)
                {
                    continue;
                }

                int addCount = Math.Min(itemData.MaxStackCount, remainingAmount);
                _acquiredSequenceGenerator++;
                slotItem.SetItem(itemData.Id, addCount, _acquiredSequenceGenerator);
                remainingAmount -= addCount;
                addedAmount += addCount;

                if (remainingAmount <= 0)
                {
                    return;
                }
            }
        }

        private void SortItems(Comparison<DaniTechItemModel> comparison)
        {
            List<DaniTechItemModel> ownedItems = new List<DaniTechItemModel>();
            for (int i = 0; i < _inventorySlots.Length; i++)
            {
                DaniTechItemModel itemModel = _inventorySlots[i];
                if (itemModel == null || itemModel.IsEmpty)
                {
                    continue;
                }

                ownedItems.Add(itemModel.Clone());
            }

            ownedItems.Sort(comparison);
            InitializeSlotArray();
            for (int i = 0; i < ownedItems.Count && i < _inventorySlots.Length; i++)
            {
                _inventorySlots[i].SetItem(ownedItems[i].ItemId, ownedItems[i].Amount, ownedItems[i].AcquiredSequence);
            }

            SelectFirstOwnedSlot();
            NotifyInventoryChanged();
        }

        private string GetSortName(GameDataManager gameDataManager, DaniTechItemModel itemModel)
        {
            DaniTechItemData itemData = gameDataManager.GetDaniTechItem(itemModel.ItemId);
            if (itemData == null)
            {
                return itemModel.ItemId;
            }

            return itemData.DisplayName;
        }

        private string GetSortType(GameDataManager gameDataManager, DaniTechItemModel itemModel)
        {
            DaniTechItemData itemData = gameDataManager.GetDaniTechItem(itemModel.ItemId);
            if (itemData == null)
            {
                return string.Empty;
            }

            return itemData.ItemType;
        }

        private void SelectFirstOwnedSlotIfSelectedEmpty()
        {
            DaniTechItemModel selectedItem = GetSelectedItem();
            if (selectedItem != null && selectedItem.IsEmpty == false)
            {
                return;
            }

            SelectFirstOwnedSlot();
        }

        private void SelectFirstOwnedSlot()
        {
            _selectedSlotIndex = -1;
            for (int i = 0; i < _inventorySlots.Length; i++)
            {
                DaniTechItemModel itemModel = _inventorySlots[i];
                if (itemModel == null || itemModel.IsEmpty)
                {
                    continue;
                }

                _selectedSlotIndex = i;
                return;
            }
        }

        private void InitializeSlotArray()
        {
            for (int i = 0; i < _inventorySlots.Length; i++)
            {
                if (_inventorySlots[i] == null)
                {
                    _inventorySlots[i] = new DaniTechItemModel();
                }

                _inventorySlots[i].Clear();
            }
        }

        private bool IsSlotIndexValid(int slotIndex)
        {
            return slotIndex >= 0 && slotIndex < InventorySlotCount;
        }

        private void NotifyInventoryChanged()
        {
            if (OnInventoryChanged == null)
            {
                return;
            }

            OnInventoryChanged.Invoke();
        }
    }
}
