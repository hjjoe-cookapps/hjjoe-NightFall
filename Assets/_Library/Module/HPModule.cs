using System;

public class HPModule
{
    private float hp;
    private float maxhp;

    public float HP => hp;
    public float MaxHP => maxhp;

    private event Action onDamageEvent;
    private event Action onHealEvent;
    private event Action onDeadEvent;

    public Action OnDamageEvent => onDamageEvent;
    public Action OnHealEvent => onHealEvent;
    public Action OnDeadEvent => onDeadEvent;


    public void ChangeHP(float hp)
    {
        this.hp = hp;
        this.hp = Math.Min(this.hp, maxhp);
        this.hp = Math.Max(this.hp, 0);
    }

    public void ApplyHealthChange(float number)
    {
        if (number > 0)
        {
            SetHeal(number);
        }
        else
        {
            SetDamage(number);
        }
    }

    private void SetHeal(float heal)
    {
        hp += heal;
        hp = Math.Min(hp, maxhp);
        onHealEvent?.Invoke();
    }

    private void SetDamage(float damage)
    {
        hp -= damage;
        hp = Math.Max(hp, 0);
        onDamageEvent?.Invoke();

        if (hp <= 0)
        {
            OnDead();
        }

    }

    public void OnDead()
    {
        onDeadEvent?.Invoke();
    }
}
