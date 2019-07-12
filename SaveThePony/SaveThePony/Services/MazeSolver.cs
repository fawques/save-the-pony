using System;
using System.Collections.Generic;
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
                Source = maze.Pony.Position,
                Destination = maze.EndPoint,
                Steps = CalculatePath(maze)
            };

            return path;
        }

        IEnumerable<Point> CalculatePath(Maze maze)
        {
            List<Point> pathSteps = new List<Point>();
            SimplePriorityQueue<Node, int> openSet = new SimplePriorityQueue<Node, int>();
            HashSet<Point> closedSet = new HashSet<Point>();

            // Start from the endpoint, so that later traversing the parents we get the real path
            openSet.Enqueue(new Node(maze.GetTile(maze.EndPoint), null), 0);

            Node finalNode = null;

            while (openSet.Count != 0)
            {
                Node currentNode = openSet.Dequeue();
                closedSet.Add(currentNode.Tile.Position);

                foreach (var point in currentNode.Tile.AccessibleTiles)
                {
                    if (closedSet.Contains(point))
                    {
                        continue;
                    }

                    Node adjacentNode = new Node(maze.GetTile(point), currentNode, currentNode.G + 1);
                    if (point.Equals(maze.Pony.Position))
                    {
                        finalNode = adjacentNode;
                        break;
                    }

                    int newPriority = CalculatePriority(point, maze.EndPoint, adjacentNode.G);
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

            Node current = finalNode;
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
