using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    [SerializeField] private int wood = 0;
    [SerializeField] private int woodPerTick = 10;
    [SerializeField] private float tickRate = 5f;
    [SerializeField] private TextMeshProUGUI woodText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        UpdateUI();
        InvokeRepeating(nameof(GainWood), tickRate, tickRate);
    }

    void GainWood()
    {
        wood += woodPerTick;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (woodText != null)
            woodText.text = "Wood: " + wood;
    }

    public bool CanAfford(int amount)
    {
        return wood >= amount;
    }

    public void SpendWood(int amount)
    {
        wood -= amount;
        UpdateUI();
    }

    public int GetCurrentWood() => wood;
}