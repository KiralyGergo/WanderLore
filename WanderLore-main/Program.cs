using Microsoft.Extensions.DependencyInjection;
using WanderLore;
using WanderLore.Services;

var services = new ServiceCollection();
services.AddSingleton<IScreenService, ScreenService>();
services.AddSingleton<IAtlasService, AtlasService>();
services.AddSingleton<IDrawService, DrawService>();
services.AddSingleton<IInputService, InputService>();
services.AddSingleton<ICameraService, CameraService>();
services.AddSingleton<WanderLore.Models.TileMapModel>();
services.AddSingleton<ITileMapService, TileMapService>();
services.AddSingleton<WanderLoreGame>();
var serviceProvider = services.BuildServiceProvider();

using var game = serviceProvider.GetRequiredService<WanderLoreGame>();
game.Run();