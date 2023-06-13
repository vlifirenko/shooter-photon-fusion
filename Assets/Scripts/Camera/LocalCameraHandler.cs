using ShooterPhotonFusion.Movement;
using UnityEngine;

namespace ShooterPhotonFusion.Camera
{
    public class LocalCameraHandler : MonoBehaviour
    {
        [SerializeField] private Transform cameraAnchorPoint;

        private UnityEngine.Camera _localCamera;
        private NetworkCharacterControllerPrototypeCustom _networkCharacterControllerPrototypeCustom;
        private float _cameraRotationX;
        private float _cameraRotationY;
        private Vector2 _viewInput;

        private void Awake()
        {
            _localCamera = GetComponent<UnityEngine.Camera>();
            _networkCharacterControllerPrototypeCustom = GetComponentInParent<NetworkCharacterControllerPrototypeCustom>();
        }

        private void Start()
        {
            if (_localCamera.enabled)
                _localCamera.transform.parent = null;
        }

        private void Update()
        {
        }

        private void LateUpdate()
        {
            if (cameraAnchorPoint == null)
                return;
            if (!_localCamera.enabled)
                return;

            _localCamera.transform.position = cameraAnchorPoint.position;

            _cameraRotationX += _viewInput.y * Time.deltaTime * _networkCharacterControllerPrototypeCustom.viewUpDownRotationSpeed;
            _cameraRotationX = Mathf.Clamp(_cameraRotationX, -90, 90);
            _cameraRotationY += _viewInput.x * Time.deltaTime * _networkCharacterControllerPrototypeCustom.viewUpDownRotationSpeed;

            _localCamera.transform.rotation = Quaternion.Euler(_cameraRotationX, _cameraRotationY, 0);
        }

        public void SetViewInputVector(Vector2 viewInput) => _viewInput = viewInput;
    }
}