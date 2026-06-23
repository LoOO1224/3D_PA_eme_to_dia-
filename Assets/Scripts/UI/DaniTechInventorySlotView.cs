using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EmeToDia.Gameplay
{
    // 인벤토리 한 칸을 표시하는 View 컴포넌트입니다.
    // 선택과 드래그 시작만 기록하고 실제 사용/버리기는 InventoryUI가 연결합니다.
    public sealed class DaniTechInventorySlotView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Text _iconText;
        [SerializeField] private Text _nameText;
        [SerializeField] private Text _amountText;
        [SerializeField] private Text _cooldownText;

        private DaniTechInventoryUI _inventoryUI;
        private int _slotIndex;
        private bool _isInitialized;

        public int SlotIndex
        {
            get
            {
                return _slotIndex;
            }
        }

        public void Initialize(DaniTechInventoryUI inventoryUI)
        {
            _inventoryUI = inventoryUI;
            if (_isInitialized)
            {
                return;
            }

            if (_button != null)
            {
                _button.onClick.AddListener(SelectSlot);
            }

            _isInitialized = true;
        }

        public void SetSlot(
            int slotIndex,
            DaniTechItemModel itemModel,
            DaniTechItemData itemData,
            bool isSelected,
            float cooldownRemainingSeconds)
        {
            _slotIndex = slotIndex;
            if (itemModel == null || itemModel.IsEmpty || itemData == null)
            {
                SetText(_iconText, "");
                SetText(_nameText, "Empty");
                SetText(_amountText, "");
                SetText(_cooldownText, "");
                RefreshBackground(false, isSelected);
                return;
            }

            SetText(_iconText, itemData.IconText);
            SetText(_nameText, itemData.DisplayName);
            SetText(_amountText, "x" + itemModel.Amount);
            SetText(_cooldownText, cooldownRemainingSeconds > 0f ? cooldownRemainingSeconds.ToString("0.0") + "s" : "");
            RefreshBackground(true, isSelected);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_inventoryUI == null)
            {
                return;
            }

            _inventoryUI.BeginDragSlot(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_inventoryUI == null)
            {
                return;
            }

            _inventoryUI.EndDragSlot();
        }

        private void SelectSlot()
        {
            if (_inventoryUI == null)
            {
                return;
            }

            _inventoryUI.SelectSlot(_slotIndex);
        }

        private void RefreshBackground(bool hasItem, bool isSelected)
        {
            if (_backgroundImage == null)
            {
                return;
            }

            if (isSelected)
            {
                _backgroundImage.color = new Color(0.48f, 0.30f, 0.12f, 0.98f);
                return;
            }

            if (hasItem)
            {
                _backgroundImage.color = new Color(0.28f, 0.18f, 0.09f, 0.96f);
                return;
            }

            _backgroundImage.color = new Color(0.15f, 0.09f, 0.05f, 0.84f);
        }

        private void SetText(Text targetText, string value)
        {
            if (targetText == null)
            {
                return;
            }

            targetText.text = value;
        }
    }
}
