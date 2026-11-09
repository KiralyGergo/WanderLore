using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using WanderLore.Models;

namespace WanderLore.Services;

class AtlasService : IAtlasService
{
    private List<AtlasModel> _atlasList = [];
    private Dictionary<RegionModel, Rectangle> _atlasDictionary = [];

    public void LoadAtlasData()
    {
        string _atlasPath = "Content/texture_atlas/texture_atlas.json";
        if (!File.Exists(_atlasPath))
        {
            throw new FileNotFoundException($"The atlas file '{_atlasPath}' was not found.");
        }

        string _atlasData = File.ReadAllText(_atlasPath);
        if (string.IsNullOrWhiteSpace(_atlasData))
        {
            throw new InvalidDataException($"The atlas file '{_atlasPath}' is empty or contains only whitespace.");
        }

        _atlasList = JsonSerializer.Deserialize<List<AtlasModel>>(_atlasData);

        ManageAtlasData();

    }

    public void ManageAtlasData()
    {
        _atlasDictionary.Clear();
        foreach (var atlas in _atlasList)
        {
            if (Enum.TryParse<RegionModel>(atlas.Name, true, out var regionEnum))
            {
                Rectangle _sourceRectangle = new(atlas.X, atlas.Y, atlas.Width, atlas.Height);
                _atlasDictionary[regionEnum] = _sourceRectangle;
            }
        }
    }

    public Rectangle this[RegionModel region]
    {
        get
        {
            if (_atlasDictionary.TryGetValue(region, out var rectangle))
            {
                return rectangle;
            }
            throw new KeyNotFoundException($"A(z) '{region}' nevű atlasz elem nem található a szótárban!");
        }
    }

}