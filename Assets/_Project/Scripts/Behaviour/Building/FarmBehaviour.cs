using _Project.Scripts.Defines;
using CookApps.Inspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FarmBehaviour : BuildingBehaviour
{
    [Header("Farm")]
    [Required]
    [SerializeField]
    private GameObject _earnInfoUI;
    [Required]
    [SerializeField]
    private TextMeshProUGUI _textMeshProUGUI;
    [Required]
    [SerializeField]
    private Image _iconImage;

    private void Awake()
    {
        _textMeshProUGUI = _textMeshProUGUI == null ? gameObject.GetComponentInChildren<TextMeshProUGUI>() : _textMeshProUGUI;
    }

    public override void StartWave()
    {
        base.StartWave();
        _earnInfoUI.SetActive(false);
    }

    public override void EndWave()
    {
        if (_state != BuildingState.Crash)
        {
            // create currency;
            for (int i = 0; i < Level; i++)
            {
                ResourceManager.Instance.Instantiate("Currency/wood", transform.position);
            }
            _iconImage.color = Color.white;
        }
        else
        {
            _iconImage.color = Color.red;
        }

        if (Level != 0)
        {
            _earnInfoUI.SetActive(true);
        }

        base.EndWave();
    }

    public override void Upgrade()
    {
        base.Upgrade();
        _earnInfoUI.SetActive(true);
        _textMeshProUGUI.text = Level.ToString();
    }
}
