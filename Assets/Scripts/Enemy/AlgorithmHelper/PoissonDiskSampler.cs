using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 通用 Poisson Disk 采样器：支持传入不同的 IsValid 检查逻辑
/// 可用于敌人组生成、组内敌人生成等场景
/// </summary>
public static class PoissonDiskSampler
{
    public static List<Vector2> GeneratePoints(
        float radius,
        Vector2 regionSize,
        Func<Vector2, bool> isValid,
        int numSamplesBeforeRejection = 30)
    {
        float cellSize = radius / Mathf.Sqrt(2);// 网格单元尺寸
        int[,] grid = new int[Mathf.CeilToInt(regionSize.x / cellSize), Mathf.CeilToInt(regionSize.y / cellSize)];

        List<Vector2> points = new List<Vector2>();//用于存储最终有效的采样点
        List<Vector2> spawnPoints = new List<Vector2>();//存储待处理的 “生成点”，算法会从这些点周围尝试生成新的有效点

        spawnPoints.Add(regionSize / 2); // 从中心开始采样

        while (spawnPoints.Count > 0)
        {
            int spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
            Vector2 spawnCenter = spawnPoints[spawnIndex];//随机选一个点做生成中心
            bool candidateAccepted = false;//是否成功生成一个有效的新点

            for (int i = 0; i < numSamplesBeforeRejection; i++)
            {
                float angle = UnityEngine.Random.value * Mathf.PI * 2;
                Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                float distance = UnityEngine.Random.Range(radius, 2 * radius);
                Vector2 candidate = spawnCenter + dir * distance;

                if (isValid(candidate) && IsFarEnough(candidate, radius, cellSize, points, grid))
                {
                    points.Add(candidate);
                    spawnPoints.Add(candidate);
                    int cellX = (int)(candidate.x / cellSize);
                    int cellY = (int)(candidate.y / cellSize);
                    grid[cellX, cellY] = points.Count;// 记录点索引
                    candidateAccepted = true;
                    break;
                }
            }

            if (!candidateAccepted)
            {
                spawnPoints.RemoveAt(spawnIndex);
            }
        }

        return points;
    }

    // 邻域检查：是否与已有点保持足够间距
    private static bool IsFarEnough(Vector2 candidate, float radius, float cellSize, List<Vector2> points, int[,] grid)
    {
        int cellX = (int)(candidate.x / cellSize);
        int cellY = (int)(candidate.y / cellSize);

        int searchRadius = 2;// 检查附近 5x5 网格单元
        for (int x = Mathf.Max(0, cellX - searchRadius); x < Mathf.Min(grid.GetLength(0), cellX + searchRadius); x++)
        {
            for (int y = Mathf.Max(0, cellY - searchRadius); y < Mathf.Min(grid.GetLength(1), cellY + searchRadius); y++)
            {
                int pointIndex = grid[x, y] - 1;
                if (pointIndex >= 0 && pointIndex < points.Count)
                {
                    if ((candidate - points[pointIndex]).sqrMagnitude < radius * radius)
                        return false;
                }
            }
        }

        return true;
    }
}
