using UnityEngine;

public class ColorPalette{
    static readonly Texture2D color_palette;
    static ColorPalette() { color_palette = Resources.Load<Texture2D>("Sprites/color_palette"); }
    public static Color getColor(int x, int y) => color_palette.GetPixel(x, y);
    public static Color getPalette(ORB_VALUE o, bool primary) {
        switch (o) {
            case ORB_VALUE.ZERO: return primary ? color_palette.GetPixel(12, 1) : color_palette.GetPixel(14, 1);
            case ORB_VALUE.ONE: return primary ? color_palette.GetPixel(1, 1) : color_palette.GetPixel(0, 1);
            case ORB_VALUE.TWO: return primary ? color_palette.GetPixel(5, 3) : color_palette.GetPixel(4, 3);
            case ORB_VALUE.THREE: return primary ? color_palette.GetPixel(11, 2) : color_palette.GetPixel(9, 2);
            case ORB_VALUE.FOUR: return primary ? color_palette.GetPixel(3, 3) : color_palette.GetPixel(2, 3);
            case ORB_VALUE.FIVE: return primary ? color_palette.GetPixel(3, 1) : color_palette.GetPixel(2, 1);
            case ORB_VALUE.SIX: return primary ? color_palette.GetPixel(14, 3) : color_palette.GetPixel(13, 3);
            case ORB_VALUE.SEVEN: return primary ? color_palette.GetPixel(8, 1) : color_palette.GetPixel(15, 2);
            case ORB_VALUE.EIGHT: return primary ? color_palette.GetPixel(2, 3) : color_palette.GetPixel(6, 3);
            case ORB_VALUE.NINE: return primary ? color_palette.GetPixel(5, 2) : color_palette.GetPixel(4, 2);
            default: return color_palette.GetPixel(0, 0);
        }
    }

    public static void saveColorAsPref(string pref, int x, int y) { PlayerPrefs.SetString(pref, x + "," + y); }

    public static Color loadColorFromPref(string pref) {
        if(PlayerPrefs.HasKey(pref)) {
            string[] coords = pref.Split(',');
            return getColor(int.Parse(coords[0]), int.Parse(coords[1]));
        }
        return getColor(0, 0);
    }
}
