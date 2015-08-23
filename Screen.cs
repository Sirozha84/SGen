using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SGen
{
    /// <summary>
    /// Класс для работы с экраном
    /// </summary>
    public class Screen
    {
        #region Глобальные константы
        /// <summary>
        /// Режим рисования мира
        /// </summary>
        public enum DrawMode { All, Back, Front };
        #endregion

        #region Константы экрана, задающиеся при старте игры
        public static SpriteBatch spriteBatch;
        /// <summary>
        /// Ширина экрана в пикселях
        /// </summary>
        private static int Width;
        /// <summary>
        /// Высота экрана в пикселях
        /// </summary>
        private static int Height;
        /// <summary>
        /// Размер тайла
        /// </summary>
        public static int TileSize;
        /// <summary>
        /// Размер пикселя (для пиксель-арт игр) для округления движения камеры, 0 или 1 - если стандартный размер
        /// </summary>
        public static int PixelSize;
        #endregion

        #region Рабочие переменные
        /// <summary>
        /// Индекс самого правого пикселя мира
        /// </summary>
        internal static int RightMapPixelPixel;
        /// <summary>
        /// Индекс самого нижнего пикселя мира
        /// </summary>
        internal static int BottomMapPixel; 
        /// <summary>
        /// Правый предел передвижения камеры
        /// </summary>
        internal int RightLimit;
        /// <summary>
        /// Нижник предел передвижения камеры
        /// </summary>
        internal int BottomLimit;
        /// <summary>
        /// Смещение по X для задника и передника
        /// </summary>
        internal int BackShiftX;
        /// <summary>
        /// Смещение по Y для задника и передника
        /// </summary>
        internal int BackShiftY;
        /// <summary>
        /// Точка центра камеры по X
        /// </summary>
        public int CenterX;
        /// <summary>
        /// Точка центра камеры по Y
        /// </summary>
        public int CenterY;
        /// <summary>
        /// Ширина экрана в блоках (нужна для рисования мира)
        /// </summary>
        static int WidthInBlocks;
        /// <summary>
        /// Высота экрана в блоках (нужна для рисования мира)
        /// </summary>
        static int HeightInBlocks;
        /// <summary>
        /// Объект слежения камеры
        /// </summary>
        Box TrackingObject;
        /// <summary>
        /// Положение камеры (центр)
        /// </summary>
        public Vector2 Camera = new Vector2();
        /// <summary>
        /// Область рисования
        /// </summary>
        Viewport viewport;
        /// <summary>
        /// Цвет фантомного слоя
        /// </summary>
        static int PhantomColor;
        #endregion

        /// <summary>
        /// Установка параметров экрана
        /// </summary>
        /// <param name="graphics">GraphicsDeviceManager</param>
        /// <param name="width">Ширина экрана</param>
        /// <param name="height">Высота экрана</param>
        public static void Set(GraphicsDeviceManager graphics, int width, int height, int blocksize, int pixelsize)
        {
            Width = width;
            Height = height;
            TileSize = blocksize;
            graphics.PreferredBackBufferWidth = Width;
            graphics.PreferredBackBufferHeight = Height;
            graphics.ApplyChanges();
            WidthInBlocks = Width / TileSize + 2;
            HeightInBlocks = Height / TileSize + 2;
            PixelSize = pixelsize;
            if (PixelSize < 1) PixelSize = 1;
        }

        /// <summaru>
        /// Конструктор экрана. Размеры устанавливаются на весь экран.
        /// Делается после загрузки карты (устанавливаются границы для движения камеры).
        /// </summary>
        public Screen(Box trackingObject)
        {
            TrackingObject = trackingObject;
            RightMapPixelPixel = World.Width * TileSize - 1;
            BottomMapPixel = World.Height * TileSize - 1;
            RightLimit = World.Width * TileSize - Width;
            BottomLimit = World.Height * TileSize - Height;
            CenterX = Width / 2;
            CenterY = Height / 2;
            viewport = new Viewport(0, 0, Width, Height);
            BackShiftX = 0;
            BackShiftY = 0;
            PhantomColor = 255;
        }

        /// <summaru>
        /// Конструктор экрана, с указанными размерами.
        /// Делается после загрузки карты (устанавливаются границы для движения камеры).
        /// </summary>
        /// <param name = "x">Координата X</param>
        /// <param name = "y">Координата Y</param>
        /// <param name = "width">Ширина видимого экрана</param>
        /// <param name = "height">Высота видимого экрана</param>
        public Screen(Box trackingObject, int x, int y, int width, int height)
        {
            TrackingObject = trackingObject;
            RightMapPixelPixel = World.Width * TileSize - 1;
            BottomMapPixel = World.Height * TileSize - 1;
            RightLimit = World.Width * TileSize - width;
            BottomLimit = World.Height * TileSize - height;
            CenterX = width / 2;
            CenterY = height / 2;
            viewport = new Viewport(x, y, width, height);
            BackShiftX = width / 2 - Width / 2;
            BackShiftY = height / 2 - Height / 2;
        }

        /// <summary>
        /// Установка объекта слежения камеры и установка первоначальной позиции камеры
        /// </summary>
        /// <param name="obj"></param>
        public void SetTrackingObject(Box obj)
        {
            TrackingObject = obj;
        }

        /// <summary>
        /// Движение камеры к объекту по умолчанию с ускорением по умолчанию
        /// </summary>
        public void Update()
        {
            Update(TrackingObject.Center(), 0.02f);
        }

        /// <summary>
        /// Движение камеры к указанной точке с ускорением по умолчанию
        /// </summary>
        /// <param name="position">Точка слежения камеры</param>
        public void Update(Vector2 position)
        {
            Update(position, 0.02f);
        }

        /// <summary>
        /// Движение камеры к указанной точке с указанным ускорением
        /// </summary>
        /// <param name="position">Точка слежения камеры</param>
        /// <param name="a">Ускорение движения, 0.02F - ускорение по умолчанию.</param>
        public void Update(Vector2 position, float a)
        {
            //Движем камеру к точке
            Camera.X += (position.X - CenterX - Camera.X) * a;
            Camera.Y += (position.Y - CenterY - Camera.Y) * a;
            //Корректируем движение чтоб камера не вылетала за пределы карты
            if (Camera.X > RightLimit) Camera.X = RightLimit;
            if (Camera.Y > BottomLimit) Camera.Y = BottomLimit;
            if (Camera.X < 0) Camera.X = 0;
            if (Camera.Y < 0) Camera.Y = 0;
        }

        /// <summary>
        /// Рисование карты
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch</param>
        /// <param name="texture">Texture2D</param>
        public void Draw(DrawMode drawmode)
        {
            //Решаем какие слои рисовать
            int StartLayer = 1;
            int LastLayer = World.Layers;
            if (drawmode == DrawMode.Back) LastLayer = World.Main;
            if (drawmode == DrawMode.Front) StartLayer = World.Main + 1;
            //Рисуем
            for (int l = StartLayer; l <= LastLayer; l++)
            {
                //Считаем участок в матрице для вывода на экран
                int i1 = (int)(Camera.X / TileSize * (World.kX[l]));
                int j1 = (int)(Camera.Y / TileSize * (World.kY[l]));
                //Считаем прозрачность, если надо
                Color col = Color.White;
                if (World.Phantom > 0 && l == World.Phantom)
                {
                    bool under = false;
                    World.Players.ForEach(o => { if (o.UnderPhantom()) under = true; });
                    if (under & PhantomColor > 0) PhantomColor -= 16;
                    if (!under & PhantomColor < 255) PhantomColor += 16;
                    col = Color.FromNonPremultiplied(PhantomColor, PhantomColor, PhantomColor, PhantomColor);
                }
                //Рисуем слой
                for (int i = i1; i < i1 + WidthInBlocks; i++)
                {
                    for (int j = j1; j < j1 + HeightInBlocks; j++)
                    {
                        if (i >= 0 & i < World.Width & j >= 0 & j < World.Height && World.M[l, i, j] != 0)
                            spriteBatch.Draw(World.Texture,
                                new Vector2(i * TileSize - (int)(World.kX[l] * Camera.X / PixelSize) * PixelSize,
                                j * TileSize - (int)(World.kY[l] * Camera.Y / PixelSize) * PixelSize),
                                RectByNum(World.M[l, i, j], World.Texture), col);
                    }
                }
            }
        }
        /// <summaru>
        /// Извлечение блока из текстуры по номеру
        /// </summary>
        /// <param name = "num">Номер блока</param>
        /// <param name = "texture">Текстура</param>
        internal static Rectangle RectByNum(int num, Texture2D texture)
        {
            //Если на код привязана анимация, изменяем номер блока
            int index = MapAnimation.List.FindIndex(anim => anim.Included(num));
            if (index >= 0) num = MapAnimation.List[index].GetFrame(num);
            //Ищем блок на текстуре
            int count = texture.Width / TileSize;
            return new Rectangle(
                (num % count) * TileSize,
                (num / count) * TileSize, TileSize, TileSize);
        }

        /// <summary>
        /// Рисование сцены игры (задник, объекты, передник)
        /// </summary>
        public void Draw(GraphicsDevice graphics)
        {
            graphics.Viewport = viewport;
            spriteBatch.Begin();
            spriteBatch.Draw(World.Background, new Rectangle(BackShiftX, BackShiftY, Width, Height),
                new Rectangle(0, 0, World.Background.Width, World.Background.Height), Color.White);
            Draw(DrawMode.Back);
            World.Objects.ForEach(o => o.Draw(this));
            Draw(DrawMode.Front);
            if (World.Front != null) spriteBatch.Draw(World.Front, new Rectangle(BackShiftX, BackShiftY, Width, Height),
                new Rectangle(0, 0, World.Front.Width, World.Front.Height), Color.White);
            spriteBatch.End();
        }
    }
}
