using UnityEngine;

namespace EmeToDia.Gameplay
{
    // 게임 전체 생성 주기를 잡는 최상위 매니저입니다.
    // 규칙: 게임 영역에서는 GameManager만 싱글톤으로 둡니다.
    public sealed class GameManager : MonoBehaviour
    {
        [SerializeField] private GameDataManager _gameDataManager;
        [SerializeField] private DaniTechPromotionManager _promotionManager;

        private DaniTechPlayerModel _daniTechPlayerModel = new DaniTechPlayerModel();
        private DaniTechInventoryModel _daniTechInventoryModel = new DaniTechInventoryModel();
        private bool _isInitialized;

        public static GameManager Inst { get; private set; }

        public DaniTechPlayerModel DaniTechPlayerModel
        {
            get
            {
                return _daniTechPlayerModel;
            }
        }

        public DaniTechInventoryModel DaniTechInventoryModel
        {
            get
            {
                return _daniTechInventoryModel;
            }
        }

        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
        }

        private void Awake()
        {
            RegisterSingleton();
        }

        private void Start()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            if (Inst == this)
            {
                Inst = null;
            }
        }

        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            InitializeManager(_gameDataManager, "GameDataManager");
            _daniTechPlayerModel.InitializeDefault();
            _daniTechInventoryModel.InitializeDefault();
            ConnectPromotionManager();
            InitializePromotionManagerIfExists();
            _isInitialized = true;
        }

        public GameDataManager GetGameDataManager()
        {
            return _gameDataManager;
        }

        public DaniTechPromotionManager GetPromotionManager()
        {
            return _promotionManager;
        }

        private void RegisterSingleton()
        {
            if (Inst == null)
            {
                Inst = this;
                return;
            }

            if (Inst == this)
            {
                return;
            }

            Debug.LogWarning("GameManager duplicate was found. Only the first GameManager will stay active.");
            Destroy(gameObject);
        }

        private void ConnectPromotionManager()
        {
            if (_promotionManager == null)
            {
                return;
            }

            _promotionManager.SetGameManager(this);
        }

        private void InitializePromotionManagerIfExists()
        {
            if (_promotionManager == null)
            {
                return;
            }

            _promotionManager.Initialize();
        }

        private void InitializeManager<TManager>(TManager manager, string managerName)
            where TManager : MonoBehaviour, IGameInitializable
        {
            if (manager == null)
            {
                Debug.LogWarning(managerName + " reference is empty. Assign it in the Inspector before gameplay work.");
                return;
            }

            manager.Initialize();
        }
    }
}
