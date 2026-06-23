using System;
using UnityEngine;

namespace EmeToDia.Gameplay
{
    // 변하지 않는 기획 데이터의 부모 클래스입니다.
    // 아이템처럼 JSON에서 읽는 Static Data는 이 클래스를 기준으로 확장합니다.
    [Serializable]
    public class GameDataBase
    {
        [SerializeField] private string _id;

        public string Id
        {
            get
            {
                return _id;
            }
        }
    }
}
