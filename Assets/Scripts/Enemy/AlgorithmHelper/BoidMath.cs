using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 纯数学工具类：提供Boid行为的计算方法
/// 不依赖任何业务逻辑（如EnemyCore），仅处理基础数据类型
/// </summary>
public static class BoidMath
{
    // 网格相关参数（内部维护）
    private static float cellSize;
    private static Dictionary<Vector2Int, List<int>> grid = new Dictionary<Vector2Int, List<int>>();

    /// <summary>
    /// 更新空间网格（存储位置索引）
    /// </summary>
    public static void UpdateSpatialGrid(List<Vector2> positions, float radius)
    {
        grid.Clear();
        cellSize = radius;

        for (int i = 0; i < positions.Count; i++)
        {
            Vector2Int cell = GetCellPosition(positions[i]);
            if (!grid.ContainsKey(cell))
            {
                grid[cell] = new List<int>();
            }
            grid[cell].Add(i);
        }
    }

    /// <summary>
    /// 获取单元格位置
    /// </summary>
    private static Vector2Int GetCellPosition(Vector2 worldPosition)
    {
        return new Vector2Int(
            Mathf.FloorToInt(worldPosition.x / cellSize),
            Mathf.FloorToInt(worldPosition.y / cellSize)
        );
    }

    /// <summary>
    /// 查找邻居索引
    /// </summary>
    public static List<int> FindNeighborIndices(Vector2 currentPosition, List<Vector2> allPositions, float radius)
    {
        List<int> neighbors = new List<int>();
        Vector2Int currentCell = GetCellPosition(currentPosition);

        // 检查周围3x3网格
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2Int cell = new Vector2Int(currentCell.x + x, currentCell.y + y);
                if (grid.TryGetValue(cell, out var cellIndices))
                {
                    foreach (var index in cellIndices)
                    {
                        if (index >= 0 && index < allPositions.Count &&
                            Vector2.Distance(currentPosition, allPositions[index]) <= radius)
                        {
                            neighbors.Add(index);
                        }
                    }
                }
            }
        }

        return neighbors;
    }

    /// <summary>
    /// 计算聚集行为向量
    /// </summary>
    public static Vector2 CalculateCohesion(List<Vector2> neighborPositions, Vector2 currentPosition)
    {
        if (neighborPositions.Count == 0) return Vector2.zero;

        Vector2 center = Vector2.zero;
        foreach (var pos in neighborPositions)
        {
            center += pos;
        }

        return (center / neighborPositions.Count - currentPosition).normalized;
    }

    /// <summary>
    /// 计算分离行为向量
    /// </summary>
    public static Vector2 CalculateSeparation(List<Vector2> neighborPositions, Vector2 currentPosition, float minDistance)
    {
        if (neighborPositions.Count == 0) return Vector2.zero;

        Vector2 separation = Vector2.zero;
        foreach (var pos in neighborPositions)
        {
            Vector2 direction = currentPosition - pos;
            float distance = direction.magnitude;

            if (distance < minDistance && distance > 0)
            {
                separation += direction.normalized / distance;
            }
        }

        return separation.normalized;
    }

    /// <summary>
    /// 计算对齐行为向量
    /// </summary>
    public static Vector2 CalculateAlignment(List<Vector2> neighborVelocities, Vector2 currentVelocity)
    {
        if (neighborVelocities.Count == 0) return currentVelocity.normalized;

        Vector2 averageDirection = Vector2.zero;
        foreach (var vel in neighborVelocities)
        {
            averageDirection += vel.normalized;
        }

        return averageDirection.normalized;
    }
}