
using UnityEngine;

public class ArrowBehaviour : MonoBehaviour
{
    private static readonly float _travelDuration = 1.0f;
    private static readonly float _initialLift = 10f;
    private static readonly float _finalDrop = 1f;


    private GameObject _creator;
    private int _damage;
    private Vector3 _startPosition;
    private Transform _target;
    private Vector3 _targetPosition; //_target 사망시 도착 위치 update없이 이동후 삭제 처리

    private float _timeElapsed;
    private Vector3 _controlPoint1;
    private Vector3 _controlPoint2;

    private void OnEnable()
    {
        _timeElapsed = 0;
    }
    private void Update()
    {
        UpdateTargetPosition();
        BezierMovement();
    }

    public void Init(GameObject creator, int damage, Vector3 startPosition, Transform target)
    {
        _creator = creator;
        _damage = damage;
        _startPosition = startPosition;
        _target = target;

        _controlPoint1 = _startPosition + Vector3.up * _initialLift;
        _controlPoint2 = target.transform.position + Vector3.up * _finalDrop;
    }

    private void UpdateTargetPosition()
    {
        if (_target != null && _target.gameObject.activeSelf)
        {
            _targetPosition = _target.transform.position;
        }
    }

    private void BezierMovement()
    {
        _timeElapsed += Time.deltaTime;
        float t = _timeElapsed / _travelDuration;

        if (t >= 1.0f)
        {
            t = 1f;
            Hit();
            return;
        }

        Vector3 currentPosition = Bezier.GetBezierPoint(t,
            _startPosition,
            _controlPoint1,
            _controlPoint2,
            _targetPosition);

        transform.position = currentPosition;

        Vector3 nextPosition = Bezier.GetBezierPoint(t + 0.1f,
            _startPosition,
            _controlPoint1,
            _controlPoint2,
            _targetPosition);

        Vector3 tangentDirection = (nextPosition - currentPosition).normalized;

        if (tangentDirection != Vector3.zero)
        {
            // A. 카메라 정보 가져오기
            Vector3 cameraLook = Camera.main.transform.forward; // 카메라의 Look Vector (L)
            Vector3 cameraUp = Camera.main.transform.up;       // 카메라의 Up Vector (U)

            Vector3 projected = tangentDirection - Vector3.Dot(tangentDirection, cameraLook) * cameraLook;

            if (projected.sqrMagnitude > 0.0001f) // 유효한 벡터인지 확인
            {
                Quaternion targetRotation = Quaternion.LookRotation(projected, cameraUp);
                Quaternion rotationOffset = Quaternion.Euler(0, -90f, 0);

                transform.rotation = targetRotation * rotationOffset;
            }
        }
    }

    private void Hit()
    {
        if (_target != null && _target.gameObject.activeSelf)
        {
            _target.gameObject.GetRoot().GetComponent<HPModule>().TakeDamage(_damage, _creator);
        }

        // TODO : ChangeEffect;
        ResourceManager.Instance.Instantiate("Effect/Smoke_burst_1", transform.position);
        ResourceManager.Instance.Destroy(gameObject);

    }
}
