using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Durability : MonoBehaviour
{
    [SerializeField] private int durability = 60;
    [SerializeField] private int decayAmount = 10;
    [SerializeField] private float decayInterval = 5f;

    private GridManager gridManager;
    private List<GridNode> occupiedNodes = new();

    public void Setup(GridManager gridManager, List<GridNode> nodes)
    {
        this.gridManager = gridManager;
        this.occupiedNodes = nodes;
    }

    private void Start()
    {
        InvokeRepeating(nameof(Decay), decayInterval, decayInterval);
    }

    private void Decay()
    {
        durability -= decayAmount;
        Debug.Log($"{gameObject.name} durability: {durability}");

        if (durability <= 0)
        {
            foreach (var node in occupiedNodes)
            {
                node.IsOccupied = false;
                node.walkable = true;
            }

            Debug.Log($"{gameObject.name} destroyed and grid cleared.");
            Destroy(gameObject);
        }
    }
}
