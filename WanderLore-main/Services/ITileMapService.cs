using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace WanderLore.Services;

public interface ITileMapService
{
    int[,] Map { get; }
    IReadOnlyList<Point> Fruits { get; }
    void GenerateTileMap();
    void CollectFruit(Point tile);
    void ChangeTileMap();
    void DrawTileMap();
}
