using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInstance : MonoBehaviour
{
    private void Awake()
    {
        gameObject.AddComponent<Damageable>();
    }
}
