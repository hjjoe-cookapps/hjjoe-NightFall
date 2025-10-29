using _Project.Scripts.Defines;
using UnityEngine;


// TODO : ?를 실제 검사로 변경
public class CastleBehaviour : BuildingBehaviour
{
    private int _earnCoin = 1;
    public override void StartWave()
    {
        base.StartWave();
    }

    public override void EndWave()
    {
        base.EndWave();
        ++_earnCoin;

        if (_state != BuildingState.Crash)
        {
            // create currency;
            for (int i = 0; i < _earnCoin; i++)
            {
                ResourceManager.Instance.Instantiate("Coin", transform.position);
            }
        }

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameManager.Instance.Lose();
    }
}
