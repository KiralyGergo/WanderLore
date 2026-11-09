using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WanderLore.Services
{
    public interface ICameraService
    {
        Vector2 Position { get; }
        void UpdateCameraView(GameTime gameTime, Vector2 playerPosition, Vector2 playerVelocity);
        Matrix GetViewMatrix(GraphicsDevice graphicsDevice);
    }
}