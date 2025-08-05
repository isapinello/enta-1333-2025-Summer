using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectBuildingButton : MonoBehaviour
{
    [SerializeField] private TMP_Text _buttonText;
    [SerializeField] private Button _button;
    [SerializeField] private AudioClip clickSound;

    private BuildingData _buildingData;

    public void Setup(BuildingData buildingData, GameManager manager)
    {
        _buildingData = buildingData;
        _buttonText.text = _buildingData.BuildingId;

        _button.onClick.AddListener(() =>
        {
            manager.StartPlacingBuilding(_buildingData);
            AudioManager.Instance.PlayButtonSound();
        });
    }
}
