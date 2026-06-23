using System;

namespace EmeToDia.Gameplay
{
    // 인벤토리 슬롯 안에서 변하는 아이템 Instance Data입니다.
    // Static Data의 설명과 효과는 itemId를 통해 GameDataManager에서 다시 조회합니다.
    [Serializable]
    public sealed class DaniTechItemModel
    {
        private string _itemId;
        private int _amount;
        private int _acquiredSequence;

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
                return _amount;
            }
        }

        public int AcquiredSequence
        {
            get
            {
                return _acquiredSequence;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return string.IsNullOrEmpty(_itemId) || _amount <= 0;
            }
        }

        public void SetItem(string itemId, int amount, int acquiredSequence)
        {
            _itemId = itemId;
            _amount = amount;
            _acquiredSequence = acquiredSequence;
        }

        public void AddAmount(int amount)
        {
            _amount += amount;
        }

        public void RemoveAmount(int amount)
        {
            _amount -= amount;
            if (_amount <= 0)
            {
                Clear();
            }
        }

        public void Clear()
        {
            _itemId = string.Empty;
            _amount = 0;
            _acquiredSequence = 0;
        }

        public DaniTechItemModel Clone()
        {
            DaniTechItemModel itemModel = new DaniTechItemModel();
            itemModel.SetItem(_itemId, _amount, _acquiredSequence);
            return itemModel;
        }
    }
}
