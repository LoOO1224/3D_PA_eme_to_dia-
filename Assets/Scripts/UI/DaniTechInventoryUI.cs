using UnityEngine;
using UnityEngine.UI;

namespace EmeToDia.Gameplay
{
    // 다이아 과제용 인벤토리 UI입니다.
    // 슬롯 선택, 사용, 버리기, 정렬 입력을 PromotionManager로 전달하고 표시만 갱신합니다.
    public sealed class DaniTechInventoryUI : MonoBehaviour
    {
        [Header("Root")]
        [SerializeField] private GameObject _rootObject;

        [Header("Text UI")]
        [SerializeField] private Text _titleText;
        [SerializeField] private Text _messageText;
        [SerializeField] private Text _selectedItemText;
        [SerializeField] private Text _detailText;

        [Header("Buttons")]
        [SerializeField] private Button _useButton;
        [SerializeField] private Button _dropButton;
        [SerializeField] private Button _sortNameButton;
        [SerializeField] private Button _sortTypeButton;
        [SerializeField] private Button _sortSequenceButton;
        [SerializeField] private Button _closeButton;

        [Header("Slot Views")]
        [SerializeField] private DaniTechInventorySlotView[] _slotViews;
        [SerializeField] private DaniTechInventoryDropTarget _dropTarget;

        private DaniTechPromotionManager _promotionManager;
        private DaniTechInventoryModel _inventoryModel;
        private DaniTechInventorySlotView _draggingSlotView;
        private bool _isOpen;
        private bool _isInitialized;

        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
        }

        private void OnDisable()
        {
            DisconnectModel();
        }

        public void Initialize(DaniTechPromotionManager promotionManager, DaniTechInventoryModel inventoryModel)
        {
            _promotionManager = promotionManager;
            ConnectModel(inventoryModel);
            ConnectButtons();
            ConnectSlotViews();
            RefreshUI();
            _isInitialized = true;
        }

        public void Open()
        {
            SetVisible(true);
            RefreshUI();
            SetCursorState(true);
        }

        public void Close()
        {
            _draggingSlotView = null;
            SetVisible(false);
            SetCursorState(false);
        }

        public void Toggle()
        {
            if (_isOpen)
            {
                Close();
                return;
            }

            Open();
        }

        public void SelectSlot(int slotIndex)
        {
            if (_promotionManager == null)
            {
                return;
            }

            _promotionManager.SelectInventorySlot(slotIndex);
        }

        public void BeginDragSlot(DaniTechInventorySlotView slotView)
        {
            _draggingSlotView = slotView;
            RefreshMessage("드롭 영역에 놓으면 선택 아이템을 1개 버립니다.");
        }

        public void EndDragSlot()
        {
            _draggingSlotView = null;
        }

        public void DropDraggingSlot()
        {
            if (_draggingSlotView == null)
            {
                return;
            }

            SelectSlot(_draggingSlotView.SlotIndex);
            if (_promotionManager != null)
            {
                _promotionManager.DropSelectedItem();
            }

            _draggingSlotView = null;
        }

        public void RefreshUI()
        {
            if (_inventoryModel == null || _promotionManager == null)
            {
                return;
            }

            RefreshTitle();
            RefreshSlots();
            RefreshSelectedDetail();
        }

        private void ConnectModel(DaniTechInventoryModel inventoryModel)
        {
            if (_inventoryModel == inventoryModel)
            {
                return;
            }

            DisconnectModel();
            _inventoryModel = inventoryModel;
            if (_inventoryModel != null)
            {
                _inventoryModel.OnInventoryChanged += RefreshUI;
            }
        }

        private void DisconnectModel()
        {
            if (_inventoryModel != null)
            {
                _inventoryModel.OnInventoryChanged -= RefreshUI;
            }

            _inventoryModel = null;
        }

        private void ConnectButtons()
        {
            if (_isInitialized)
            {
                return;
            }

            ConnectButton(_useButton, UseSelectedItem);
            ConnectButton(_dropButton, DropSelectedItem);
            ConnectButton(_sortNameButton, SortInventoryByName);
            ConnectButton(_sortTypeButton, SortInventoryByType);
            ConnectButton(_sortSequenceButton, SortInventoryBySequence);
            ConnectButton(_closeButton, Close);
        }

