using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacing : MonoBehaviour
{
    [SerializeField] private GridManager gridManager; // Reference to the grid system
    [SerializeField] private UnitManager unitManager; // Reference to unit manager for spawning units
    private GameObject previewInstance;               // The visual preview of the building
    private BuildingData currentBuilding;             // The building type being placed
    private bool placing = false;                     // Flag to indicate if we're currently placing a building
    private Material[] originalMaterials;             // Backup of original materials for restoring after placement
    private Vector3 baseEuler;                        // Base rotation of the preview object
    private int rotationIndex = 0;                    // 0, 1, 2, 3 => 0°, 90°, 180°, 270°

    public void BeginPlacing(BuildingData buildingData)
    {
        currentBuilding = buildingData;
        previewInstance = Instantiate(buildingData.buildingPrefabs); // Instantiate preview model
        foreach (var col in previewInstance.GetComponentsInChildren<Collider>())
            col.enabled = false; // Disable collisions for placement phase

        placing = true;
        originalMaterials = previewInstance.GetComponentInChildren<Renderer>().materials; // Store original materials
        baseEuler = previewInstance.transform.eulerAngles;
        rotationIndex = 0;
    }

    // Check if we can place the building at a given node
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

        // Cancel placement on ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(previewInstance);
            placing = false;
            return;
        }

        // Rotate the building 90 degrees clockwise on right-click
        if (Input.GetMouseButtonDown(1))
        {
            rotationIndex = (rotationIndex + 1) % 4;
            float newYaw = baseEuler.y + rotationIndex * 90f;
            previewInstance.transform.rotation = Quaternion.Euler(baseEuler.x, newYaw, baseEuler.z);
        }

        // Raycast to ground to follow mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 rawPos = hit.point;
            Vector3 gridPos = new Vector3(
                Mathf.Floor(rawPos.x),
                0f,
                Mathf.Floor(rawPos.z)
            );

            previewInstance.transform.position = gridPos + new Vector3(0, 0.5f, 0); // Offset to center

            var node = gridManager.GetNodeFromWorldPosition(gridPos);
            if (node != null)
            {
                bool canPlace = CanPlaceBuildingAt(node);
                SetGhostColor(canPlace ? Color.green : Color.red); // Change preview color

                // Final placement with left-click
                if (canPlace && Input.GetMouseButtonDown(0))
                {
                    // Mark affected grid nodes as unwalkable and occupied
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

                    // Finalize the visual placement
                    previewInstance.transform.position = gridPos + new Vector3(0, 0.5f, 0);
                    foreach (var col in previewInstance.GetComponentsInChildren<Collider>())
                        col.enabled = true;

                    // Restore original material settings
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

                    // Spawn units if applicable
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
                SetGhostColor(Color.red); // Invalid position
            }
        }
    }

    // Update material to ghost color
    private void SetGhostColor(Color color)
    {
        foreach (var renderer in previewInstance.GetComponentsInChildren<Renderer>())
        {
            foreach (var mat in renderer.materials)
            {
                mat.color = color;
                mat.SetFloat("_Mode", 2); // Transparent
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