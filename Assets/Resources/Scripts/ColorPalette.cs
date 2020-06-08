using UnityEngine;

public class ColorPalette{
    static readonly Texture2D color_palette;
    static ColorPalette() { color_palette = Resources.Load<Texture2D>("Sprites/color_palette"); }
    public static Color getColor(int x, int y) { return color_palette.GetPixel(x, y); }
}
