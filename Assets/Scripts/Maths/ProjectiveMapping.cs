using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectiveMapping : MonoBehaviour
{
    [SerializeField][Range(0, 1)] private float x, y;
    [SerializeField][Range(0, 1)] private float x1, y1, x2, y2, x3, y3, x4, y4;

    private Matrix3x3 A;

    void Start()
    {
        A = new Matrix3x3();
    }

    public Matrix3x3 getTransformationMatrix(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
    {
        this.x1 = x1;   this.y1 = y1;
        this.x2 = x2;   this.y2 = y2;
        this.x3 = x3;   this.y3 = y3;
        this.x4 = x4;   this.y4 = y4;

        calculateTransformationParameters();

        return A;
    }

    //Quelle:
    //Burger, Wilhelm & Burge, Mark James (2015)
    //Digitale Bildverarbeitung – Eine algorithmische Einführung in Java (3. Auflage)
    private void calculateTransformationParameters()
    {
        A.a31 = ((x1 - x2 + x3 - x4) * (y4 - y3) - (y1 - y2 + y3 - y4) * (x4 - x3))
              / ((x2 - x3) * (y4 - y3) - (x4 - x3) * (y2 - y3));
        A.a32 = ((y1 - y2 + y3 - y4) * (x2 - x3) - (x1 - x2 + x3 - x4) * (y2 - y3))
              / ((x3 - x3) * (y4 - y3) - (x4 - x3) * (y2 - y3));
        
        A.a11 = x2 - x1 + A.a31 * x2;
        A.a12 = x4 - x1 + A.a32 * x4;
        A.a13 = x1;

        A.a21 = y2 - y1 + A.a31 * y2;
        A.a22 = y4 - y1 + A.a32 * y4;
        A.a23 = y1;

        A.a33 = 1;
    }
}
