using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacing : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    private GameObject previewInstance;
    private BuildingData currentBuilding;
    private bool placing = false;

    public void BeginPlacing(BuildingData buildingData)
    {
        currentBuilding = buildingData;
        previewInstance = Instantiate(buildingData.buildingPrefabs);
        placing = true;
    }

    private void Update()
    {
        if (!placing || currentBuilding == null || previewInstance == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 rawPos = hit.point;
            Vector3 gridPos = new Vector3(
                Mathf.Round(rawPos.x),
                0f,
                Mathf.Round(rawPos.z)
            );

            previewInstance.transform.position = gridPos + new Vector3(0, 0.5f, 0);

            if (Input.GetMouseButtonDown(0))
            {
                placing = false;
                // Optionally validate here (walkability, space, etc.)
            }
        }
    }
}
