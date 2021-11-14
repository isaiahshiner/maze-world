using static MazeWorld.Color;
using static UnityEngine.Color;
using System.Collections.Generic;

public class ColorMap
{
    private static readonly Dictionary<MazeWorld.Color, UnityEngine.Color32> colorMap = new Dictionary<MazeWorld.Color, UnityEngine.Color32>();
    public static UnityEngine.Color ConvertColor(MazeWorld.Color color)
    {
        if (colorMap.ContainsKey(color))
        {
            return colorMap[color];
        } else {
            var c = new UnityEngine.Color32((byte)color.R, (byte)color.G, (byte)color.B, 255);
            colorMap.Add(color, c);
            return c;
        }
    }
}
