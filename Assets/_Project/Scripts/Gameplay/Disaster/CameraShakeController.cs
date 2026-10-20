using UnityEngine;

namespace SiagaBencana.Gameplay
{
    public class CameraFollowController : MonoBehaviour
    {
        [Header("Target Follow")]
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _offset = new Vector3(0f, 9f, -6f);
        [SerializeField] private float _smoothSpeed = 5f;

        private Vector3 _currentVelocity;

        public void SetTarget(Transform target) => _target = target;

        private void Start()
        {
            transform.rotation = Quaternion.Euler(55f, 0f, 0f);

            if (_target == null)
            {
                var player = FindAnyObjectByType<PlayerController>();
                if (player != null) _target = player.transform;
            }
        }

        private void LateUpdate()
        {
            if (_target == null) return;

            Vector3 desiredPosition = _target.position + _offset;
            transform.position = Vector3.SmoothDamp(
                transform.position, desiredPosition, ref _currentVelocity, 1f / _smoothSpeed);

            transform.rotation = Quaternion.Euler(55f, 0f, 0f);
        }
    }
}
