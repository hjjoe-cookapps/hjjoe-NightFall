using System;
using UnityEngine;

public class HPModule : MonoBehaviour
{
    private float _hp;
    private float _maxhp;

    public float HP => _hp;
    public float MaxHP => _maxhp;

    public event Action OnDamageEvent;
    public event Action OnHealEvent;

    // 객체 동작 처리용
    public event Action<GameObject> OnDamageEventOpponent;
    public event Action<GameObject> OnHealEventOpponent;
    public event Action OnDeadEvent;

    // Setup HP
    public void Init(float hp)
    {
        _maxhp = hp;
        _hp = hp;
    }

    public void Reset()
    {
        _hp = _maxhp;
    }

    public void SetHP(float hp)
    {
        _hp = Math.Clamp(hp, 0, _maxhp);
    }

    public void Heal(float heal, GameObject obj)
    {
        _hp += heal;
        _hp = Math.Clamp(_hp, 0, _maxhp);
        OnHealEvent?.Invoke();
        OnHealEventOpponent?.Invoke(obj);
    }

    public void TakeDamage(float damage, GameObject obj)
    {
        _hp -= damage;
        _hp = Math.Clamp(_hp, 0, _maxhp);

        OnDamageEvent?.Invoke();

        if (_hp <= 0)
        {
            OnDead();
        }
        else
        {
            OnDamageEventOpponent?.Invoke(obj);
        }
    }

    private void OnDead()
    {
        OnDeadEvent?.Invoke();
    }
}
