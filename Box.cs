﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SGen
{
    /// <summary>
    /// Родительский класс для игровых объектов,
    /// содержащий базовые методы и свойства, присущие всем элементам игры.
    /// Так же класс содержит и статичные параметры игры (ускорение свободного падения).
    /// </summary>
    /// 
    public class Box
    {

        #region Базовые константы для всех объектов, задаются при запуске игры
        /// <summary>
        /// spriteBatch
        /// </summary>
        public static SpriteBatch spriteBatch;
        /// <summary>
        /// G-константа (ускорение свободного падения)
        /// </summary>
        public static float Gravitation = 0.3F;
        /// <summary>
        /// Список блоков не пускающих по всем направлениям
        /// </summary>
        public static int[] Hards;
        /// <summary>
        /// Список блоков, не пускающих вниз (Платформы на которые можно запрыгнуть снизу)
        /// </summary>
        public static int[] HardsDown;
        /// <summary>
        /// Расстояние вылета за пределы карты, уничтожающее объект
        /// </summary>
        public static int OutLength = 10;
        #endregion

        #region Базовые константы задающиеся в конструкторе конкретного объекта
        /// <summary>
        /// Размеры спрайта (ширина)
        /// </summary>
        int Width;
        /// <summary>
        /// Размеры спрайта (высота)
        /// </summary>
        int Height;
        /// <summary>
        /// Растояние от бордюров спрайта до чувствительных зоны по сторонам
        /// </summary>
        int Side;
        /// <summary>
        /// Растояние от бордюров спрайта до чувствительной зоны сверху 
        /// </summary>
        int Top;
        /// <summary>
        /// Упирается ли объект в стены
        /// </summary>
        bool Collision;
        /// <summary>
        /// Может ли объект стоять на платформах
        /// </summary>
        bool DownIntoPlatform;
        /// <summary>
        /// Вес объекта. Определяет, будет ли объект падать, и определяет скорость замедления по X.
        /// 0 - Объект совсем не падает и не замедляется.
        /// </summary>
        float Weight;
        /// <summary>
        /// Коэффициент отскока от 0 до 1.
        /// </summary>
        float Rebound;
        #endregion

        #region Рабочие переменные
        /// <summary>
        /// Позиция объекта
        /// </summary>
        Vector2 Position;
         /// <summary>
        /// Забыл описать переменную, теперь не знаю что она делает :-( но без неё беда беда прям
        /// </summary>
        Vector2 PositionFake;
        /// <summary>
        /// Скорость перемещения
        /// </summary>
        Vector2 Speed;
        /// <summary>
        /// Максимальная скорость движения
        /// </summary>
        float MaxSpeed = 5;
        /// <summary>
        /// Максимальная скорость падения (не больше размера спрайта)
        /// </summary>
        float MaxFallSpeed = 32;
        /// <summary>
        /// Ускорение движения
        /// </summary>
        float Acceliration = 0.5f;
        /// <summary>
        /// Скорость прыжка
        /// </summary>
        float JumpSpeed = 5;
        /// <summary>
        /// Флаг движения в последний кадр
        /// </summary>
        bool ControlMove = false;
        #endregion

        /// <summary>
        /// Конструктор аморфного объекта, не имеющего параметров кроме координаты
        /// </summary>
        /// <param name="position"></param>
        public Box(Vector2 position)
        {
            Position = position;
            //Остальное и не надо
            Width = 0;
            Height = 0;
            Side = 0;
            Top = 0;
            Collision = false;
            DownIntoPlatform = false;
            Weight = 0;
            Rebound = 0;
        }       
        /// <summary>
        /// Конструктор игрового объекта, не имеющего веса и проверку на столкновения
        /// </summary>
        /// <param name = "position">Позиция объекта</param>
        /// <param name = "width">Ширина объекта</param>
        /// <param name = "height">Высота объекта</param>
        /// <param name = "side">Расстояния по бокам до чувствительных зон</param>
        /// <param name = "top">Расстояние сверху до чувствительной зоны</param>
        public Box(Vector2 position, int width, int height, int side, int top)
        {
            Position = position;
            Width = width;
            Height = height;
            Side = side;
            Top = top;
            //Остальные задаются в продвинутом конструкторе, здесь же выставляются дефолтные
            Collision = false;
            DownIntoPlatform = false;
            Weight = 0;
            Rebound = 0;
        }

        /// <summary>
        /// Конструктор игрового объекта, умеющего перемещаться в игровом мире
        /// </summary>
        /// <param name = "position">Позиция объекта</param>
        /// <param name = "width">Ширина объекта</param>
        /// <param name = "height">Высота объекта</param>
        /// <param name = "side">Расстояния по бокам до чувствительных зон</param>
        /// <param name = "top">Расстояние сверху до чувствительной зоны</param>
        /// <param name = "collision">Проверять ли на столкновения при передвижении</param>
        /// <param name = "weight">имеет ли объект вес</param>
        /// <param name = "downIntoPlatform">Проходит ли вниз через платформы</param>
        /// <param name = "rebound">Коэффициент отскока от 0 до 1</param>
        public Box(Vector2 position, int width, int height, int side, int top,
            bool collision, bool downIntoPlatform, float weight, float rebound)
        {
            Position = position;
            Width = width;
            Height = height;
            Side = side;
            Top = top;
            //Продвинутые переменные
            Collision = collision;
            DownIntoPlatform = downIntoPlatform;
            Weight = weight;
            Rebound = rebound;
        }

        /// <summary>
        /// Возвращает координаты центра объекта
        /// </summary>
        public Vector2 Center()
        {
            return new Vector2(Position.X + Width / 2, Position.Y + (Height + Top) / 2);
        }

        /// <summary>
        /// Рисование спрайта
        /// </summary>
        /// <param name = "spritebatch">SpriteBatch</param>
        /// <param name = "texture">Текстура</param>
        /// <param name = "numAn">Номер анимации</param>
        /// <param name = "numKd">Номер кадра</param>
        /// <param name = "pos">Направление (+1 - вправо, -1 - влево)</param>
        protected void Draw(Texture2D texture, int numAn, int numKd, int pos)
        {
            SpriteEffects effect = SpriteEffects.None;
            if (pos < 0) effect = SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture,
                new Rectangle((int)(Position.X - Screen.Camera.X), (int)(Position.Y - Screen.Camera.Y), Width, Height),
                new Rectangle(numKd * Width, numAn * Height, Width, Height),
                Color.White, 0, new Vector2(0, 0), effect, 0);
            
        }

        /// <summary>
        /// Движение объекта по горизонтали. Возвращает True, если движению ничего не препядствовало.
        /// </summary>
        /// <param name="Delta">Расстояние перемещения.</param>
        bool MoveX(int Delta)
        {
            //Подготавливаем начальные переменные
            if (!Collision)
            {
                Position.X += Delta;
                return true;
            }
            int x = (int)Position.X - Side + Width;
            int d = +1;
            if (Delta < 0)
            {
                x = (int)Position.X + Side - 1;
                d = -1;
                Delta = -Delta;
            }
            //Двигаемся
            for (int i = 0; i < Delta; i++)
            {

                for (int y = (int)Position.Y + Top; y < Position.Y + Height; y += Screen.BlockSize)
                {
                    if (!CheckPoint(x, y, 0)) return false;
                }
                if (!CheckPoint(x, (int)Position.Y + Height - 1, 0)) return false;
                Position.X += d;
                x += d;
            }
            return true;
        }

        /// <summary>
        /// Движение объекта по вертикали. Возвращает True, если движению ничего не препядствовало.
        /// </summary>
        /// <param name="Delta">Расстояние перемещения.</param>
        bool MoveY(int Delta)
        {
            //Подготавливаем начальные переменные
            if (!Collision)
            {
                Position.Y += Delta;
                return true;
            }
            int y = (int)Position.Y + Height;
            int d = +1;
            if (Delta < 0)
            {
                y = (int)Position.Y + Top - 1;
                d = -1;
                Delta = -Delta;
            }
            //Перемещаемся
            for (int i = 0; i < Delta; i++)
            {

                for (int x = (int)Position.X + Side; x < Position.X + Width - Side; x += Screen.BlockSize)
                {
                    if (!CheckPoint(x, y, d)) return false;
                }
                if (!CheckPoint((int)Position.X - Side + Width - 1, y, d)) return false;
                Position.Y += d;
                y += d;
            }
            return true;
        }
        /// <summary>
        /// Проверка точки на карте на проходимость.
        /// </summary>
        /// <param name="x">X-координата</param>
        /// <param name="y">Y-координата</param>
        /// <param name="dy">Дельта по вертикали (для проверки на прохождение через платформы)</param>
        bool CheckPoint(int x, int y, int dy)
        {
            //Проверим, не проверяется ли точка вне матрицы
            if (x < 0 | y < 0 | x > Screen.RightMapPixelPixel | y > Screen.BottomMapPixel)
            {
                return true;
            }
            int dot = Map.M[0, x / Screen.BlockSize, y / Screen.BlockSize];
            //Сначала проверим не преграждаем ли путь платформа
            if (DownIntoPlatform && dy > 0)
            {
                //Надо проверить не находится ли точка в самом верху(только если так - тогда платформа
                //не даёт упасть
                if ((y) % Screen.BlockSize == 0)
                {
                    //System.Windows.Forms.MessageBox.Show("Вниз");
                    foreach (int b in HardsDown)
                    {
                        if (dot == b) return false;
                    }
                }
            }
            //Платформа не помешала - двигаемся дальше
            foreach (int b in Hards)
            {
                if (dot == b) return false;
            }
            return true;
        }


        /// <summary>
        /// Команда: иди влево
        /// </summary>
        public void GoLeft()
        {
            Speed.X -= Acceliration;
            if (Speed.X < -MaxSpeed) Speed.X = -MaxSpeed;
            ControlMove = true;
        }
        /// <summary>
        /// Команда: иди вправо
        /// </summary>
        public void GoRight()
        {
            Speed.X += Acceliration;
            if (Speed.X > MaxSpeed) Speed.X = MaxSpeed;
            ControlMove = true;
        }
        /// <summary>
        /// Команда: прыгай
        /// </summary>
        public void Jump()
        {
            Speed.Y = -JumpSpeed;

        }
        /// <summary>
        /// Задание импульса с заданным углом направления и силой
        /// </summary>
        /// <param name="angle">Угол, измеряемый в радианах (с "3-х часов" против часовой)</param>
        /// <param name="stronge">Сила импульса</param>
        public void Impuls(float angle, int stronge)
        {
            Speed.X = (float)Math.Cos(angle) * stronge;
            Speed.Y = -(float)Math.Sin(angle) * stronge;
        }
        /// <summary>
        /// Задание импульса с заданным углом направления и силой
        /// </summary>
        /// <param name="angle">Укол, измеряемый в градусах (с "12-и часов" по часовой)</param>
        /// <param name="stronge">Сила импульса</param>
        public void Impuls(int angle, int stronge)
        {
            double Angle = angle / 6.2831853070f;
            Speed.X = (float)Math.Sin(Angle) * stronge;
            Speed.Y = -(float)Math.Cos(Angle) * stronge;
        }
        /// <summary>
        /// Задание импульса с явным указанием скоростей по X и Y
        /// </summary>
        /// <param name="to"></param>
        public void Impuls(Vector2 to)
        {
            Speed = to;
        }
        /// <summary>
        /// Свободное перемещение в пространстве с учётом гравитации.
        /// Вызывается каждый кадр когда объект находится в свободном падении.
        /// </summary>
        public void FreeMove()
        {
            PositionFake = Position; 
            if (Weight>0) Speed.Y += Gravitation;
            PositionFake += Speed;
            
            if (Speed.Y > MaxFallSpeed) Speed.Y = MaxFallSpeed;
            int x = (int)PositionFake.X - (int)Position.X; //Хрен знает как оно работает, вовремя не прокоментировал,
            int y = (int)PositionFake.Y - (int)Position.Y; //Теперь "это" лучше не трогать!
            //Если достигли стены, считаем уровень отскока
            if (x != 0)
            {
                if (!MoveX(x))
                {
                    Speed.X = -(int)(Speed.X * Rebound-1);
                    if (Speed.X >= -1 & Speed.X <= 1) Speed.X = 0;
                }
            }
            if (y != 0)
            {
                if (!MoveY(y))
                {
                    Speed.Y = -(int)(Speed.Y * Rebound-1);         //Намешал конечно кучу лишнего
                    if (Speed.Y >= -1 & Speed.Y <= 1) Speed.Y = Speed.Y / 2; //но без этого отскок не останавливается...
                    // (Speed.Y >= -1 & Speed.Y <= 1) Speed.Y = 0; <- а так было до того как Миша доебался ))
                }
            }
            //Замедляемся, если небыло управления
            if (!ControlMove)
            {
                if (Speed.X < 0)
                {
                    Speed.X += Weight;
                    if (Speed.X > 0) Speed.X = 0; //Чтоб замедление не перескочило 0
                }
                if (Speed.X > 0)
                {
                    Speed.X -= Weight;
                    if (Speed.X < 0) Speed.X = 0; //Чтоб замедление не перескочило 0
                }
            }
            ControlMove = false;
        }
        /// <summary>
        /// Возвращает true, если объект вылетел за карту на указанное по умолчанию расстояние
        /// </summary>
        /// <returns></returns>
        public bool Out()
        {
            return Out(OutLength);
        }
        /// <summary>
        /// Возвращает true, если объект вылетел за карту на указанное расстояние
        /// </summary>
        /// <param name="Length">Допустимое расстояние вылета</param>
        /// <returns></returns>
        public bool Out(int Length)
        {
            if (Position.X < -Length - Width) return true;
            if (Position.Y < -Length - Height) return true;
            if (Position.X > Screen.RightMapPixelPixel + Length) return true;
            if (Position.Y > Screen.BottomMapPixel + Length) return true;
            return false;
        }

        public bool TestCollision(Box CompareObject)
        {
            //BoundingBox oo1 = new BoundingBox(new Vector3(
            Rectangle o1 = new Rectangle((int)Position.X + Side, (int)Position.Y + Top, Width - Side * 2, Height - Top);
            Rectangle o2 = new Rectangle((int)CompareObject.Position.X + CompareObject.Side, (int)CompareObject.Position.Y + CompareObject.Top,
                CompareObject.Width - CompareObject.Side * 2, CompareObject.Height - CompareObject.Top);
            return o1.Intersects(o2);
            // Console.WriteLine(o1.X);
            
        }
    }
}