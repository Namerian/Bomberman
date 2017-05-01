using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManagerScript : MonoBehaviour
{
    public static UiManagerScript Instance { get; private set; }

    [SerializeField]
    private Text _bombText;

    [SerializeField]
    private Text _rangeText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SetBombText(int bombs)
    {
        _bombText.text = "Bombs: " + bombs;
    }

    public void SetRangeText(int range)
    {
        _rangeText.text = "Range: " + range;
    }
}
