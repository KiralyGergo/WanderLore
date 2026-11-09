using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using WanderLore.Models;
using WanderLore.Services;

namespace WanderLore;

public class WanderLoreGame : Game
{
    private enum Scene { Menu, Playing, Paused, Won }

    private readonly GraphicsDeviceManager _graphics;
    private readonly IAtlasService _atlasService;
    private readonly IInputService _inputService;
    private readonly ICameraService _cameraService;
    private readonly ITileMapService _tileMapService;
    private SpriteBatch _spriteBatch;
    private Texture2D _textureAtlas;
    private SpriteFont _font;
    private Texture2D _pixel;
    private Song _backgroundMusic;
    private Scene _scene = Scene.Menu;
    private KeyboardState _previousKeyboard;
    private float _timeOfDay = 0.28f;
    private const int TileSize = 64;
    private const int FruitGoal = 10;
    private int _collectedFruit;

    public WanderLoreGame(IScreenService screenService, IAtlasService atlasService,
        IDrawService drawService, IInputService inputService, ICameraService cameraService,
        ITileMapService tileMapService)
    {
        _atlasService = atlasService;
        _inputService = inputService;
        _cameraService = cameraService;
        _tileMapService = tileMapService;
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.Title = "WanderLore";
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 800;
        _graphics.IsFullScreen = false;
    }

    protected override void Initialize()
    {
        base.Initialize();
        _atlasService.LoadAtlasData();
        _tileMapService.GenerateTileMap();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _textureAtlas = Content.Load<Texture2D>("texture_atlas/texture_atlas");
        _font = Content.Load<SpriteFont>("Fonts/MenuFont");
        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
        _backgroundMusic = Content.Load<Song>("music/background_music");
    }

    protected override void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();
        bool Pressed(Keys key) => keyboard.IsKeyDown(key) && _previousKeyboard.IsKeyUp(key);

        if (Pressed(Keys.Escape))
        {
            if (_scene == Scene.Playing) _scene = Scene.Paused;
            else if (_scene == Scene.Paused) _scene = Scene.Playing;
            else if (_scene == Scene.Menu || _scene == Scene.Won) Exit();
        }

        switch (_scene)
        {
            case Scene.Menu:
                if (Pressed(Keys.Enter)) StartGame();
                else if (Pressed(Keys.Q)) Exit();
                break;
            case Scene.Playing:
                if (Pressed(Keys.P)) _scene = Scene.Paused;
                else
                {
                    _inputService.CheckKeyboardInput(_tileMapService.Map);
                    _cameraService.UpdateCameraView(gameTime, _inputService.hedgehogPosition, _inputService.hedgehogVelocity);
                    CollectNearbyFruit();
                    _timeOfDay = (_timeOfDay + (float)gameTime.ElapsedGameTime.TotalSeconds / 180f) % 1f;
                }
                break;
            case Scene.Paused:
                if (Pressed(Keys.Enter) || Pressed(Keys.P)) _scene = Scene.Playing;
                else if (Pressed(Keys.M)) _scene = Scene.Menu;
                break;
            case Scene.Won:
                if (Pressed(Keys.Enter)) StartGame();
                else if (Pressed(Keys.M)) _scene = Scene.Menu;
                break;
        }

