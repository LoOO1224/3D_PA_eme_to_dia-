using UnityEngine;

namespace EmeToDia.Gameplay
{
    // 어떤 아이템을 몇 개 생성할지 보관하는 월드 스폰 포인트입니다.
    public sealed class DaniTechItemSpawnPoint : MonoBehaviour
    {
        [SerializeField] private string _itemId;
        [SerializeField] private int _amount = 1;

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
    }
}
