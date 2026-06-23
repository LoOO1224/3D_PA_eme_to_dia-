using System;
using UnityEngine;

namespace EmeToDia.Gameplay
{
    // 다이아 과제 씬에서 저장해야 하는 플레이어 상태 Instance Data입니다.
    // 아이템 사용 결과가 이 모델에 누적되고 HUD는 이벤트로만 갱신합니다.
    [Serializable]
    public sealed class DaniTechPlayerModel
    {
        private const float MaxShield = 60f;
        private const float BaseMoveSpeedMultiplier = 1f;

        private float _health;
        private float _maxHealth;
        private float _stamina;
        private float _maxStamina;
        private float _shield;
        private float _speedBoostMultiplier;
        private float _speedBoostRemainingSeconds;

        public event Action OnStatusChanged;

        public float Health
        {
            get
            {
                return _health;
            }
        }

        public float MaxHealth
        {
            get
            {
                return _maxHealth;
            }
        }

        public float Stamina
        {
            get
            {
                return _stamina;
            }
        }

        public float MaxStamina
        {
            get
            {
                return _maxStamina;
            }
        }

        public float Shield
        {
            get
            {
                return _shield;
            }
        }

        public float SpeedBoostRemainingSeconds
        {
            get
            {
                return _speedBoostRemainingSeconds;
            }
        }

        public float MoveSpeedMultiplier
        {
            get
            {
                if (_speedBoostRemainingSeconds <= 0f)
                {
                    return BaseMoveSpeedMultiplier;
                }

                return Mathf.Max(BaseMoveSpeedMultiplier, _speedBoostMultiplier);
            }
        }

        public void InitializeDefault()
        {
            _maxHealth = 100f;
            _health = 62f;
            _maxStamina = 100f;
            _stamina = 70f;
            _shield = 0f;
            _speedBoostMultiplier = BaseMoveSpeedMultiplier;
            _speedBoostRemainingSeconds = 0f;
            NotifyStatusChanged();
        }

        public void UpdateStatus(float deltaTime)
        {
            if (_speedBoostRemainingSeconds <= 0f)
            {
                return;
            }

            _speedBoostRemainingSeconds -= deltaTime;
            if (_speedBoostRemainingSeconds <= 0f)
            {
                _speedBoostRemainingSeconds = 0f;
                _speedBoostMultiplier = BaseMoveSpeedMultiplier;
            }

            NotifyStatusChanged();
        }

        public bool CanUseItem(DaniTechItemData itemData, out string failMessage)
        {
            failMessage = string.Empty;
            if (itemData == null)
            {
                failMessage = "아이템 데이터가 비어 있습니다.";
                return false;
            }

            switch (itemData.UseConditionType)
            {
                case DaniTechItemUseConditionType.HealthBelowMax:
                    if (_health >= _maxHealth)
                    {
                        failMessage = "HP가 가득 차 있어 회복 아이템을 사용할 수 없습니다.";
                        return false;
                    }

                    break;
                case DaniTechItemUseConditionType.ShieldBelowMax:
                    if (_shield >= MaxShield)
                    {
                        failMessage = "보호막이 이미 최대치입니다.";
                        return false;
                    }

                    break;
            }

            return true;
        }

        public void ApplyItem(DaniTechItemData itemData)
        {
            if (itemData == null)
            {
                return;
            }

            switch (itemData.EffectType)
            {
                case DaniTechItemEffectType.Heal:
                    Heal(itemData.EffectValue);
                    break;
                case DaniTechItemEffectType.Shield:
                    AddShield(itemData.EffectValue);
                    break;
                case DaniTechItemEffectType.SpeedBoost:
                    BoostSpeed(itemData.EffectValue, itemData.DurationSeconds);
                    break;
                case DaniTechItemEffectType.Stamina:
                    RecoverStamina(itemData.EffectValue);
                    break;
            }

            NotifyStatusChanged();
        }

        public void Heal(float amount)
        {
            if (amount <= 0f)
            {
                return;
            }

            _health += amount;
            _health = Mathf.Min(_health, _maxHealth);
        }

        public void AddShield(float amount)
        {
            if (amount <= 0f)
            {
                return;
            }

            _shield += amount;
            _shield = Mathf.Min(_shield, MaxShield);
        }

        public void RecoverStamina(float amount)
        {
            if (amount <= 0f)
            {
                return;
            }

            _stamina += amount;
            _stamina = Mathf.Min(_stamina, _maxStamina);
        }

        public void BoostSpeed(float multiplier, float durationSeconds)
        {
            if (multiplier <= 1f || durationSeconds <= 0f)
            {
                return;
            }

            _speedBoostMultiplier = multiplier;
            _speedBoostRemainingSeconds = durationSeconds;
        }

        private void NotifyStatusChanged()
        {
            if (OnStatusChanged == null)
            {
                return;
            }

            OnStatusChanged.Invoke();
        }
    }
}
