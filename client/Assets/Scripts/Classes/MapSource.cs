using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;

public class MapSource
{
    public static bool[][] Chunks;
    public static readonly Circle PoisonousCircle;
    public static readonly int Length;
    public static readonly int Width;
    public static void UpdateMap(Position position, float radius)
    {
        PoisonousCircle.Update(position, radius);
    }
}