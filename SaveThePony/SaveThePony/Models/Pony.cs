namespace SaveThePony.Models
{
    public class Pony : MazeObject
    {
        public Pony(int x, int y) : base(x, y) { }
        public string Name { get; set; }
    }
}