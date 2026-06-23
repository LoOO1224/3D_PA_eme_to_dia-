using UnityEngine;

namespace EmeToDia.Gameplay
{
    // 1인칭 마우스 시점 회전을 담당하는 컴포넌트입니다.
    // 이동과 상호작용은 다른 컴포넌트가 담당합니다.
    public sealed class DaniTechPlayerView : MonoBehaviour
    {
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private float _mouseSensitivity = 240f;

        private float _xRotation;

        private void Start()
        {
            ResolveCameraIfNeeded();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                return;
            }

            RotateView();
        }

        private void ResolveCameraIfNeeded()
        {
            if (_cameraTransform != null)
            {
                return;
            }

            Camera camera = GetComponentInChildren<Camera>();
            if (camera != null)
            {
                _cameraTransform = camera.transform;
            }
        }

        private void RotateView()
        {
            if (_cameraTransform == null)
            {
                return;
            }

            float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -50f, 50f);
            _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }
    }
}
