using UnityEngine;
using UnityEngine.UI;

namespace EmeToDia.Gameplay
{
    // 플레이어 상태와 선택 아이템을 표시하는 HUD View입니다.
    public sealed class DaniTechPlayerHUD : MonoBehaviour
    {
        [SerializeField] private Text _statusText;
        [SerializeField] private Text _guideText;
        [SerializeField] private Image _healthFillImage;
        [SerializeField] private Image _staminaFillImage;
        [SerializeField] private Image _shieldFillImage;

        private DaniTechPlayerModel _playerModel;
        private DaniTechInventoryModel _inventoryModel;
        private GameDataManager _gameDataManager;

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

        public void RefreshUI()
        {
            if (_playerModel == null || _inventoryModel == null)
            {
                return;
            }

            RefreshBars();
            RefreshTexts();
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
            string selectedItemText = "선택 없음";
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
                    "HP " + _playerModel.Health.ToString("0") + " / " + _playerModel.MaxHealth.ToString("0") + "\n" +
                    "Stamina " + _playerModel.Stamina.ToString("0") + " / " + _playerModel.MaxStamina.ToString("0") + "\n" +
                    "Shield " + _playerModel.Shield.ToString("0") + " / 60\n" +
                    "Speed x" + _playerModel.MoveSpeedMultiplier.ToString("0.00") + " (" + _playerModel.SpeedBoostRemainingSeconds.ToString("0.0") + "s)\n" +
                    "Selected: " + selectedItemText;
            }

            if (_guideText != null)
            {
                _guideText.text = "WASD 이동 / 우클릭 드래그 시점 / 휠 줌 / E 획득 / Tab 인벤토리 / T 사용 / G 버리기";
            }
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
