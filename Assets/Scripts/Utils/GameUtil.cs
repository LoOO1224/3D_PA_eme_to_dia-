using UnityEngine;

namespace EmeToDia.Gameplay
{
    // 공용 계산과 데이터 로드 시작점을 모아두는 순수 도구 클래스입니다.
    // 오브젝트를 직접 찾지 않고, 필요한 값과 매니저를 파라미터로 받아 처리합니다.
    public static class GameUtil
    {
        public const string DaniTechItemDataResourcePath = "DaniTech/data/dani_items";

        public static void LoadFullData(GameDataManager gameDataManager)
        {
            if (gameDataManager == null)
            {
                Debug.LogWarning("GameUtil: GameDataManager reference is empty.");
                return;
            }

            gameDataManager.LoadDaniTechItemData(gameDataManager.GetDaniTechItemDataResourcePath());
        }

        public static float GetRate(float currentValue, float maxValue)
        {
            if (maxValue <= 0f)
            {
                return 0f;
            }

            return Mathf.Clamp01(currentValue / maxValue);
        }
    }
}
