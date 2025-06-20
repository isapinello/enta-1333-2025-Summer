using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacing : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private UnitManager unitManager;
    private GameObject previewInstance;
    private BuildingData currentBuilding;
    private bool placing = false;
    private Material[] originalMaterials;

    public void BeginPlacing(BuildingData buildingData)
    {
        currentBuilding = buildingData;
        previewInstance = Instantiate(buildingData.buildingPrefabs);
        foreach (var col in previewInstance.GetComponentsInChildren<Collider>())
            col.enabled = false;
        placing = true;

        originalMaterials = previewInstance.GetComponentInChildren<Renderer>().materials;
    }

    private bool CanPlaceBuildingAt(GridNode originNode)
    {
        for (int x = 0; x < currentBuilding.gridSizeX; x++)
        {
            for (int y = 0; y < currentBuilding.gridSizeY; y++)
            {
                int gx = originNode.GridX - currentBuilding.gridOffsetX + x;
                int gy = originNode.GridY - currentBuilding.gridOffsetY + y;

                if (!gridManager.IsValidCoord(gx, gy)) return false;
                var node = gridManager.GetGrid()[gx, gy];
                if (!node.walkable || node.IsOccupied) return false;
            }
        }
        return true;
    }

    private void Update()
    {
        if (!placing || currentBuilding == null || previewInstance == null) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(previewInstance);
            placing = false;
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            previewInstance.transform.Rotate(0, 90f, 0);
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 rawPos = hit.point;
            Vector3 gridPos = new Vector3(
                Mathf.Floor(rawPos.x),
                0f,
                Mathf.Floor(rawPos.z)
            );

            previewInstance.transform.position = gridPos + new Vector3(0, 0.5f, 0);

            var node = gridManager.GetNodeFromWorldPosition(gridPos);
            if (node != null)
            {
                bool canPlace = CanPlaceBuildingAt(node);

                SetGhostColor(canPlace ? Color.green : Color.red);

                if (canPlace && Input.GetMouseButtonDown(0))
                {
                    for (int x = 0; x < currentBuilding.gridSizeX; x++)
                    {
                        for (int y = 0; y < currentBuilding.gridSizeY; y++)
                        {
                            int gx = node.GridX - currentBuilding.gridOffsetX + x;
                            int gy = node.GridY - currentBuilding.gridOffsetY + y;

                            var n = gridManager.GetGrid()[gx, gy];
                            n.IsOccupied = true;
                            n.walkable = false;
                        }
                    }

                    previewInstance.transform.position = gridPos + new Vector3(0, 0.5f, 0);
                    foreach (var col in previewInstance.GetComponentsInChildren<Collider>())
                        col.enabled = true;

                    foreach (var renderer in previewInstance.GetComponentsInChildren<Renderer>())
                    {
                        foreach (var mat in renderer.materials)
                        {
                            mat.color = Color.white;
                            mat.SetFloat("_Mode", 0);
                            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                            mat.SetInt("_ZWrite", 1);
                            mat.DisableKeyword("_ALPHATEST_ON");
                            mat.DisableKeyword("_ALPHABLEND_ON");
                            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                            mat.renderQueue = -1;
                        }
                    }

                    if (currentBuilding.canSpawnUnits)
                    {
                        unitManager.SpawnUnitsNear(
                            gridPos - new Vector3(currentBuilding.gridOffsetX, 0f, currentBuilding.gridOffsetY),
                            5,
                            currentBuilding.gridSizeX,
                            currentBuilding.gridSizeY
                        );
                    }

                    previewInstance = null;
                    placing = false;
                }
            }
            else
            {
                SetGhostColor(Color.red);
            }
        }
    }

    private void SetGhostColor(Color color)
    {
        foreach (var renderer in previewInstance.GetComponentsInChildren<Renderer>())
        {
            foreach (var mat in renderer.materials)
            {
                mat.color = color;
                mat.SetFloat("_Mode", 2);
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
            }
        }
    }
}