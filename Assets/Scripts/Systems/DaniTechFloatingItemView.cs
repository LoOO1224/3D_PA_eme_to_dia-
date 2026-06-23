using UnityEngine;

namespace EmeToDia.Gameplay
{
    // 아이템의 회전, 부유, 월드 라벨 표시만 담당하는 View 컴포넌트입니다.
    public sealed class DaniTechFloatingItemView : MonoBehaviour
    {
        [SerializeField] private Transform _visualRoot;
        [SerializeField] private TextMesh _labelText;
        [SerializeField] private float _rotateSpeed = 65f;
        [SerializeField] private float _bobAmplitude = 0.12f;
        [SerializeField] private float _bobSpeed = 2.5f;

        private Vector3 _startLocalPosition;

        private void Awake()
        {
            ResolveReferences();
            _startLocalPosition = _visualRoot.localPosition;
        }

        private void Update()
        {
            RotateVisual();
            BobVisual();
            FaceLabelToCamera();
        }

        public void SetLabel(string label)
        {
            ResolveReferences();
            if (_labelText == null)
            {
                return;
            }

            _labelText.text = label;
        }

        private void ResolveReferences()
        {
            if (_visualRoot == null)
            {
                _visualRoot = transform;
            }

            if (_labelText != null)
            {
                return;
            }

            _labelText = GetComponentInChildren<TextMesh>();
        }

        private void RotateVisual()
        {
            if (_visualRoot == null)
            {
                return;
            }

            _visualRoot.Rotate(Vector3.up, _rotateSpeed * Time.deltaTime, Space.World);
        }

        private void BobVisual()
        {
            if (_visualRoot == null)
            {
                return;
            }

            float bobOffset = Mathf.Sin(Time.time * _bobSpeed) * _bobAmplitude;
            _visualRoot.localPosition = _startLocalPosition + Vector3.up * bobOffset;
        }

        private void FaceLabelToCamera()
        {
            if (_labelText == null || Camera.main == null)
            {
                return;
            }

            _labelText.transform.rotation = Quaternion.LookRotation(_labelText.transform.position - Camera.main.transform.position);
        }
    }
}
