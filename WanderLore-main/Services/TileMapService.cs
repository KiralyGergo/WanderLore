using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using WanderLore.Models;

namespace WanderLore.Services;

public class TileMapService : ITileMapService
{
    private readonly TileMapModel _model;
    private readonly Random _random = new(42);
    private readonly List<Point> _fruits = new();
    private int[,] _map = new int[0, 0];

    public TileMapService(TileMapModel model) => _model = model;
    public int[,] Map => _map;
    public IReadOnlyList<Point> Fruits => _fruits;

    public void GenerateTileMap()
    {
        _map = new int[_model.TileMapWidth, _model.TileMapHeight];
        _fruits.Clear();

        // Vízfoltok: a 3-as tile-típus vízblokkot jelöl.
        for (int pond = 0; pond < 18; pond++)
        {
            int cx = _random.Next(5, _model.TileMapWidth - 5);
            int cy = _random.Next(5, _model.TileMapHeight - 5);
            int rx = _random.Next(2, 5);
            int ry = _random.Next(2, 5);
            for (int x = cx - rx; x <= cx + rx; x++)
            for (int y = cy - ry; y <= cy + ry; y++)
                if (x >= 0 && y >= 0 && x < _model.TileMapWidth && y < _model.TileMapHeight &&
                    Math.Pow((x - cx) / (double)rx, 2) + Math.Pow((y - cy) / (double)ry, 2) <= 1.2)
                    _map[x, y] = 3;
        }

        // Erdős foltok. A vízre nem kerülhet fa.
        for (int patch = 0; patch < _model.ForestNumber; patch++)
        {
            int cx = _random.Next(4, _model.TileMapWidth - 4);
            int cy = _random.Next(4, _model.TileMapHeight - 4);
            int radiusX = _random.Next(2, 7);
            int radiusY = _random.Next(2, 7);
            for (int x = cx - radiusX; x <= cx + radiusX; x++)
            for (int y = cy - radiusY; y <= cy + radiusY; y++)
                if (x >= 0 && y >= 0 && x < _model.TileMapWidth && y < _model.TileMapHeight &&
                    _map[x, y] != 3 && _random.Next(100) < 72)
                    _map[x, y] = _random.Next(1, 3);
        }

        // A kezdőterület legyen szabad, hogy a játékos ne fában vagy vízben induljon.
        for (int x = 47; x <= 53; x++)
        for (int y = 47; y <= 53; y++) _map[x, y] = 0;

        // Gyümölcsök elhelyezése járható mezőkön.
        while (_fruits.Count < 10)
        {
            int x = _random.Next(2, _model.TileMapWidth - 2);
            int y = _random.Next(2, _model.TileMapHeight - 2);
            if (_map[x, y] == 0 && Math.Abs(x - 50) + Math.Abs(y - 50) > 8 && !_fruits.Contains(new Point(x, y)))
                _fruits.Add(new Point(x, y));
        }
    }

    public void CollectFruit(Point tile) => _fruits.Remove(tile);
    public void ChangeTileMap() { }
    public void DrawTileMap() { }
}