        _previousKeyboard = keyboard;
        base.Update(gameTime);
    }

    private void StartGame()
    {
        _scene = Scene.Playing;
        _inputService.hedgehogPosition = new Vector2(50 * TileSize, 50 * TileSize);
        _collectedFruit = 0;
        _tileMapService.GenerateTileMap();
        MediaPlayer.IsRepeating = true;
        if (MediaPlayer.State != MediaState.Playing) MediaPlayer.Play(_backgroundMusic);
    }

    private void CollectNearbyFruit()
    {
        int tx = (int)(_inputService.hedgehogPosition.X / TileSize);
        int ty = (int)(_inputService.hedgehogPosition.Y / TileSize);
        foreach (var fruit in new System.Collections.Generic.List<Point>(_tileMapService.Fruits))
        {
            if (Math.Abs(fruit.X - tx) <= 0 && Math.Abs(fruit.Y - ty) <= 0)
            {
                _tileMapService.CollectFruit(fruit);
                _collectedFruit++;
            }
        }
        if (_collectedFruit >= FruitGoal) _scene = Scene.Won;
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(18, 27, 34));
        bool worldScene = _scene == Scene.Playing || _scene == Scene.Paused || _scene == Scene.Won;
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp,
            transformMatrix: worldScene ? _cameraService.GetViewMatrix(GraphicsDevice) : Matrix.Identity);

        if (worldScene)
        {
            DrawWorld();
            DrawDayNightOverlay();
        }
        _spriteBatch.End();

        if (_scene == Scene.Menu) DrawMenu();
        else if (_scene == Scene.Paused) DrawOverlay("SZÜNET", "ENTER / P - FOLYTATÁS", "M - FŐMENÜ", "ESC - FOLYTATÁS");
        else if (_scene == Scene.Won) DrawOverlay("GYŐZELEM!", "Összegyűjtötted az összes gyümölcsöt!", "ENTER - ÚJ JÁTÉK", "M - FŐMENÜ");
        base.Draw(gameTime);
    }

    // Vulkanon a SpriteBatch saját textúra-samplerével dolgozunk.
    // A képernyőre rajzolt áttetsző réteg adja a napszak színvilágát,
    // így nem kell külön Effect samplert hozzárendelni a SpriteBatch-hez.
    private void DrawDayNightOverlay()
    {
        float phase = _timeOfDay * MathF.Tau;
        float daylight = MathHelper.Clamp(MathF.Sin(phase - MathF.PI / 2f) * 0.5f + 0.5f, 0f, 1f);
        float twilight = 1f - MathHelper.Clamp(MathF.Abs(daylight - 0.5f) * 3f, 0f, 1f);
        float nightAmount = (1f - daylight) * 0.38f;
        float sunsetAmount = twilight * 0.18f;
        var night = new Color(28, 43, 105, (int)(nightAmount * 255));
        var sunset = new Color(255, 125, 65, (int)(sunsetAmount * 255));
        _spriteBatch.Draw(_pixel, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), night);
        _spriteBatch.Draw(_pixel, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), sunset);
    }

    private void DrawWorld()
    {
        var map = _tileMapService.Map;
        var camera = _cameraService.Position;
        int halfW = GraphicsDevice.Viewport.Width / 2;
        int halfH = GraphicsDevice.Viewport.Height / 2;
        int minX = Math.Max(0, (int)(camera.X - halfW) / TileSize - 2);
        int maxX = Math.Min(map.GetLength(0) - 1, (int)(camera.X + halfW) / TileSize + 2);
        int minY = Math.Max(0, (int)(camera.Y - halfH) / TileSize - 2);
        int maxY = Math.Min(map.GetLength(1) - 1, (int)(camera.Y + halfH) / TileSize + 2);

        // 1. réteg: fű és víz.
        for (int x = minX; x <= maxX; x++)
        for (int y = minY; y <= maxY; y++)
        {
            Rectangle ground = map[x, y] == 3
                ? _atlasService[RegionModel.Water1]
                : _atlasService[(RegionModel)((int)RegionModel.Grass1 + ((x * 7 + y * 3) % 10))];
            _spriteBatch.Draw(_textureAtlas, new Vector2(x * TileSize, y * TileSize), ground, Color.White);
        }

        // 2. réteg: gyümölcsök és a süni.
        var fruitSource = _atlasService[RegionModel.Fruits];
        foreach (var fruit in _tileMapService.Fruits)
            if (fruit.X >= minX && fruit.X <= maxX && fruit.Y >= minY && fruit.Y <= maxY)
                _spriteBatch.Draw(_textureAtlas, new Vector2(fruit.X * TileSize, fruit.Y * TileSize), fruitSource, Color.White);

        var hedgehog = _atlasService[RegionModel.Hedgehog];
        var p = _inputService.hedgehogPosition;
        float rotation = MathF.Atan2(_inputService.FacingDirection.Y, _inputService.FacingDirection.X) - MathF.PI / 2f;
        _spriteBatch.Draw(_textureAtlas, p, hedgehog, Color.White, rotation,
            new Vector2(hedgehog.Width / 2f, hedgehog.Height / 2f), 1.5f,
            SpriteEffects.None, 0f);

        // 3. réteg: a lombkoronák a szereplő elé kerülnek.
        for (int x = minX; x <= maxX; x++)
        for (int y = minY; y <= maxY; y++)
        {
            int objectType = map[x, y];
            if (objectType == 1 || objectType == 2)
            {
                var tree = _atlasService[objectType == 1 ? RegionModel.Trees1 : RegionModel.Trees2];
                _spriteBatch.Draw(_textureAtlas, new Vector2(x * TileSize, y * TileSize), tree, Color.White);
            }
        }

        DrawHud();
    }

    private void DrawHud()
    {
        string timeLabel = _timeOfDay < 0.2f || _timeOfDay > 0.8f ? "ÉJSZAKA" :
            (_timeOfDay < 0.3f || _timeOfDay > 0.7f ? "NAPKELTE / NAPLEMENTE" : "NAPPAL");
        _spriteBatch.DrawString(_font, $"Gyümölcs: {_collectedFruit}/{FruitGoal}   |   {timeLabel}   |   P: szünet", 
            new Vector2(_cameraService.Position.X - GraphicsDevice.Viewport.Width / 2f + 20,
                        _cameraService.Position.Y - GraphicsDevice.Viewport.Height / 2f + 18), Color.White);
    }

    private void DrawMenu()
    {
        // A menü saját SpriteBatch szakaszt kap: a DrawPanel és DrawTextCentered
        // csak Begin/End között hívható meg.
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        var center = new Vector2(GraphicsDevice.Viewport.Width / 2f, GraphicsDevice.Viewport.Height / 2f);
        DrawPanel(new Rectangle((int)center.X - 260, (int)center.Y - 170, 520, 340), new Color(22, 48, 38));
        DrawTextCentered("WANDERLORE", center.X, center.Y - 105, 1.7f, Color.White);
        DrawTextCentered("Egy kis kaland egy nagy erdőben", center.X, center.Y - 35, 1f, new Color(205, 225, 205));
        DrawTextCentered("ENTER  -  JÁTÉK INDÍTÁSA", center.X, center.Y + 55, 1.1f, Color.LightGreen);
        DrawTextCentered("Q / ESC  -  KILÉPÉS", center.X, center.Y + 105, 0.8f, Color.Silver);
        _spriteBatch.End();
    }

    private void DrawOverlay(string title, string line1, string line2, string line3)
    {
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        var center = new Vector2(GraphicsDevice.Viewport.Width / 2f, GraphicsDevice.Viewport.Height / 2f);
        DrawPanel(new Rectangle((int)center.X - 290, (int)center.Y - 150, 580, 300), new Color(15, 30, 25, 235));
        DrawTextCentered(title, center.X, center.Y - 90, 1.5f, Color.White);
        DrawTextCentered(line1, center.X, center.Y - 15, 0.9f, Color.LightGreen);
        DrawTextCentered(line2, center.X, center.Y + 45, 0.9f, Color.White);
        DrawTextCentered(line3, center.X, center.Y + 95, 0.75f, Color.Silver);
        _spriteBatch.End();
    }

    private void DrawTextCentered(string text, float x, float y, float scale, Color color)
    {
        var size = _font.MeasureString(text) * scale;
        _spriteBatch.DrawString(_font, text, new Vector2(x - size.X / 2f, y), color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
    }

    private void DrawPanel(Rectangle rectangle, Color color) => _spriteBatch.Draw(_pixel, rectangle, color);
}
