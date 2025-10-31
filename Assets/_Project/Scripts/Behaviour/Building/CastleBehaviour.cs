using _Project.Scripts.Defines;
using CookApps.Inspector;
using UnityEngine;


// TODO : ?를 실제 검사로 변경
public class CastleBehaviour : BuildingBehaviour
{
    [Required]
    [SerializeField]
    private GameObject _startButton;

    public override void EndWave()
    {
        base.EndWave();

        if (_state != BuildingState.Crash)
        {
            // create currency;
            for (int i = 0; i < GameManager.Instance.CurrentWaveCount; i++)
            {
                ResourceManager.Instance.Instantiate("Currency/Wood", transform.position);
            }
        }

    }

    public override void Upgrade()
    {
        base.Upgrade();
        _startButton.SetActive(true);

        GameManager.Instance.Castle = _default[Level];
    }

    protected override void OnBuildingDestroy()
    {
        base.OnBuildingDestroy();
        GameManager.Instance.Lose();
    }
}
