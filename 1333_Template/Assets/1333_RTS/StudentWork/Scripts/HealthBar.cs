using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Vector3 _offset = new Vector3(0, 2, 0);

    private Transform _target;

    public void AttachTo(Transform target)
    {
        _target = target;
    }

    public void SetValue(float value)
    {
        _slider.value = value;
    }

    private void LateUpdate()
    {
        if (_target != null)
        {
            transform.position = _target.position + _offset;
            transform.forward = Camera.main.transform.forward;
        }
    }
}
