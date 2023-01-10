/// <summary>
/// Struct used for indicating X and Y positions in our GameWorld
/// </summary>
public struct Point
{
    /// <summary>
    /// The x position
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// The y position
    /// </summary>
    public int Y { get; set; }

    /// <summary>
    /// Sets the values of the struct
    /// </summary>
    /// <param name="x">initial x</param>
    /// <param name="y">initial y</param>
    public Point(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public static bool operator ==(Point first, Point second)
    {
        return first.X == second.X && first.Y == second.Y;
    }

    public static bool operator !=(Point first, Point second)
    {
        return first.X != second.X || first.Y != second.Y;
    }

    public static Point operator -(Point x, Point y)
    {
        return new Point(x.X - y.X, x.Y - y.Y);
    }
}
