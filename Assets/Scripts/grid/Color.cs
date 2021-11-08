namespace MazeWorld
{
    public class Color {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public Color(int red, int green, int blue) {
            R = red;
            G = green;
            B = blue;
        }
        public readonly static Color White = new Color(255, 255, 255);
        public readonly static Color Black = new Color(0, 0, 0);
        public readonly static Color Red = new Color(255, 0, 0);
        public readonly static Color Green = new Color(0, 255, 0);
        public readonly static Color Blue = new Color(0, 0, 255);
        public readonly static Color Yellow = new Color(255, 255, 0);
        public readonly static Color Cyan = new Color(0, 255, 255);
        public readonly static Color Magenta = new Color(255, 0, 255);
        public readonly static Color Orange = new Color(255, 128, 0);
        public readonly static Color Purple = new Color(128, 0, 255);
        public readonly static Color Brown = new Color(128, 64, 0);
        public readonly static Color Gray = new Color(128, 128, 128);
        public readonly static Color DarkGray = new Color(64, 64, 64);
        public readonly static Color LightGray = new Color(192, 192, 192);
        public readonly static Color DarkRed = new Color(128, 0, 0);
        public readonly static Color DarkGreen = new Color(0, 128, 0);
        public readonly static Color DarkBlue = new Color(0, 0, 128);
        public readonly static Color DarkYellow = new Color(128, 128, 0);
        public readonly static Color DarkCyan = new Color(0, 128, 128);
        public readonly static Color DarkMagenta = new Color(128, 0, 128);
        public readonly static Color DarkOrange = new Color(128, 64, 0);
        public readonly static Color DarkPurple = new Color(64, 0, 128);
        public readonly static Color DarkBrown = new Color(64, 32, 0);
        public readonly static Color LightRed = new Color(255, 128, 128);
        public readonly static Color LightGreen = new Color(128, 255, 128);
        public readonly static Color LightBlue = new Color(128, 128, 255);
        public readonly static Color LightYellow = new Color(255, 255, 128);
        public readonly static Color LightCyan = new Color(128, 255, 255);
        public readonly static Color LightMagenta = new Color(255, 128, 255);
        public readonly static Color LightOrange = new Color(255, 192, 128);
        public readonly static Color LightPurple = new Color(192, 128, 255);
        public readonly static Color LightBrown = new Color(192, 96, 128);
    }
}