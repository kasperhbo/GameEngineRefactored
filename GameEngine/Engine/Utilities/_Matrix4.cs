using Accord.Math;

public class _Matrix4
{
    public static Matrix4x4 CreateScale()
    {
        return new Matrix4x4();
    }
    
    public static Matrix4x4 CreateTranslation(Matrix4x4 mat, double x, double y, double z)
    {
        mat.V30 = (float)Math.FusedMultiplyAdd(mat.V00, x, Math.FusedMultiplyAdd(mat.V10, y, Math.FusedMultiplyAdd(mat.V20, z, mat.V30)));
        mat.V31 = (float)Math.FusedMultiplyAdd(mat.V01, x, Math.FusedMultiplyAdd(mat.V11, y, Math.FusedMultiplyAdd(mat.V21, z, mat.V31)));
        mat.V32 = (float)Math.FusedMultiplyAdd(mat.V02, x, Math.FusedMultiplyAdd(mat.V12, y, Math.FusedMultiplyAdd(mat.V22, z, mat.V32)));
        mat.V33 = (float)Math.FusedMultiplyAdd(mat.V03, x, Math.FusedMultiplyAdd(mat.V13, y, Math.FusedMultiplyAdd(mat.V23, z, mat.V33)));
        
        return mat;
    }

    public static void Print(Matrix4x4 mat)
    {
        Console.WriteLine (mat.V00 + ", " + mat.V01 + ", " + mat.V02 + ", " + mat.V03);
        Console.WriteLine (mat.V10 + ", " + mat.V11 + ", " + mat.V22 + ", " + mat.V33);
        Console.WriteLine (mat.V20 + ", " + mat.V21 + ", " + mat.V22 + ", " + mat.V33);
        Console.WriteLine (mat.V30 + ", " + mat.V31 + ", " + mat.V22 + ", " + mat.V33);
    }
}