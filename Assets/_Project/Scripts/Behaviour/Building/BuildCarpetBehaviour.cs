using CookApps.Inspector;
using TMPro;
using UnityEngine;

public class BuildCarpetBehaviour : MonoBehaviour
{
    [Required]
    [SerializeField]
    private TextMeshProUGUI _costText;

    [Required]
    [SerializeField]
    private GameObject _upgradeUI;

    public void OnUpgrade(int nextCost)
    {
        _costText.text = nextCost.ToString();

        if (nextCost == 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _upgradeUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _upgradeUI.SetActive(false);
        }
    }
}

