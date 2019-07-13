namespace SaveThePony.Models
{
    public class Pony : MazeObject
    {
        public Pony(int x, int y) : base(x, y) { }
        public Pony(Point p) : base(p.X, p.Y) { }
        public string Name { get; set; }
    }
}