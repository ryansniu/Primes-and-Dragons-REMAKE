using UnityEngine;

public class ColorPalette{
    static readonly Texture2D color_palette;
    static ColorPalette() { color_palette = (Texture2D)Resources.Load("Sprites/color_palette", typeof(Texture2D)) as Texture2D; }
    public static Color getColor(int x, int y) { return color_palette.GetPixel(x, y); }
}
