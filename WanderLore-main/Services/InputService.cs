using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WanderLore.Services;

public class InputService : IInputService
{
    private const float MovementSpeed = 5.0f;
    private const float WorldSize = 100 * 64;
    public Vector2 hedgehogPosition { get; set; } = new(50 * 64, 50 * 64);
    public Vector2 hedgehogVelocity { get; private set; } = Vector2.Zero;
    public Vector2 FacingDirection { get; private set; } = new(0, 1);

    public void CheckKeyboardInput(int[,] map)
    {
        KeyboardState keyboard = Keyboard.GetState();
        Vector2 movement = Vector2.Zero;
        float speed = keyboard.IsKeyDown(Keys.Space) ? MovementSpeed * 1.5f : MovementSpeed;
        if (keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up)) movement.Y -= 1;
        if (keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down)) movement.Y += 1;
        if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left)) movement.X -= 1;
        if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right)) movement.X += 1;

        if (movement != Vector2.Zero)
        {
            movement.Normalize();
            FacingDirection = movement;
            // A víz lassítja a sünt a pillanatnyi csempén.
            int tileX = Math.Clamp((int)(hedgehogPosition.X / 64), 0, map.GetLength(0) - 1);
            int tileY = Math.Clamp((int)(hedgehogPosition.Y / 64), 0, map.GetLength(1) - 1);
            if (map[tileX, tileY] == 3) speed *= 0.5f;
            movement *= speed;
        }

        hedgehogPosition = Vector2.Clamp(hedgehogPosition + movement, new Vector2(32), new Vector2(WorldSize - 32));
        hedgehogVelocity = movement;
    }

    public void ClickHandler() { }
    public void MovementHandler() { }
}
