using System;
using System.Collections.Generic;
using System.Linq;
using Priority_Queue;
using SaveThePony.Models;

namespace SaveThePony.Services
{
    public class MazeSolver
    {
        class Node
        {
            public Node(MazeTile tile, Node parent, int g = 0)
            {
                Tile = tile;
                Parent = parent;
                G = g;
            }

            public Node Parent { get; set; }
            public MazeTile Tile { get; set; }
            public int G { get; set; }

            public override bool Equals(object obj)
            {
                if (obj is Node otherNode)
                {
                    return Tile.Position.Equals(otherNode.Tile.Position);
                }
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return Tile.Position.GetHashCode();
            }

        }
        public int CalculateManhattanDistance(Point source, Point destination)
        {
            return Math.Abs(destination.X - source.X) + Math.Abs(destination.Y - source.Y);
        }

        public Path Solve(Maze maze)
        {
            Path path = new Path
            {
                MazeId = maze.MazeId,
                Source = maze.Pony.Position,
                Destination = maze.EndPoint,
                Steps = CalculatePath(maze, maze.Pony.Position, maze.EndPoint, maze.Domokun.Position)
            };

            var domokunPath = CalculatePath(maze, maze.Domokun.Position, maze.Pony.Position);

            for (int i = 0; i < Math.Min(path.Length, domokunPath.Count()); i++)
            {
                if (path.Steps.ElementAt(i).Equals(domokunPath.ElementAt(i)) ||
                    domokunPath.ElementAt(i).Equals(path.Steps.ElementAtOrDefault(i + 1)))
                {
                    // Paths collide, the pony will die
                    path.Steps = new List<Point>();
                    break;
                }
            }

            return path;
        }

        IEnumerable<Point> CalculatePath(Maze maze, Point source, Point destination, Point avoidPoint = null)
        {
            List<Point> pathSteps = new List<Point>();
            SimplePriorityQueue<Node, int> openSet = new SimplePriorityQueue<Node, int>();
            HashSet<Point> closedSet = new HashSet<Point>();

            // Start from the endpoint, so that later traversing the parents we get the real path
            openSet.Enqueue(new Node(maze.GetTile(destination), null), 0);

            Node finalNode = null;

            while (openSet.Count != 0)
            {
                Node currentNode = openSet.Dequeue();
                closedSet.Add(currentNode.Tile.Position);

                foreach (var point in currentNode.Tile.AccessibleTiles)
                {
                    if (closedSet.Contains(point))
                    {
                        // Already traversed
                        continue;
                    }

                    if (point.Equals(avoidPoint))
                    {
                        // This tile is not accessible
                        continue;
                    }

                    Node adjacentNode = new Node(maze.GetTile(point), currentNode, currentNode.G + 1);
                    if (point.Equals(source))
                    {
                        finalNode = adjacentNode;
                        break;
                    }

                    int newPriority = CalculatePriority(point, destination, adjacentNode.G);
                    if (openSet.Contains(adjacentNode))
                    {
                        if (newPriority < openSet.GetPriority(adjacentNode))
                        {
                            openSet.UpdatePriority(adjacentNode, newPriority);
                        }
                    }
                    else
                    {
                        openSet.Enqueue(adjacentNode, newPriority);
                    }
                }

                if (finalNode != null)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Couldn't find path, something went wrong");
                }
            }

            Node current = finalNode?.Parent;
            while (current != null)
            {
                pathSteps.Add(current.Tile.Position);
                current = current.Parent;
            }

            return pathSteps;
        }

        int CalculatePriority(Point point, Point endPoint, int currentDistance)
        {
            return currentDistance + CalculateManhattanDistance(point, endPoint);
        }
    }
}
