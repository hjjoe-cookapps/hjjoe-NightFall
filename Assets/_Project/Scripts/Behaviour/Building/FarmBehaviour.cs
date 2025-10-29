using _Project.Scripts.Defines;

public class FarmBehaviour : BuildingBehaviour
{
    private int _earnCoin = 1;

    public override void EndWave()
    {
        base.EndWave();

        if (_state != BuildingState.Crash)
        {
            // create currency;
            for (int i = 0; i < _earnCoin; i++)
            {
                ResourceManager.Instance.Instantiate("Coin", transform.position);
            }
        }
    }
}
