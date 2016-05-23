using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace SGen
{
    /// <summary>
    /// Родительский класс для игровых объектов,
    /// содержащий базовые методы и свойства, присущие всем элементам игры.
    /// Так же класс содержит и статичные параметры игры (ускорение свободного падения).
    /// </summary>
    public abstract class Box
    {
        #region Базовые константы для всех объектов, задаются при запуске игры
        /// <summary>
        /// spriteBatch
        /// </summary>
        public static SpriteBatch spriteBatch;
        /// <summary>
        /// Текущий "скрин", в котором рисуется сцена.
        /// </summary>
        public static Screen screen;
        /// <summary>
        /// G-константа (ускорение свободного падения)
        /// </summary>
        public static float Gravitation = 0.3F;
        /// <summary>
        /// Список блоков не пускающих по всем направлениям
        /// </summary>
        public static int[] Grounds = new int[0];
        /// <summary>
        /// Список блоков, не пускающих вниз (Платформы на которые можно запрыгнуть снизу)
        /// </summary>
        public static int[] Platforms = new int[0];
        /// <summary>
        /// Расстояние вылета за пределы карты, уничтожающее объект
        /// </summary>
        public static int OutLength = 10;
        #endregion

        #region Базовые константы задающиеся в конструкторе конкретного объекта
        /// <summary>
        /// Размеры спрайта (ширина)
        /// </summary>
        public int Width;
        /// <summary>
        /// Размеры спрайта (высота)
        /// </summary>
        public int Height;
        /// <summary>
        /// Растояние от бордюров спрайта до чувствительных зоны по сторонам
        /// </summary>
        public int SpaceSide;
        /// <summary>
        /// Растояние от бордюров спрайта до чувствительной зоны сверху 
        /// </summary>
        public int SpaceTop;
        /// <summary>
        /// Упирается ли объект в стены
        /// </summary>
        public bool Hard;
        /// <summary>
        /// Может ли объект стоять на платформах
        /// </summary>
        protected bool DownIntoPlatform;
        /// <summary>
        /// Вес объекта. Определяет, будет ли объект падать, и определяет скорость замедления по X.
        /// 0 - Объект совсем не падает и не замедляется.
        /// </summary>
        float Weight;
        /// <summary>
        /// Коэффициент отскока от 0 до 1.
        /// </summary>
        float Rebound;
        /// <summary>
        /// Объект следит за столкновениями
        /// </summary>
        public bool CollisionTests;
        /// <summary>
        /// Реакция на приближение, 0 - если объект не реагирует
        /// </summary>
        public int TriggerDistance;
        /// <summary>
        /// Список объектов, проверяеммых на приближение
        /// </summary>
        public List<Box> ObjectsForTriggers;
        #endregion

        #region Рабочие переменные
        /// <summary>
        /// Позиция объекта
        /// </summary>
        public Vector2 Position;
        /// <summary>
        /// Позиция объекта, округляемая для рисования (не настоящая)
        /// </summary>
        Vector2 PositionFake;
        /// <summary>
        /// Скорость перемещения
        /// </summary>
        Vector2 Speed;
        /// <summary>
        /// Максимальная скорость движения
        /// </summary>
        protected float MaxSpeed = 5;
        /// <summary>
        /// Максимальная скорость падения (не больше размера спрайта)
        /// </summary>
        protected float MaxFallSpeed = 32;
        /// <summary>
        /// Ускорение движения
        /// </summary>
        protected float Acceliration = 0.5f;
        /// <summary>
        /// Скорость прыжка
        /// </summary>
        protected float JumpSpeed = 5;
        /// <summary>
        /// Флаг движения в последний кадр
        /// </summary>
        bool ControlMove = false;
        /// <summary>
        /// True, если объект надо уничтожить
        /// </summary>
        public bool Destroyed = false;
        /// <summary>
        /// Номер анимации
        /// </summary>
        protected int AnimationSet = 0;
        /// <summary>
        /// Номер кадра
        /// </summary>
        protected int AnimationFrame = 0;
        /// <summary>
        /// Направление (больше 0 - вправо, меньше 0 - влево)
        /// </summary>
        protected int AnimationSide = 0;
        /// <summary>
        /// Рисование объекта вверх ногами
        /// </summary>
        protected bool UpsideDown = false;
        /// <summary>
        /// Падаем?
        /// </summary>
        public bool Fall = false;
        /// <summary>
        /// Рандомайзер
        /// </summary>
        protected static Random RND = new Random();
        #endregion

        #region Абстрактные методы
        /// <summary>
        /// Обновление объекта
        /// </summary>
        /// <returns></returns>
        public abstract void Update();

        /// <summary>
        /// Рисование объекта
        /// </summary>
        public abstract void Draw();

        /// <summary>
        /// Действие при коллизиях с указанным объекта
        /// </summary>
        /// <param name="box">Объект, с которым произошло столкновение</param>
        public abstract void Collision(Box box);

        /// <summary>
        /// Сработал триггер
        /// </summary>
        public abstract void Trigger();
        #endregion

        /// <summary>
        /// Конструктор аморфного объекта, с размерами тайла
        /// </summary>
        /// <param name = "x">X-координата</param>
        /// <param name = "y">Y-координата</param>
        public Box(int x, int y)
        {
            Position = new Vector2(x, y);
            //Остальное по умолчанию
            Width = Screen.TileSize;
            Height = Screen.TileSize;
            SpaceSide = 0;
            SpaceTop = 0;
            CollisionTests = false;
            Hard = false;
            DownIntoPlatform = false;
            Weight = 0;
            Rebound = 0;
            TriggerDistance = 0;
        }

        /// <summary>
        /// Конструктор аморфного объекта, с выборочным размером
        /// </summary>
        /// <param name = "x">X-координата</param>
        /// <param name = "y">Y-координата</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Box(int x, int y, int width, int height)
        {
            Position = new Vector2(x, y);
            Width = width;
            Height = height;
            //Остальное по умолчанию
            SpaceSide = 0;
            SpaceTop = 0;
            CollisionTests = false;
            Hard = false;
            DownIntoPlatform = false;
            Weight = 0;
            Rebound = 0;
            TriggerDistance = 0;
        }

        /// <summary>
        /// Конструктор игрового объекта, не имеющего веса и проверку на столкновения
        /// </summary>
        /// <param name = "x">X-координата</param>
        /// <param name = "y">Y-координата</param>
        /// <param name = "width">Ширина объекта</param>
        /// <param name = "height">Высота объекта</param>
        /// <param name = "side">Расстояния по бокам до чувствительных зон</param>
        /// <param name = "top">Расстояние сверху до чувствительной зоны</param>
        /// <param name = "collision">Реагирует ли объект на прикосновения других</param>
        public Box(int x, int y, int width, int height, int side, int top, bool collision)
        {
            Position = new Vector2(x, y);
            Width = width;
            Height = height;
            SpaceSide = side;
            SpaceTop = top;
            CollisionTests = collision;
            //Остальное по умолчанию
            Hard = false;
            DownIntoPlatform = false;
            Weight = 1;
            Rebound = 0;
            TriggerDistance = 0;
        }

        /// <summary>
        /// Конструктор игрового объекта, умеющего перемещаться в игровом мире
        /// </summary>
        /// <param name = "x">X-координата</param>
        /// <param name = "y">Y-координата</param>
        /// <param name = "width">Ширина объекта</param>
        /// <param name = "height">Высота объекта</param>
        /// <param name = "side">Расстояния по бокам до чувствительных зон</param>
        /// <param name = "top">Расстояние сверху до чувствительной зоны</param>
        /// <param name = "collision">Реагирует ли объект на прикосновения других</param>
        /// <param name = "hard">Проверять ли на столкновения</param>
        /// <param name = "weight">Вес объекта, 0 - если невесомый</param>
        /// <param name = "downIntoPlatform">Проходит ли вниз через платформы</param>
        /// <param name = "rebound">Коэффициент отскока от 0 до 1</param>
        /// <param name = "triggerDistance">Дистанция триггера. 0 - объект не реагирует на приближения</param>
        public Box(int x, int y, int width, int height, int side, int top, bool collision,
            bool hard, bool downIntoPlatform, float weight, float rebound, int triggerDistance)
        {
            Position = new Vector2(x, y);
            Width = width;
            Height = height;
            SpaceSide = side;
            SpaceTop = top;
            CollisionTests = collision;
            Hard = hard;
            DownIntoPlatform = downIntoPlatform;
            Weight = weight;
            Rebound = rebound;
            TriggerDistance = triggerDistance;
        }

        /// <summary>
        /// Возвращает координаты центра объекта
        /// </summary>
        public Vector2 Center()
        {
            return new Vector2(Position.X + Width / 2, Position.Y + (Height + SpaceTop) / 2);
        }

        /// <summary>
        /// Рисование главного спрайта
        /// </summary>
        /// <param name = "texture">Текстура</param>
        public void Draw(Texture2D texture) //(Texture2D texture, int numAn, int numKd, int pos, Screen screen)
        {
            Draw(texture, Color.White);
        }

        /// <summary>
        /// Рисование главного спрайта с указанным цветом
        /// </summary>
        /// <param name = "texture">Текстура</param>
        /// <param name="col">Цвет</param>
        public void Draw(Texture2D texture, Color col)
        {
            SpriteEffects effect = SpriteEffects.None;
            if (AnimationSide < 0) effect = SpriteEffects.FlipHorizontally;
            if (UpsideDown) effect = SpriteEffects.FlipVertically;
            spriteBatch.Draw(texture,
                new Rectangle(
                    (int)(Position.X / Screen.PixelSize) * Screen.PixelSize - (int)(screen.Camera.X / Screen.PixelSize) * Screen.PixelSize,
                    (int)(Position.Y / Screen.PixelSize) * Screen.PixelSize - (int)(screen.Camera.Y / Screen.PixelSize) * Screen.PixelSize,
                    Width, Height),
                new Rectangle(AnimationFrame * Width, AnimationSet * Height, Width, Height),
                col, 0, new Vector2(0, 0), effect, 0);
        }

        /// <summary>
        /// Рисование дополнительного спрайта
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="screen"></param>
        /// <param name="x">Смещение по X</param>
        /// <param name="y">Смещение по Y</param>
        /// <param name="source">Область спрайта</param>
        public void Draw(Texture2D texture, int x, int y, Rectangle source)
        {
            SpriteEffects effect = SpriteEffects.None;
            if (AnimationSide < 0) effect = SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture,
                new Rectangle(
                    (int)(Position.X / Screen.PixelSize) * Screen.PixelSize - (int)(screen.Camera.X / Screen.PixelSize) * Screen.PixelSize + x,
                    (int)(Position.Y / Screen.PixelSize) * Screen.PixelSize - (int)(screen.Camera.Y / Screen.PixelSize) * Screen.PixelSize + y,
                    source.Width, source.Height),
                source, Color.White, 0, new Vector2(0, 0), effect, 0);
        }

        /// <summary>
        /// Рисование дополнительного спрайта
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="screen"></param>
        /// <param name="dist">Область спрайта</param>
        /// <param name="source">Область источника</param>
        public void Draw(Texture2D texture, Rectangle dist, Rectangle source)
        {
            SpriteEffects effect = SpriteEffects.None;
            if (AnimationSide < 0) effect = SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture,
                new Rectangle(
                    (int)(Position.X / Screen.PixelSize) * Screen.PixelSize - (int)(screen.Camera.X / Screen.PixelSize) * Screen.PixelSize + dist.X,
                    (int)(Position.Y / Screen.PixelSize) * Screen.PixelSize - (int)(screen.Camera.Y / Screen.PixelSize) * Screen.PixelSize + dist.Y,
                    dist.Width, dist.Height),
                source, Color.White, 0, new Vector2(0, 0), effect, 0);
        }

        /// <summary>
        /// Движение объекта по горизонтали. Возвращает True, если движению ничего не препядствовало.
        /// </summary>
        /// <param name="Delta">Расстояние перемещения.</param>
        public bool MoveX(int Delta)
        {
            //Подготавливаем начальные переменные
            if (!Hard)
            {
                Position.X += Delta;
                return true;
            }
            int x = (int)Position.X - SpaceSide + Width;
            int d = +1;
            if (Delta < 0)
            {
                x = (int)Position.X + SpaceSide - 1;
                d = -1;
                Delta = -Delta;
            }
            //Двигаемся
            for (int i = 0; i < Delta; i++)
            {

                for (int y = (int)Position.Y + SpaceTop; y < Position.Y + Height; y += Screen.TileSize)
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
        public bool MoveY(int Delta)
        {
            //Подготавливаем начальные переменные
            if (!Hard)
            {
                Position.Y += Delta;
                return true;
            }
            int y = (int)Position.Y + Height;
            int d = +1;
            if (Delta < 0)
            {
                y = (int)Position.Y + SpaceTop - 1;
                d = -1;
                Delta = -Delta;
            }
            //Перемещаемся
            for (int i = 0; i < Delta; i++)
            {

                for (int x = (int)Position.X + SpaceSide; x < Position.X + Width - SpaceSide; x += Screen.TileSize)
                {
                    if (!CheckPoint(x, y, d)) return false;
                }
                if (!CheckPoint((int)Position.X - SpaceSide + Width - 1, y, d)) return false;
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
            if (x < 0 | y < 0 | x > Screen.RightMapPixelPixel | y > Screen.BottomMapPixel) return true;
            int dot = World.M[0, x / Screen.TileSize, y / Screen.TileSize];
            //Сначала проверим не преграждаем ли путь платформа
            if (DownIntoPlatform && dy > 0)
            {
                //Надо проверить не находится ли точка в самом верху
                //(только если так - тогда платформане даёт упасть)
                if ((y) % Screen.TileSize == 0) foreach (int b in Platforms) if (dot == b) return false;
            }
            //Платформа не помешала - двигаемся дальше
            foreach (int b in Grounds) if (dot == b) return false;
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
        /// Команда: прыгай с указанной скоростью
        /// </summary>
        public void Jump(float jumpSpeed)
        {
            Speed.Y = -jumpSpeed;
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
        /// Обработка физики.
        /// Вызывается каждый кадр когда объект находится в свободном падении.
        /// </summary>
        // <param name="Gravity">Учитывается ли гравитация</param>
        /// <returns>Возвращает true, если объект стоит на земле</returns>
        public bool Physics(bool Gravity)
        {
            PositionFake = Position;
            if (!Gravity) Speed.Y = 0;
            if (Gravity && Weight > 0) Speed.Y += Gravitation;
            PositionFake += Speed;
            
            if (Speed.Y > MaxFallSpeed) Speed.Y = MaxFallSpeed;
            int x = (int)PositionFake.X - (int)Position.X; //Количество пикселей, которое надо пройти за этот кадр
            int y = (int)PositionFake.Y - (int)Position.Y;
            //Падаем?
            Fall = false;
            //Отскок от стен
            if (x != 0)
            {
                if (!MoveX(x))
                {
                    Speed.X = -(int)(Speed.X * Rebound-1);
                    if (Speed.X >= -1 & Speed.X <= 1) Speed.X = 0;
                }
            }
            //Отскок от пола или потолка
            bool Ground = false;
            if (y != 0)
            {
                if (!MoveY(y))
                {
                    Speed.Y = -(int)(Speed.Y * Rebound - 1);
                    if (Speed.Y >= -1 & Speed.Y <= 1) Speed.Y = Speed.Y / 2;
                    if (y > 0) Ground = true;
                }
                else
                {
                    if (Speed.Y>0 & Gravity) Fall = true;
                }
            }
            //Замедляемся, если небыло управления
            if (!ControlMove)
            {
                if (Speed.X < 0)
                {
                    Speed.X += Weight;
                    if (Speed.X > 0) Speed.X = 0;
                }
                if (Speed.X > 0)
                {
                    Speed.X -= Weight;
                    if (Speed.X < 0) Speed.X = 0;
                }
            }
            ControlMove = false;
            return Ground;
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

        /// <summary>
        /// Проверка на коллиюию с указанным объектом
        /// </summary>
        /// <param name="CompareObject">Проверяемый объект</param>
        /// <returns></returns>
        public void CollisionTest(Box CompareObject)
        {
            if (this == CompareObject | !CollisionTests) return;
            Rectangle o1 = new Rectangle((int)Position.X + SpaceSide, (int)Position.Y + SpaceTop, Width - SpaceSide * 2, Height - SpaceTop);
            Rectangle o2 = new Rectangle((int)CompareObject.Position.X + CompareObject.SpaceSide, (int)CompareObject.Position.Y + CompareObject.SpaceTop,
                CompareObject.Width - CompareObject.SpaceSide * 2, CompareObject.Height - CompareObject.SpaceTop);
            if (o1.Intersects(o2)) Collision(CompareObject);
        }

        /// <summary>
        /// Проверка на приближенность к указанному объекту
        /// </summary>
        /// <param name="CompareObject"></param>
        public void TriggerTest()
        {
            World.Players.ForEach(o =>
            {
                double dist = Math.Sqrt(Math.Pow((o.Position.X + o.Width / 2) - (Position.X + Width / 2), 2) +
                    Math.Pow((o.Position.Y + o.SpaceTop + (o.Height - o.SpaceTop) / 2) - (Position.Y + SpaceTop + (Height - SpaceTop) / 2), 2));
                if (dist < TriggerDistance) Trigger();
            });
        }

        /// <summary>
        /// Проверка не находится ли объект под фантомным тайлом
        /// </summary>
        /// <returns></returns>
        public bool UnderPhantom()
        {
            if (World.Phantom == 0) return false;
            int x = (int)(Position.X + Width / 2) / Screen.TileSize;
            int y = (int)(Position.Y + SpaceTop + (Height - SpaceTop) / 2) / Screen.TileSize;
            if (InMatrix(x, y))
                return World.M[World.Phantom, x, y] > 0;
            else
                return false;
        }

        /// <summary>
        /// Пометить объект на уничтожение
        /// </summary>
        public void Destroy()
        {
            Destroyed = true;
        }

        /// <summary>
        /// Проверка точки на наличие стены
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected bool Ground(int x, int y, bool andPlatforms)
        {
            //Сделать проверку на то что точка в матрице
            bool ground = false;
            for (int i = 0; i < Grounds.Length; i++)
                if (World.M[0, ((int)Position.X + x) / Screen.TileSize, ((int)Position.Y + y) / Screen.TileSize] == Grounds[i]) ground = true;
            return ground;
        }

        /// <summary>
        /// Что за тайл находится в этой точке?
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected int WhatIsTiler(int x, int y)
        {
            //Сделать проверку на то что точка в матрице
            x = (x + (int)Position.X) / Screen.TileSize;
            y = (y + (int)Position.Y) / Screen.TileSize;
            if (InMatrix(x, y))
                return World.M[0, x, y];
            else
                return 0;
        }

        protected bool InMatrix(int x, int y)
        {
            return x >= 0 & y >= 0 & x < World.Width & y < World.Height;
        }

        /// <summary>
        /// Находится ли объект в зоне видимости?
        /// </summary>
        /// <returns></returns>
        protected bool Visible()
        {
            bool visible = true;
            if (Position.X + Width < screen.Camera.X) visible = false;
            if (Position.Y + Height < screen.Camera.Y) visible = false;
            if (Position.X > screen.Camera.X + screen.viewport.Width) visible = false;
            if (Position.Y > screen.Camera.Y + screen.viewport.Height) visible = false;
            return visible;
        }

        /// <summary>
        /// Определение стороны, с которой играет звук
        /// </summary>
        /// <returns></returns>
        protected float Pan()
        {
            if (Position.X + Width < screen.Camera.X) return -1;
            if (Position.Y + Height < screen.Camera.Y) return 1;
            return 0;
        }

        /// <summary>
        /// Воспроизведение звука, если объект видно (если не видно, он играет тише с той стороны, где он находится)
        /// </summary>
        /// <param name="snd">Звук</param>
        /// <param name="Tiho">true - если играть тихо с нужной стороны, false - если совсем не играть когда не в кадре</param>
        protected void PlayIfVisible(SoundEffect snd, bool Tiho)
        {
            if (Visible()) snd.Play();
            else if (Tiho) snd.Play(0.3f, 0, Pan());
        }

        /// <summary>
        /// Сброс ускорений и полная остановка
        /// </summary>
        protected void SpeedReset()
        {
            Speed.X = 0;
            Speed.Y = 0;
            Fall = false;
        }
    }
}