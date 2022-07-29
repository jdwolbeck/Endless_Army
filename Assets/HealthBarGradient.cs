using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarGradient : MonoBehaviour
{
    private Slider slider;
    public Gradient gradient;
    public Image fill;

    void Start()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
