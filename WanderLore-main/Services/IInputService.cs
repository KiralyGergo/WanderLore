using Microsoft.Xna.Framework;

public interface IInputService
{
    void ClickHandler();
    void MovementHandler();
    void CheckKeyboardInput(int[,] map);
    Vector2 hedgehogPosition { get; set; }
    Vector2 hedgehogVelocity { get; }
    Vector2 FacingDirection { get; }
}
