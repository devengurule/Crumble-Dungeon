using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    private GameController gameController;

    private Node[,] nodes;
    private int width;
    private int height;

    private void Start()
    {
        gameController = GameController.instance;

        width = gameController.GridWidth();
        height = gameController.GridHeight();

        nodes = new Node[width, height];
    }

    public List<Vector2Int> FindPath(CellData start, CellData end)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                CellData cell = gameController.GetCellData(new Vector2Int(x, y));
                nodes[x, y] = new Node(cell, int.MaxValue, 0, null);
            }
        }

        List<Node> open = new();
        HashSet<Node> closed = new();

        Node startNode = nodes[start.position.x, start.position.y];
        startNode.gCost = 0;
        startNode.hCost = Heuristic(start, end);

        open.Add(startNode);

        int iterations = 0;

        while (open.Count > 0)
        {
            iterations++;
            if (iterations > 10000)
            {
                Debug.LogError("A* exceeded 10000 iterations.");
                return null;
            }

            Node current = open[0];

            foreach (Node node in open)
            {
                if (node.fCost < current.fCost)
                {
                    current = node;
                }
                else if (node.fCost ==  current.fCost && node.hCost < current.hCost)
                {
                    current = node;
                }
            }

            open.Remove(current);
            closed.Add(current);

            if (current.cell.position == end.position)
            {
                return BuildPath(startNode, current);
            }

            foreach(Node neighbor in GetNeighbors(current))
            {
                if (neighbor.cell.position != end.position && neighbor.cell.cellType != CellType.empty) continue;
                
                if (closed.Contains(neighbor)) continue;

                bool diagonal = current.cell.position.x != neighbor.cell.position.x && current.cell.position.y != neighbor.cell.position.y;

                int moveCost = diagonal ? 14 : 10;

                int newGCost = current.gCost + moveCost;

                if (!open.Contains(neighbor) || newGCost < neighbor.gCost)
                {
                    neighbor.gCost = newGCost;
                    neighbor.hCost = Heuristic(neighbor.cell, end);
                    neighbor.parent = current;

                    if (!open.Contains(neighbor))
                    {
                        open.Add(neighbor);
                    }
                }
            }
        }
        
        return null;
    }

    private List<Vector2Int> BuildPath(Node start, Node end)
    {
        List<Vector2Int> path = new();

        Node current = end;

        while (current != start)
        {
            path.Add(current.cell.position);
            current = current.parent;
        }

        path.Reverse();

        return path;
    }

    private List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                int checkX = node.cell.position.x + x;
                int checkY = node.cell.position.y + y;

                if (checkX < 0 || checkX >= gameController.GridWidth()) continue;
                if (checkY < 0 || checkY >= gameController.GridHeight()) continue;

                neighbors.Add(nodes[checkX, checkY]);
            }
        }
        return neighbors;
    }

    private int Heuristic(CellData start, CellData end)
    {
        int deltaX = Mathf.Abs(start.position.x - end.position.x);
        int deltaY = Mathf.Abs(start.position.y - end.position.y);

        if (deltaX > deltaY) return (14 * deltaY) + (10 * (deltaX - deltaY));

        return (14 * deltaX) + (10 * (deltaY - deltaX));
    }
}
