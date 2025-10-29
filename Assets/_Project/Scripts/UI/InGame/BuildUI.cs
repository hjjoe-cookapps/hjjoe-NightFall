using System;
using CookApps.Inspector;
using UnityEngine;

public class BuildUI : MonoBehaviour
{
    [Required]
    [SerializeField]
    private GameObject _ui;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Hero")
        {
            _ui.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Hero")
        {
            _ui.SetActive(false);
        }
    }
}

