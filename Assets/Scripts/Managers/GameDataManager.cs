using System.Collections.Generic;
using UnityEngine;

namespace EmeToDia.Gameplay
{
    // 변하지 않는 기획 데이터를 JSON에서 읽어 보관하는 매니저입니다.
    // 다이아 과제에서는 아이템 Static Data만 다루고, 진행 중 변하는 값은 Model에 둡니다.
    public sealed class GameDataManager : MonoBehaviour, IGameInitializable
    {
        [SerializeField] private string _daniTechItemDataResourcePath = GameUtil.DaniTechItemDataResourcePath;

        private readonly Dictionary<string, DaniTechItemData> _daniTechItemDataList = new Dictionary<string, DaniTechItemData>();
        private bool _isInitialized;

        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
        }

        public IReadOnlyDictionary<string, DaniTechItemData> DaniTechItemDataList
        {
            get
            {
                return _daniTechItemDataList;
            }
        }

        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            GameUtil.LoadFullData(this);
            _isInitialized = true;
        }

        public void LoadDaniTechItemData(string resourcePath)
        {
            _daniTechItemDataList.Clear();

            TextAsset jsonAsset = Resources.Load<TextAsset>(resourcePath);
            if (jsonAsset == null)
            {
                Debug.LogWarning("GameDataManager: DaniTech item JSON was not found at Resources/" + resourcePath + ".json");
                return;
            }

            DaniTechItemDataCatalog catalog = JsonUtility.FromJson<DaniTechItemDataCatalog>(jsonAsset.text);
            if (catalog == null || catalog.Items == null)
            {
                Debug.LogWarning("GameDataManager: DaniTech item JSON format is invalid.");
                return;
            }

            for (int i = 0; i < catalog.Items.Count; i++)
            {
                AddData(_daniTechItemDataList, catalog.Items[i]);
            }
        }

        public DaniTechItemData GetDaniTechItem(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            if (_daniTechItemDataList.ContainsKey(id) == false)
            {
                return null;
            }

            return _daniTechItemDataList[id];
        }

        public string GetDaniTechItemDataResourcePath()
        {
            return _daniTechItemDataResourcePath;
        }

        private void AddData<TData>(Dictionary<string, TData> target, TData data)
            where TData : GameDataBase
        {
            if (target == null || data == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(data.Id))
            {
                return;
            }

            if (target.ContainsKey(data.Id))
            {
                target[data.Id] = data;
                return;
            }

            target.Add(data.Id, data);
        }
    }
}
