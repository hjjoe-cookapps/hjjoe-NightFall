using System;

public class HPModule
{
    private float hp;
    private float maxhp;

    public float HP => hp;
    public float MaxHP => maxhp;

    public event Action OnDamageEvent;
    public event Action OnHealEvent;
    public event Action OnDeadEvent;


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
        OnHealEvent?.Invoke();
    }

    private void SetDamage(float damage)
    {
        hp -= damage;
        hp = Math.Max(hp, 0);
        OnDamageEvent?.Invoke();

        if (hp <= 0)
        {
            OnDead();
        }

    }

    private void OnDead()
    {
        OnDeadEvent?.Invoke();
    }
}
