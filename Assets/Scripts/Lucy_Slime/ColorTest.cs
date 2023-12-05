using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTest : MonoBehaviour
{
    private const float deg60 = 0.1666666667f;

    [SerializeField] private Color color;
    [SerializeField] private float hue = 0f;
    [SerializeField] private float hueScaled = 0f;

    [SerializeField] private float r, g, b;
    [SerializeField] private float cmax, cmin;

    // Start is called before the first frame update
    void Start()
    {

    }

    private float convertToH(Color color)
    {
        float r = color.r;
        float g = color.g;
        float b = color.b;

        this.r = r;
        this.g = g;
        this.b = b;

        float cmax = Mathf.Max(r, g, b);
        float cmin = Mathf.Min(r, g, b);

        this.cmax = cmax;
        this.cmin = cmin;

        float delta = cmax - cmin;

        if(delta == 0f)
        {
            return 0f;
        }
        
        if(r == cmax)
        {
            return ((((g - b) / delta) + 6) % 6);
        }

        if(g == cmax)
        {
            return (((b - r) / delta) + 2);
        }

        if(b == cmax)
        {
            return (((r - g) / delta) + 4);
        }

        return 0f;
    }

    // Update is called once per frame
    void Update()
    {
        hue = convertToH(color);
        hueScaled = hue * 360;
    }
}
