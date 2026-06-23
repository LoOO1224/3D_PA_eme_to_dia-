using System;
using UnityEngine;

namespace EmeToDia.Gameplay
{
    // 플레이어가 바라보는 오브젝트와 상호작용하는 입구 컴포넌트입니다.
    // 실제 아이템 획득 처리는 대상 오브젝트의 IInteractable 구현체가 처리합니다.
    public sealed class DaniTechPlayerInteraction : MonoBehaviour
    {
        [SerializeField] private float _interactionDistance = 4f;
        [SerializeField] private LayerMask _interactionLayerMask = ~0;
        [SerializeField] private KeyCode _interactionKey = KeyCode.E;
        [SerializeField] private Transform _cameraTransform;

        private string _currentHintText;

        public event Action<string> OnInteractionHintChanged;

        private void Start()
        {
            ConnectCameraIfNeeded();
        }

        private void Update()
        {
            RefreshInteractionHint();
            ReadInteractionInput();
        }

        public void TryInteract()
        {
            IInteractable interactable = GetInteractableFromRay();
            if (interactable == null)
            {
                return;
            }

            if (interactable.CanInteract(gameObject) == false)
            {
                return;
            }

            interactable.Interact(gameObject);
        }

        private void ReadInteractionInput()
        {
            if (Input.GetKeyDown(_interactionKey))
            {
                TryInteract();
            }
        }

        private void RefreshInteractionHint()
        {
            IInteractable interactable = GetInteractableFromRay();
            string hintText = string.Empty;
            if (interactable != null && interactable.CanInteract(gameObject))
            {
                hintText = GetHintText(interactable);
            }

            SetHintText(hintText);
        }

        private string GetHintText(IInteractable interactable)
        {
            DaniTechItemPickup itemPickup = interactable as DaniTechItemPickup;
            if (itemPickup != null)
            {
                return itemPickup.GetInteractionText();
            }

            return "E 상호작용";
        }

        private void SetHintText(string hintText)
        {
            if (_currentHintText == hintText)
            {
                return;
            }

            _currentHintText = hintText;
            if (OnInteractionHintChanged != null)
            {
                OnInteractionHintChanged.Invoke(_currentHintText);
            }
        }

        private void ConnectCameraIfNeeded()
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

        private IInteractable GetInteractableFromRay()
        {
            if (_cameraTransform == null)
            {
                return null;
            }

            Ray ray = new Ray(_cameraTransform.position, _cameraTransform.forward);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, _interactionDistance, _interactionLayerMask) == false)
            {
                return null;
            }

            return hitInfo.collider.GetComponentInParent<IInteractable>();
        }
    }
}
