using System.Collections.Generic;
using UnityEngine;

public static class AStarPathfinder
{
    public static List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal, int maxDistance)
    {
        if (GameManager.Instance == null || GameManager.Instance.grid == null)
        {
            Debug.LogError("Pathfinding failed: GameManager or grid is null.");
            return null;
        }

        string[,] grid = GameManager.Instance.grid;
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        if (!InBounds(start, width, height) || !InBounds(goal, width, height))
        {
            Debug.LogWarning("Pathfinding aborted: start or goal out of bounds.");
            return null;
        }

        if (goal != start && !IsWalkable(grid[goal.x, goal.y]))
        {
            Debug.LogWarning("Goal is not walkable — fallback to partial path. " + goal + " Tile: " + grid[goal.x, goal.y]);
        }

        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();
        PriorityQueue<Vector2Int> openSet = new PriorityQueue<Vector2Int>();
        openSet.Enqueue(start, 0);

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, int> gScore = new Dictionary<Vector2Int, int> { [start] = 0 };

        Vector2Int bestSoFar = start;
        int bestHeuristic = Heuristic(start, goal);

        while (openSet.Count > 0)
        {
            Vector2Int current = openSet.Dequeue();

            int currentHeuristic = Heuristic(current, goal);
            if (currentHeuristic < bestHeuristic)
            {
                bestHeuristic = currentHeuristic;
                bestSoFar = current;
            }

            if (current == goal)
                return ReconstructPath(cameFrom, current);

            closedSet.Add(current);

            foreach (Vector2Int neighbor in GetNeighbors(current, width, height))
            {
                if (neighbor != start && !IsWalkable(grid[neighbor.x, neighbor.y]))
                    continue;

                if (closedSet.Contains(neighbor))
                    continue;

                int tentativeGScore = gScore[current] + 1;
                if (tentativeGScore > maxDistance)
                    continue;

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    int fScore = tentativeGScore + Heuristic(neighbor, goal);
                    openSet.Enqueue(neighbor, fScore);
                }
            }
        }

        // Return partial path to best reachable tile
        if (bestSoFar != start)
        {
            Debug.LogWarning("Returning partial path — goal was unreachable.");
            return ReconstructPath(cameFrom, bestSoFar);
        }

        return null;
    }

    private static List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        List<Vector2Int> path = new List<Vector2Int> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path;
    }

    private static IEnumerable<Vector2Int> GetNeighbors(Vector2Int pos, int width, int height)
    {
        Vector2Int[] directions = {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        foreach (var dir in directions)
        {
            Vector2Int neighbor = pos + dir;
            if (InBounds(neighbor, width, height))
                yield return neighbor;
        }
    }

    private static bool InBounds(Vector2Int pos, int width, int height)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    private static int Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); // Manhattan distance
    }

    private static bool IsWalkable(string tile)
    {
        switch (tile)
        {
            case "floor":
                return true;
            default:
                return false;
        }
    }

    private class PriorityQueue<T>
    {
        private List<(T item, int priority)> elements = new List<(T, int)>();

        public int Count => elements.Count;

        public void Enqueue(T item, int priority)
        {
            elements.Add((item, priority));
        }

        public T Dequeue()
        {
            int bestIndex = 0;
            for (int i = 1; i < elements.Count; i++)
                if (elements[i].priority < elements[bestIndex].priority)
                    bestIndex = i;

            T bestItem = elements[bestIndex].item;
            elements.RemoveAt(bestIndex);
            return bestItem;
        }
    }
}
