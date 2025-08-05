using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingPlacementUI : MonoBehaviour
{
    [SerializeField] private RectTransform LayoutGroupParent;
    [SerializeField] private SelectBuildingButton ButtonPrefab;
    [SerializeField] private BuildingTypeSo BuildingData;
    [SerializeField] private GameManager gameManager;

    private void Start()
    {
        foreach (BuildingData t in BuildingData.Buildings)
        {
            if (t == null || t.buildingPrefabs == null)
            {
                Debug.LogWarning("[UI] Skipped null BuildingData or missing prefab.");
                continue;
            }

            SelectBuildingButton button = Instantiate(ButtonPrefab, LayoutGroupParent);
            button.Setup(t, gameManager);
        }
    }
}