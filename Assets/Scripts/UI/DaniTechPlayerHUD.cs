using UnityEngine;
using UnityEngine.UI;

namespace EmeToDia.Gameplay
{
    // 플레이어 상태와 선택 아이템을 표시하는 HUD View입니다.
    public sealed class DaniTechPlayerHUD : MonoBehaviour
    {
        [SerializeField] private Text _statusText;
        [SerializeField] private Text _guideText;
        [SerializeField] private Text _healthText;
        [SerializeField] private Text _staminaText;
        [SerializeField] private Text _shieldText;
        [SerializeField] private Text _tooltipText;
        [SerializeField] private Text _inventorySummaryText;
        [SerializeField] private Image _healthFillImage;
        [SerializeField] private Image _staminaFillImage;
        [SerializeField] private Image _shieldFillImage;

        private DaniTechPlayerModel _playerModel;
        private DaniTechInventoryModel _inventoryModel;
        private GameDataManager _gameDataManager;
        private string _interactionHintText;

        private void OnDestroy()
        {
            DisconnectModels();
        }

        public void Initialize(
            DaniTechPlayerModel playerModel,
            DaniTechInventoryModel inventoryModel,
            GameDataManager gameDataManager)
        {
            DisconnectModels();
            _playerModel = playerModel;
            _inventoryModel = inventoryModel;
            _gameDataManager = gameDataManager;
            _interactionHintText = string.Empty;

            if (_playerModel != null)
            {
                _playerModel.OnStatusChanged += RefreshUI;
            }

            if (_inventoryModel != null)
            {
                _inventoryModel.OnInventoryChanged += RefreshUI;
            }

            RefreshUI();
        }

        public void SetTooltip(string tooltipText)
        {
            _interactionHintText = tooltipText;
            RefreshTooltipText();
        }

        public void RefreshUI()
        {
            if (_playerModel == null || _inventoryModel == null)
            {
                return;
            }

            RefreshBars();
            RefreshTexts();
            RefreshInventorySummary();
        }

        private void RefreshBars()
        {
            SetFill(_healthFillImage, GameUtil.GetRate(_playerModel.Health, _playerModel.MaxHealth));
            SetFill(_staminaFillImage, GameUtil.GetRate(_playerModel.Stamina, _playerModel.MaxStamina));
            SetFill(_shieldFillImage, GameUtil.GetRate(_playerModel.Shield, 60f));
        }

        private void RefreshTexts()
        {
            DaniTechItemModel selectedItem = _inventoryModel.GetSelectedItem();
            string selectedItemText = "None";
            if (selectedItem != null && selectedItem.IsEmpty == false && _gameDataManager != null)
            {
                DaniTechItemData itemData = _gameDataManager.GetDaniTechItem(selectedItem.ItemId);
                if (itemData != null)
                {
                    selectedItemText = itemData.DisplayName + " x" + selectedItem.Amount;
                }
            }

            if (_statusText != null)
            {
                _statusText.text =
                    "Selected: " + selectedItemText + "\n" +
                    "Speed x" + _playerModel.MoveSpeedMultiplier.ToString("0.00") +
                    " (" + _playerModel.SpeedBoostRemainingSeconds.ToString("0.0") + "s)";
            }

            if (_healthText != null)
            {
                _healthText.text = "HP " + _playerModel.Health.ToString("0") + " / " + _playerModel.MaxHealth.ToString("0");
            }

            if (_staminaText != null)
            {
                _staminaText.text = "STM " + _playerModel.Stamina.ToString("0") + " / " + _playerModel.MaxStamina.ToString("0");
            }

            if (_shieldText != null)
            {
                _shieldText.text = "SHD " + _playerModel.Shield.ToString("0") + " / 60";
            }

            if (_guideText != null)
            {
                _guideText.text = "Right drag: look  /  Q,C: turn  /  Wheel: zoom  /  Tab/I: bag";
            }

            RefreshTooltipText();
        }

        private void RefreshInventorySummary()
        {
            if (_inventorySummaryText == null || _inventoryModel == null)
            {
                return;
            }

            int ownedCount = 0;
            string summaryText = "BAG  Tab/I\n";
            for (int i = 0; i < DaniTechInventoryModel.InventorySlotCount; i++)
            {
                DaniTechItemModel itemModel = _inventoryModel.GetItem(i);
                if (itemModel == null || itemModel.IsEmpty)
                {
                    continue;
                }

                ownedCount++;
                if (ownedCount <= 4)
                {
                    summaryText += ownedCount.ToString() + ". " + GetInventoryDisplayText(itemModel) + "\n";
                }
            }

            if (ownedCount <= 0)
            {
                _inventorySummaryText.text = "BAG  Tab/I\nEmpty\nPick up items with E";
                return;
            }

            if (ownedCount > 4)
            {
                summaryText += "+ " + (ownedCount - 4).ToString() + " more";
            }

            _inventorySummaryText.text = summaryText.TrimEnd();
        }

        private string GetInventoryDisplayText(DaniTechItemModel itemModel)
        {
            if (_gameDataManager == null)
            {
                return itemModel.ItemId + " x" + itemModel.Amount.ToString();
            }

            DaniTechItemData itemData = _gameDataManager.GetDaniTechItem(itemModel.ItemId);
            if (itemData == null)
            {
                return itemModel.ItemId + " x" + itemModel.Amount.ToString();
            }

            return itemData.IconText + " " + itemData.DisplayName + " x" + itemModel.Amount.ToString();
        }

        private void RefreshTooltipText()
        {
            if (_tooltipText == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(_interactionHintText))
            {
                _tooltipText.text = "Tip: Aim at an item and press E to pick it up.";
                return;
            }

            _tooltipText.text = _interactionHintText;
        }

        private void DisconnectModels()
        {
            if (_playerModel != null)
            {
                _playerModel.OnStatusChanged -= RefreshUI;
            }

            if (_inventoryModel != null)
            {
                _inventoryModel.OnInventoryChanged -= RefreshUI;
            }

            _playerModel = null;
            _inventoryModel = null;
            _gameDataManager = null;
        }

        private void SetFill(Image image, float fillAmount)
        {
            if (image == null)
            {
                return;
            }

            image.fillAmount = fillAmount;
        }
    }
}
