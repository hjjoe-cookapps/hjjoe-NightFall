using _Project.Scripts.Defines;
using CookApps.Inspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FarmBehaviour : BuildingBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _textMeshProUGUI;

    [Required]
    [SerializeField]
    private Image _iconImage;

    private int _earnWood = 1;

    private void Awake()
    {
        _textMeshProUGUI = _textMeshProUGUI == null ? gameObject.GetComponentInChildren<TextMeshProUGUI>() : _textMeshProUGUI;
    }

    public override void EndWave()
    {
        if (_state != BuildingState.Crash)
        {
            // create currency;
            for (int i = 0; i < _earnWood; i++)
            {
                ResourceManager.Instance.Instantiate("Currency/wood", transform.position);
            }
            _iconImage.color = Color.white;
        }
        else
        {
            _iconImage.color = Color.red;
        }

        base.EndWave();
    }
}
