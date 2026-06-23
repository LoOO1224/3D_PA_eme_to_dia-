using UnityEngine;

namespace EmeToDia.Gameplay
{
    // 플레이어가 E 입력으로 상호작용할 수 있는 월드 오브젝트 규칙입니다.
    // 아이템 획득 로직은 아이템 오브젝트가 직접 처리하고, 플레이어는 인터페이스만 호출합니다.
    public interface IInteractable
    {
        bool CanInteract(GameObject gameObjectInteractor);

        void Interact(GameObject gameObjectInteractor);
    }
}
