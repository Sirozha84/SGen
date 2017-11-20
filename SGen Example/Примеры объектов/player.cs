using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SGen;

namespace SGenExample
{
    class Player : Box
    {
        // Текстура, используемая классом "Player"
        public static Texture2D Texture;
        // Конструктор персонажа, базовые переменные описаны
        public Player(int x, int y) : base(x, y, 13, 24, 0, 0, true, true, true, 0.5f, 0, 0) { }
        public override void Update()
        {
            //Управление
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) GoLeft();
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) GoRight();
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) Jump();
            if (Keyboard.GetState().IsKeyDown(Keys.Space)) Screen.Shake(50, 3, 0.99f);

            Physics(true);
        }
        public override void Draw()
        {
            //Рисование. При необходимости можно дорисовывать дополнительные текстуры (оружие, полоски жизни)
            Draw(Texture);
        }
        public override void Collision(Box box)
        {
            //Событие при коллизии с другим объектом, в данном случае взятие шарика, после чего шарик уничтожаем
            if (box is Ball)
            box.Destroy();
        }
        public override void Trigger() { }
    }
}
