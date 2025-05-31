using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    [SerializeField] protected TagUnitType _unitType;

    //public virtual int Width => _unitType != null ? _unitType.Width : 1;
}
