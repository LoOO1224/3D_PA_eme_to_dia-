namespace EmeToDia.Gameplay
{
    // 모든 매니저가 같은 이름의 초기화 함수를 갖도록 맞추는 인터페이스입니다.
    // GameManager는 이 인터페이스만 보고 정해진 순서로 초기화합니다.
    public interface IGameInitializable
    {
        bool IsInitialized { get; }

        void Initialize();
    }
}
