using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WanderLore.Services;

class ScreenService : IScreenService
{
    public void SetFullScreen(GraphicsDeviceManager graphics)
    {
        var display = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
        graphics.PreferredBackBufferWidth = display.Width;
        graphics.PreferredBackBufferHeight = display.Height;
        graphics.IsFullScreen = true;
        graphics.ApplyChanges();
    }
}
