using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace EmeToDia.Gameplay
{
    // 월드 아이템 프리팹을 Addressables 주소로 생성하는 전용 스포너입니다.
    // 씬에는 SpawnPoint만 두고 실제 아이템 배우는 Addressables 프리팹에서 로드합니다.
    public sealed class DaniTechAddressableItemSpawner : MonoBehaviour
    {
        [SerializeField] private Transform _itemRoot;
        [SerializeField] private Transform _dropAnchor;
        [SerializeField] private DaniTechItemSpawnPoint[] _spawnPoints;
        [SerializeField] private GameObject _useEffectPrefab;

        private DaniTechPromotionManager _promotionManager;
        private bool _hasSpawnedInitialItems;

        public void SetPromotionManager(DaniTechPromotionManager promotionManager)
        {
            _promotionManager = promotionManager;
        }

        public void SpawnInitialItems()
        {
            if (_hasSpawnedInitialItems)
            {
                return;
            }

            _hasSpawnedInitialItems = true;
            ResolveSpawnPointsIfNeeded();

            for (int i = 0; i < _spawnPoints.Length; i++)
            {
                DaniTechItemSpawnPoint spawnPoint = _spawnPoints[i];
                if (spawnPoint == null)
                {
                    continue;
                }

                SpawnItem(spawnPoint.ItemId, spawnPoint.Amount, spawnPoint.transform.position, spawnPoint.transform.rotation);
            }
        }

        public void SpawnDroppedItem(string itemId, int amount)
        {
            Transform dropAnchor = GetDropAnchor();
            Vector3 position = dropAnchor.position + dropAnchor.forward * 1.6f + Vector3.up * 0.35f;
            SpawnItem(itemId, amount, position, Quaternion.identity);
        }

        public void PlayUseEffect()
        {
            if (_useEffectPrefab == null)
            {
                return;
            }

            Transform dropAnchor = GetDropAnchor();
            GameObject effectObject = Instantiate(_useEffectPrefab, dropAnchor.position + Vector3.up * 0.8f, Quaternion.identity);
            Destroy(effectObject, 2.5f);
        }

        private void SpawnItem(string itemId, int amount, Vector3 position, Quaternion rotation)
        {
            if (_promotionManager == null)
            {
                return;
            }

            DaniTechItemData itemData = _promotionManager.GetItemData(itemId);
            if (itemData == null)
            {
                Debug.LogWarning("DaniTechAddressableItemSpawner: item data was not found. " + itemId);
                return;
            }

            string addressableKey = itemData.AddressableKey;
            AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(addressableKey, position, rotation, _itemRoot);
            handle.Completed += operationHandle =>
            {
                if (operationHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    SetupSpawnedItem(operationHandle.Result, itemId, amount);
                    return;
                }

                Debug.LogWarning("DaniTechAddressableItemSpawner: Addressables load failed. " + addressableKey);
                CreateFallbackItem(itemId, amount, position, rotation);
            };
        }

        private void SetupSpawnedItem(GameObject itemObject, string itemId, int amount)
        {
            if (itemObject == null)
            {
                return;
            }

            DaniTechItemPickup itemPickup = itemObject.GetComponent<DaniTechItemPickup>();
            if (itemPickup == null)
            {
                itemPickup = itemObject.AddComponent<DaniTechItemPickup>();
            }

            itemPickup.SetItem(itemId, amount);
        }

        private void CreateFallbackItem(string itemId, int amount, Vector3 position, Quaternion rotation)
        {
            GameObject itemObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            itemObject.name = "Fallback_Item_" + itemId;
            itemObject.transform.SetParent(_itemRoot);
            itemObject.transform.position = position;
            itemObject.transform.rotation = rotation;
            itemObject.transform.localScale = new Vector3(0.7f, 0.9f, 0.7f);

            DaniTechItemPickup itemPickup = itemObject.AddComponent<DaniTechItemPickup>();
            itemObject.AddComponent<DaniTechFloatingItemView>();
            itemPickup.SetItem(itemId, amount);
        }

        private void ResolveSpawnPointsIfNeeded()
        {
            if (_spawnPoints != null && _spawnPoints.Length > 0)
            {
                return;
            }

            _spawnPoints = GetComponentsInChildren<DaniTechItemSpawnPoint>();
        }

        private Transform GetDropAnchor()
        {
            if (_dropAnchor != null)
            {
                return _dropAnchor;
            }

            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                _dropAnchor = playerObject.transform;
                return _dropAnchor;
            }

            return transform;
        }
    }
}