        private void ConnectSlotViews()
        {
            if (_slotViews != null)
            {
                for (int i = 0; i < _slotViews.Length; i++)
                {
                    if (_slotViews[i] == null)
                    {
                        continue;
                    }

                    _slotViews[i].Initialize(this);
                }
            }

            if (_dropTarget != null)
            {
                _dropTarget.Initialize(this);
            }
        }

        private void UseSelectedItem()
        {
            if (_promotionManager != null)
            {
                _promotionManager.UseSelectedItem();
            }
        }

        private void DropSelectedItem()
        {
            if (_promotionManager != null)
            {
                _promotionManager.DropSelectedItem();
            }
        }

        private void SortInventoryByName()
        {
            if (_promotionManager != null)
            {
                _promotionManager.SortInventoryByName();
            }
        }

        private void SortInventoryByType()
        {
            if (_promotionManager != null)
            {
                _promotionManager.SortInventoryByType();
            }
        }

        private void SortInventoryBySequence()
        {
            if (_promotionManager != null)
            {
                _promotionManager.SortInventoryByAcquiredSequence();
            }
        }

        private void RefreshTitle()
        {
            if (_titleText == null)
            {
                return;
            }

            _titleText.text = "Diamond Promotion Inventory";
        }

        private void RefreshSlots()
        {
            if (_slotViews == null)
            {
                return;
            }

            for (int i = 0; i < _slotViews.Length; i++)
            {
                DaniTechInventorySlotView slotView = _slotViews[i];
                if (slotView == null)
                {
                    continue;
                }

                DaniTechItemModel itemModel = _inventoryModel.GetItem(i);
                DaniTechItemData itemData = null;
                float cooldownRemainingSeconds = 0f;
                if (itemModel != null && itemModel.IsEmpty == false)
                {
                    itemData = _promotionManager.GetItemData(itemModel.ItemId);
                    cooldownRemainingSeconds = _promotionManager.GetCooldownRemainingSeconds(itemModel.ItemId);
                }

                bool isSelected = _inventoryModel.SelectedSlotIndex == i;
                slotView.SetSlot(i, itemModel, itemData, isSelected, cooldownRemainingSeconds);
            }
        }

        private void RefreshSelectedDetail()
        {
            DaniTechItemModel selectedItem = _inventoryModel.GetSelectedItem();
            if (selectedItem == null || selectedItem.IsEmpty)
            {
                SetSelectedText("선택 아이템 없음");
                SetDetailText("월드 아이템에 가까이 가서 E로 획득하세요.");
                RefreshMessage("Tab/I: 열기/닫기, T: 사용, G: 버리기, R: 정렬");
                return;
            }

            DaniTechItemData itemData = _promotionManager.GetItemData(selectedItem.ItemId);
            if (itemData == null)
            {
                SetSelectedText(selectedItem.ItemId);
                SetDetailText("아이템 데이터가 없습니다.");
                return;
            }

            float cooldownRemainingSeconds = _promotionManager.GetCooldownRemainingSeconds(selectedItem.ItemId);
            SetSelectedText(itemData.IconText + "  " + itemData.DisplayName + " x" + selectedItem.Amount);
            SetDetailText(
                itemData.ItemType + "\n" +
                itemData.Description + "\n" +
                "효과: " + itemData.EffectType + " " + itemData.EffectValue.ToString("0.##") + "\n" +
                "쿨타임: " + cooldownRemainingSeconds.ToString("0.0") + " / " + itemData.CooldownSeconds.ToString("0.0") + "초");
            RefreshMessage("선택 후 사용 버튼 또는 T 키로 상태 변화를 확인하세요.");
        }

        private void RefreshMessage(string message)
        {
            if (_messageText == null)
            {
                return;
            }

            _messageText.text = message;
        }

        private void SetSelectedText(string text)
        {
            if (_selectedItemText != null)
            {
                _selectedItemText.text = text;
            }
        }

        private void SetDetailText(string text)
        {
            if (_detailText != null)
            {
                _detailText.text = text;
            }
        }

        private void ConnectButton(Button button, UnityEngine.Events.UnityAction action)
        {
            if (button == null)
            {
                return;
            }

            button.onClick.RemoveListener(action);
            button.onClick.AddListener(action);
        }

        private void SetVisible(bool isVisible)
        {
            _isOpen = isVisible;
            if (_rootObject == null)
            {
                gameObject.SetActive(isVisible);
                return;
            }

            _rootObject.SetActive(isVisible);
        }

        private void SetCursorState(bool isVisible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
