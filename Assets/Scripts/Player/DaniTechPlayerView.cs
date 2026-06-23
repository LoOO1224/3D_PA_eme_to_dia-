using UnityEngine;

namespace EmeToDia.Gameplay
{
    // 1인칭과 어깨 뒤 줌 카메라 전환을 담당하는 컴포넌트입니다.
    // 이동과 상호작용은 다른 컴포넌트가 담당합니다.
    public sealed class DaniTechPlayerView : MonoBehaviour
    {
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private Transform _characterVisualRoot;
        [SerializeField] private float _mouseSensitivity = 0.18f;
        [SerializeField] private float _keyboardRotateSpeed = 95f;
        [SerializeField] private float _zoomSpeed = 4f;
        [SerializeField] private float _maxZoomDistance = 7f;
        [SerializeField] private float _firstPersonHideDistance = 0.25f;
        [SerializeField] private Vector3 _firstPersonCameraPosition = new Vector3(0f, 1.58f, 0f);
        [SerializeField] private Vector3 _thirdPersonCameraOffset = new Vector3(0f, 2.45f, -7f);

        private float _xRotation;
        private float _zoomDistance;
        private Vector3 _lastMousePosition;
        private bool _isMouseRotating;

        private void Start()
        {
            ResolveCameraIfNeeded();
            SetCursorVisible();
            RefreshCameraPose();
            RefreshCharacterVisibility();
        }

        private void Update()
        {
            SetCursorVisible();
            ZoomCamera();
            RotateView();
            RefreshCameraPose();
            RefreshCharacterVisibility();
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

        private void SetCursorVisible()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void ZoomCamera()
        {
            float scrollValue = Input.mouseScrollDelta.y;
            if (Mathf.Abs(scrollValue) <= 0.001f)
            {
                return;
            }

            _zoomDistance -= scrollValue * _zoomSpeed;
            _zoomDistance = Mathf.Clamp(_zoomDistance, 0f, _maxZoomDistance);
        }

        private void RotateView()
        {
            if (_cameraTransform == null)
            {
                return;
            }

            RotateByMouseDrag();
            RotateByKeyboard();
        }

        private void RotateByMouseDrag()
        {
            if (Input.GetMouseButtonDown(1))
            {
                _isMouseRotating = true;
                _lastMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(1))
            {
                _isMouseRotating = false;
            }

            if (_isMouseRotating == false)
            {
                return;
            }

            Vector3 mouseDelta = Input.mousePosition - _lastMousePosition;
            _lastMousePosition = Input.mousePosition;
            ApplyLook(mouseDelta.x * _mouseSensitivity, mouseDelta.y * _mouseSensitivity);
        }

        private void RotateByKeyboard()
        {
            float yaw = 0f;
            if (Input.GetKey(KeyCode.Q))
            {
                yaw -= _keyboardRotateSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.C))
            {
                yaw += _keyboardRotateSpeed * Time.deltaTime;
            }

            if (Mathf.Abs(yaw) > 0.001f)
            {
                ApplyLook(yaw, 0f);
            }
        }

        private void ApplyLook(float yawDelta, float pitchDelta)
        {
            _xRotation -= pitchDelta;
            _xRotation = Mathf.Clamp(_xRotation, -50f, 50f);
            transform.Rotate(Vector3.up * yawDelta);
        }

        private void RefreshCameraPose()
        {
            if (_cameraTransform == null)
            {
                return;
            }

            float zoomRate = GameUtil.GetRate(_zoomDistance, _maxZoomDistance);
            _cameraTransform.localPosition = Vector3.Lerp(_firstPersonCameraPosition, _thirdPersonCameraOffset, zoomRate);
            _cameraTransform.localRotation = Quaternion.Euler(_xRotation + zoomRate * 10f, 0f, 0f);
        }

        private void RefreshCharacterVisibility()
        {
            if (_characterVisualRoot == null)
            {
                return;
            }

            bool isVisible = _zoomDistance > _firstPersonHideDistance;
            if (_characterVisualRoot.gameObject.activeSelf == isVisible)
            {
                return;
            }

            _characterVisualRoot.gameObject.SetActive(isVisible);
        }
    }
}
