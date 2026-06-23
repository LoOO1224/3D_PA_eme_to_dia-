using System;
using System.Collections.Generic;
using UnityEngine;

namespace EmeToDia.Gameplay
{
    public enum DaniTechItemEffectType
    {
        None,
        Heal,
        Shield,
        SpeedBoost,
        Stamina
    }

    public enum DaniTechItemUseConditionType
    {
        None,
        HealthBelowMax,
        ShieldBelowMax
    }

    // 다이아 과제용 아이템 Static Data입니다.
    // JSON에는 고유 ID, 설명, Addressables 주소, 사용 조건과 효과를 저장합니다.
    [Serializable]
    public sealed class DaniTechItemData : GameDataBase
    {
        [SerializeField] private string _displayName;
        [SerializeField] private string _itemType;
        [SerializeField] private string _iconText;
        [SerializeField] private string _description;
        [SerializeField] private string _addressableKey;
        [SerializeField] private string _effectType;
        [SerializeField] private string _useConditionType;
        [SerializeField] private int _maxStackCount;
        [SerializeField] private float _effectValue;
        [SerializeField] private float _durationSeconds;
        [SerializeField] private float _cooldownSeconds;

        public string DisplayName
        {
            get
            {
                return _displayName;
            }
        }

        public string ItemType
        {
            get
            {
                return _itemType;
            }
        }

        public string IconText
        {
            get
            {
                return _iconText;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public string AddressableKey
        {
            get
            {
                return _addressableKey;
            }
        }

        public DaniTechItemEffectType EffectType
        {
            get
            {
                return ParseEffectType();
            }
        }

        public DaniTechItemUseConditionType UseConditionType
        {
            get
            {
                return ParseUseConditionType();
            }
        }

        public int MaxStackCount
        {
            get
            {
                return Mathf.Max(1, _maxStackCount);
            }
        }

        public float EffectValue
        {
            get
            {
                return _effectValue;
            }
        }

        public float DurationSeconds
        {
            get
            {
                return Mathf.Max(0f, _durationSeconds);
            }
        }

        public float CooldownSeconds
        {
            get
            {
                return Mathf.Max(0f, _cooldownSeconds);
            }
        }

        public bool HasCooldown
        {
            get
            {
                return CooldownSeconds > 0f;
            }
        }

        private DaniTechItemEffectType ParseEffectType()
        {
            DaniTechItemEffectType effectType;
            if (Enum.TryParse(_effectType, true, out effectType))
            {
                return effectType;
            }

            return DaniTechItemEffectType.None;
        }

        private DaniTechItemUseConditionType ParseUseConditionType()
        {
            DaniTechItemUseConditionType conditionType;
            if (Enum.TryParse(_useConditionType, true, out conditionType))
            {
                return conditionType;
            }

            return DaniTechItemUseConditionType.None;
        }
    }

    [Serializable]
    public sealed class DaniTechItemDataCatalog
    {
        public List<DaniTechItemData> Items = new List<DaniTechItemData>();
    }
}
