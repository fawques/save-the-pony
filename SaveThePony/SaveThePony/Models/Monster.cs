namespace SaveThePony.Models
{
    public class Monster : MazeObject
    {
        public Monster(int x, int y) : base(x, y) { }
        public Monster(Point p) : base(p.X, p.Y) { }
    }
}