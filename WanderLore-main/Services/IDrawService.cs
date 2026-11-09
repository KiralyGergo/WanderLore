using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WanderLore.Services;

public interface IDrawService
{
    void DrawAtlasItems(SpriteBatch spriteBatch, Texture2D textureAtlas, Viewport viewport , Vector2 position);
}