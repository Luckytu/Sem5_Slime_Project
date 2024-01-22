using System;
using System.Collections;
using System.Collections.Generic;

public class Matrix3x3
{
    public float a11 { get; set; } public float a12{ get; set; } public float a13 { get; set; }
    public float a21 { get; set; } public float a22{ get; set; } public float a23 { get; set; }
    public float a31 { get; set; } public float a32{ get; set; } public float a33 { get; set; }
    
    public Matrix3x3()
    { }

    public Matrix3x3(float a11, float a12, float a13, float a21, float a22, float a23, float a31, float a32, float a33)
    {
        this.a11 = a11;   this.a12 = a12;   this.a13 = a13;
        
        this.a21 = a21;   this.a22 = a22;   this.a23 = a23;
        
        this.a31 = a31;   this.a32 = a32;   this.a33 = a33;
    }

    public Matrix3x3 getAdjoint()
    {
        Matrix3x3 Aadj = new Matrix3x3();

        Aadj.a11 = a22 * a33 - a23 * a32;   Aadj.a12 = a13 * a32 - a12 * a33;   Aadj.a13 = a12 * a23 - a13 * a22;

        Aadj.a21 = a23 * a31 - a21 * a33;   Aadj.a22 = a11 * a33 - a13 * a31;   Aadj.a23 = a13 * a21 - a11 * a23;

        Aadj.a31 = a21 * a32 - a22 * a31;   Aadj.a32 = a12 * a31 - a11 * a32;   Aadj.a33 = a11 * a22 - a12 * a21;

        return Aadj;
    }

    public float this[int row, int col]
    {
        get { return get(row, col); }
        set { set(row, col, value); }
    }

    private float get(int row, int col)
    {
        if(row == 0)
        {
            if (col == 0) return a11;
            if (col == 1) return a12;
            if (col == 2) return a13;
        }
        
        if(row == 1)
        {
            if (col == 0) return a21;
            if (col == 1) return a22;
            if (col == 2) return a23;
        }
        
        if(row == 2)
        {
            if (col == 0) return a31;
            if (col == 1) return a32;
            if (col == 2) return a33;
        }

        throw new ArgumentOutOfRangeException();
    }

    private void set(int row, int col, float value)
    {
        if(row > 2 || col > 2)
        {
            throw new ArgumentOutOfRangeException();
        }

        if (row == 0)
        {
            if (col == 0) a11 = value;
            if (col == 1) a12 = value;
            if (col == 2) a13 = value;
        }

        if (row == 1)
        {
            if (col == 0) a21 = value;
            if (col == 1) a22 = value;
            if (col == 2) a23 = value;
        }

        if (row == 2)
        {
            if (col == 0) a31 = value;
            if (col == 1) a32 = value;
            if (col == 2) a33 = value;
        }
    }
}
