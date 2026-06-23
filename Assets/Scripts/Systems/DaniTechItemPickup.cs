using UnityEngine;

namespace EmeToDia.Gameplay
{
    // 월드에 놓인 획득 가능 아이템 배우입니다.
    // 자신의 아이템 ID와 수량을 가지고, 상호작용 입력을 받으면 인벤토리 모델로 이동합니다.
    public sealed class DaniTechItemPickup : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _itemId;
        [SerializeField] private int _amount = 1;
        [SerializeField] private DaniTechFloatingItemView _itemView;

        private bool _isCollected;

        public string ItemId
        {
            get
            {
                return _itemId;
            }
        }

        public int Amount
        {
            get
            {
                return Mathf.Max(1, _amount);
            }
        }

        private void Awake()
        {
            ConnectViewIfNeeded();
            RefreshView();
        }

        public void SetItem(string itemId, int amount)
        {
            _itemId = itemId;
            _amount = Mathf.Max(1, amount);
            RefreshView();
        }

        public string GetInteractionText()
        {
            return "E 획득: " + GetDisplayName() + " x" + Amount;
        }

        public bool CanInteract(GameObject gameObjectInteractor)
        {
            return _isCollected == false && string.IsNullOrEmpty(_itemId) == false;
        }

        public void Interact(GameObject gameObjectInteractor)
        {
            if (CanInteract(gameObjectInteractor) == false)
            {
                return;
            }

            if (GameManager.Inst == null || GameManager.Inst.GetPromotionManager() == null)
            {
                return;
            }

            GameManager.Inst.GetPromotionManager().TryCollectItem(this);
        }

        public void Collect()
        {
            if (_isCollected)
            {
                return;
            }

            _isCollected = true;
            gameObject.SetActive(false);
            Destroy(gameObject, 0.1f);
        }

        private void ConnectViewIfNeeded()
        {
            if (_itemView != null)
            {
                return;
            }

            _itemView = GetComponent<DaniTechFloatingItemView>();
        }

        private void RefreshView()
        {
            ConnectViewIfNeeded();
            if (_itemView == null)
            {
                return;
            }

            string displayName = GetDisplayName();
            _itemView.SetLabel(displayName + " x" + Amount);
        }

        private string GetDisplayName()
        {
            string displayName = _itemId;
            if (GameManager.Inst != null && GameManager.Inst.GetGameDataManager() != null)
            {
                DaniTechItemData itemData = GameManager.Inst.GetGameDataManager().GetDaniTechItem(_itemId);
                if (itemData != null)
                {
                    displayName = itemData.DisplayName;
                }
            }

            return displayName;
        }
    }
}
