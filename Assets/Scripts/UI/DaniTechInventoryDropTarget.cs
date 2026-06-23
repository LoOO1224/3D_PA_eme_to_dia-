using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EmeToDia.Gameplay
{
    // 드래그한 아이템을 놓으면 버리기를 실행하는 DropTarget입니다.
    public sealed class DaniTechInventoryDropTarget : MonoBehaviour, IDropHandler
    {
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Text _labelText;

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

            _inventoryUI.DropDraggingSlot();
        }

        private void RefreshView()
        {
            if (_backgroundImage != null)
            {
                _backgroundImage.color = new Color(0.36f, 0.12f, 0.12f, 0.9f);
            }

            if (_labelText != null)
            {
                _labelText.text = "Drop / 버리기";
            }
        }
    }
}
