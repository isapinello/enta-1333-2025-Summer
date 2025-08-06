using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image _fill = null;

    private Camera _cam;

    private void Awake()
    {
        _cam = Camera.main;
    }

    // See camera
    private void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(
            transform.position - _cam.transform.position, Vector3.up);
    }

    public void SetRatio(float r)
    {
        if (_fill == null) return;

        _fill.fillAmount = Mathf.Clamp01(r);
        _fill.color = Color.Lerp(Color.red, Color.green, _fill.fillAmount);
    }
}
