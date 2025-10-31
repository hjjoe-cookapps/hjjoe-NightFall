
using UnityEngine;

public class ArrowBehaviour : MonoBehaviour
{
    private static readonly float _travelDuration = 1.5f;
    private static readonly float _initialLift = 3f;
    private static readonly float _finalDrop = 0.5f;


    private GameObject _creator;
    private float _damage;
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

    public void Init(GameObject creator, float damage, Vector3 startPosition, Transform target)
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
            _controlPoint2 = _target.transform.position + Vector3.up * _finalDrop;
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

            // TODO : ChangeEffect;
            ResourceManager.Instance.Instantiate("Effect/Smoke_burst_1", transform.position);
            ResourceManager.Instance.Destroy(gameObject);
            return;
        }

        Vector3 currentPosition = Bezier.GetBezierPoint(t,
            _startPosition,
            _controlPoint1,
            _controlPoint2,
            _targetPosition);

        transform.position = currentPosition;

        Vector3 nextPosition = Bezier.GetBezierPoint(t + 0.01f,
            _startPosition,
            _controlPoint1,
            _controlPoint2,
            _targetPosition);

        Vector3 tangentDirection = (nextPosition - currentPosition).normalized;

        if (tangentDirection != Vector3.zero)
        {
// Vector2 방향을 오일러 각도로 변환합니다. (Y축 회전)
            // Mathf.Atan2(y, x)를 사용하여 라디안 각도를 얻은 후, Degree로 변환합니다.
            float angle = Mathf.Atan2(tangentDirection.y, tangentDirection.x) * Mathf.Rad2Deg;

            // 스프라이트가 기본적으로 오른쪽(X축)을 바라보도록 설정된 경우
            // Z축을 기준으로 angle만큼 회전합니다.
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    private void Hit()
    {
        if (_target != null && _target.gameObject.activeSelf)
        {
            HPModule module = _target.gameObject.GetRoot().GetComponent<HPModule>();
            if (module != null)
            {
                module.TakeDamage(_damage, _creator);
            }
        }
    }
}
