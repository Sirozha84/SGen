using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SGen;

namespace SGenExample
{
    class Ball : Box
    {
        //Текстура, используемая классом "Ball"
        public static Texture2D Texture;

        static Random rnd = new Random();
        int time;

        //Конструктор щарика, базовые переменные описаны
        public Ball(int x, int y) : base(x, y, 32, 32, 0, 0, false, true, true, 0.01f, 0.9f,100) { }
        public override void Update()
        {
            //Если нажат пробел, бросаемся в рандомную сторону
            if (Keyboard.GetState().IsKeyDown(Keys.Space)) Impuls(rnd.Next(360), 10);
            Physics(true);
            if (time > 0) time--;
        }
        public override void Draw()
        {
            //Рисование. При необъодимости можно дорисовывать дополнительные текстуры (оружие, полоски жизни)
            Draw(Texture);
        }
        public override void Collision(Box box) { }
        public override void Trigger()
        {
            //Персонаж приближается, бросаемся в рандомную сторону
            if (time == 0)
            {
                Impuls(rnd.Next(360), 10);
                time = 30;
            }
        }
    }
}
