using System.Collections;
using System.Collections.Generic;
using IngameDebugConsole;
using UnityEngine;

public class DebugCommands : MonoBehaviour
{
    [SerializeField] private UnitManager _unitManager;
    private void OnEnable()
    {
        DebugLogConsole.AddCommand("HelloWorld", "says  heyy", HelloWorld);
    }
    private void OnDisable()
    {
        DebugLogConsole.RemoveCommand("HelloWorld");
    }
    private void HelloWorld()
    {
        Debug.Log("heyyy");
    }
}
