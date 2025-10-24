using System;
using UnityEngine;

public class HPModule : MonoBehaviour
{
    private int _hp;
    private int _maxhp;

    public int HP => _hp;
    public int MaxHP => _maxhp;

    // 수치 관련 UI 처리용
    public event Action<int> OnDamageEventNumber;
    public event Action<int> OnHealEventNumber;

    // 객체 동작 처리용
    public event Action<GameObject> OnDamageEventOpponent;
    public event Action<GameObject> OnHealEventOpponent;
    public event Action OnDeadEvent;

    // Setup HP
    public void Init(int hp)
    {
        _maxhp = hp;
        _hp = hp;
    }

    public void SetHP(int hp)
    {
        _hp = Math.Clamp(hp, 0, _maxhp);
    }

    public void Heal(int heal, GameObject obj)
    {
        _hp += heal;
        _hp = Math.Clamp(_hp, 0, _maxhp);
        OnHealEventNumber?.Invoke(heal);
        OnHealEventOpponent?.Invoke(obj);
    }

    public void TakeDamage(int damage, GameObject obj)
    {
        _hp -= damage;
        _hp = Math.Clamp(_hp, 0, _maxhp);

        OnDamageEventNumber?.Invoke(damage);

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
