using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EmeToDia.Gameplay
{
    public enum DaniTechInventoryDropTargetAction
    {
        Drop,
        Use
    }

    // Dragged inventory slots tell the inventory UI which action area received them.
    public sealed class DaniTechInventoryDropTarget : MonoBehaviour, IDropHandler
    {
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Text _labelText;
        [SerializeField] private DaniTechInventoryDropTargetAction _targetAction;

        private DaniTechInventoryUI _inventoryUI;

        public void Initialize(DaniTechInventoryUI inventoryUI)
        {
            _inventoryUI = inventoryUI;
            RefreshView();
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (_inventoryUI == null)
            {
                return;
            }

            _inventoryUI.ApplyDraggingSlot(_targetAction);
        }

        private void RefreshView()
        {
            if (_backgroundImage != null)
            {
                _backgroundImage.color = GetTargetColor();
            }

            if (_labelText != null)
            {
                _labelText.text = GetTargetLabel();
            }
        }

        private Color GetTargetColor()
        {
            if (_targetAction == DaniTechInventoryDropTargetAction.Use)
            {
                return new Color(0.18f, 0.36f, 0.16f, 0.92f);
            }

            return new Color(0.36f, 0.12f, 0.12f, 0.9f);
        }

        private string GetTargetLabel()
        {
            if (_targetAction == DaniTechInventoryDropTargetAction.Use)
            {
                return "Drag To Use";
            }

            return "Drag To Drop";
        }
    }
}
