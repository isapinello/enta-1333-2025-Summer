using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingPlacementUI : MonoBehaviour
{
    [SerializeField] private RectTransform LayoutGroupParent;
    [SerializeField] private Button ButtonPrefab;
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

            BuildingData localData = t; // Fix lambda capture issue

            Button button = Instantiate(ButtonPrefab, LayoutGroupParent).GetComponent<Button>();
            TMP_Text label = button.GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = localData.BuildingId;

            button.onClick.AddListener(() => gameManager.StartPlacingBuilding(localData));
        }
    }
}
