using UnityEngine;

namespace EmeToDia.Gameplay
{
    // 플레이어 WASD 이동과 중력 처리를 담당합니다.
    // 아이템 사용으로 생기는 속도 증가는 DaniTechPlayerModel에서 읽어 적용합니다.
    public sealed class DaniTechPlayerMovement : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _runSpeed = 8f;
        [SerializeField] private float _gravity = -20f;
        [SerializeField] private float _groundStickVelocity = -2f;

        private CharacterController _characterController;
        private float _verticalVelocity;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            if (_characterController == null)
            {
                return;
            }

            Vector3 moveDirection = GetMoveDirection();
            float speed = GetCurrentSpeed();
            Vector3 horizontalVelocity = moveDirection * speed;
            Vector3 velocity = horizontalVelocity + Vector3.up * _verticalVelocity;

            _characterController.Move(velocity * Time.deltaTime);
            ApplyGravity();
        }

        private Vector3 GetMoveDirection()
        {
            float horizontal = 0f;
            float vertical = 0f;

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                horizontal -= 1f;
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                horizontal += 1f;
            }

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                vertical -= 1f;
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                vertical += 1f;
            }

            Vector3 direction = transform.right * horizontal + transform.forward * vertical;
            return direction.normalized;
        }

        private float GetCurrentSpeed()
        {
            float baseSpeed = Input.GetKey(KeyCode.LeftShift) ? _runSpeed : _moveSpeed;
            if (GameManager.Inst == null)
            {
                return baseSpeed;
            }

            return baseSpeed * GameManager.Inst.DaniTechPlayerModel.MoveSpeedMultiplier;
        }

        private void ApplyGravity()
        {
            if (_characterController.isGrounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = _groundStickVelocity;
                return;
            }

            _verticalVelocity += _gravity * Time.deltaTime;
        }
    }
}
