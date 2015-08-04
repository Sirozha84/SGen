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
        /// Размер блока
        /// </summary>
        public static int BlockSize;
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
        internal static int RightLimit;
        /// <summary>
        /// Нижник предел передвижения камеры
        /// </summary>
        internal static int BottomLimit;
        /// <summary>
        /// Точка центра камеры по X
        /// </summary>
        public static int CenterX;
        /// <summary>
        /// Точка центра камеры по Y
        /// </summary>
        public static int CenterY;
        /// <summary>
        /// Ширина экрана в блоках (нужна для рисования мира)
        /// </summary>
        private static int WidthInBlocks;
        /// <summary>
        /// Высота экрана в блоках (нужна для рисования мира)
        /// </summary>
        private static int HeightInBlocks;
        #endregion

        #region Рабочие переменные
        /// <summary>
        /// Положение камеры (центр)
        /// </summary>
        public static Vector2 Camera = new Vector2();
        #endregion

        /// <summary>
        /// Установка параметров экрана
        /// </summary>
        /// <param name="graphics">GraphicsDeviceManager</param>
        /// <param name="width">Ширина экрана</param>
        /// <param name="height">Высота экрана</param>
        public static void Set(GraphicsDeviceManager graphics, int width, int height, int blocksize)
        {
            Width = width;
            Height = height;
            BlockSize = blocksize;
            graphics.PreferredBackBufferWidth = Width;
            graphics.PreferredBackBufferHeight = Height;
            graphics.ApplyChanges();
            CenterX = Width / 2;
            CenterY = Height / 2;
            WidthInBlocks = Width / BlockSize + 2;
            HeightInBlocks = Height / BlockSize + 2;
        }
        /// <summary>
        /// Движение камеры у указанной точке. Точка находится в центре экрана.
        /// Ускорение используется по умолчанию.
        /// </summary>
        /// <param name="position">Точка слежения камеры</param>
        public static void Action(Vector2 position)
        {
            Action(position, 0.02f);
        }
        /// <summary>
        /// Обновление позиции камеры, движущейся с ускорением к позиции, с которой
        /// указанная точка показывается в центре экрана.
        /// </summary>
        /// <param name="position">Точка слежения камеры</param>
        /// <param name="a">Ускорение движения, 0.02F - ускорение по умолчанию.</param>
        public static void Action(Vector2 position, float a)
        {
            //Движем камеру к точке
            Camera.X += (position.X - CenterX - Camera.X) * a;
            Camera.Y += (position.Y - CenterY - Camera.Y) * a;
            //Корректируем движение чтоб камера не вылетала за пределы карты
            if (Camera.X < 0) Camera.X = 0;
            if (Camera.Y < 0) Camera.Y = 0;
            if (Camera.X > RightLimit) Camera.X = RightLimit;
            if (Camera.Y > BottomLimit) Camera.Y = BottomLimit;
            //Обрабатываем анимацию
            foreach (MapAnimation anim in MapAnimation.List) anim.Action();
        }
        /// <summary>
        /// Рисование карты
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch</param>
        /// <param name="texture">Texture2D</param>
        public static void Draw(DrawMode drawmode)
        {
            //Решаем какие слои рисовать
            int StartLayer = 1;
            int LastLayer = Map.Layers;
            if (drawmode == DrawMode.Back) LastLayer = Map.Main;
            if (drawmode == DrawMode.Front) StartLayer = Map.Main + 1;
            //Рисуем
            for (int l = StartLayer; l <= LastLayer; l++)
            {
                //Считаем участок в матрице для вывода на экран
                int i1 = (int)(Camera.X / BlockSize * (Map.kX[l]));
                int j1 = (int)(Camera.Y / BlockSize * (Map.kY[l]));
                for (int i = i1; i < i1 + WidthInBlocks; i++)
                {
                    for (int j = j1; j < j1 + HeightInBlocks; j++)
                    {
                        if (i >= 0 & i < Map.Width & j >= 0 & j < Map.Height && Map.M[l, i, j] != 0)
                            spriteBatch.Draw(Map.Texture,
                                new Vector2((int)(i * BlockSize - Map.kX[l] * Camera.X+BlockSize)-BlockSize,
                                (int)(j * BlockSize - Map.kY[l] * Camera.Y + BlockSize) - BlockSize),
                                RectByNum(Map.M[l, i, j], Map.Texture), Color.White);
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
            int count = texture.Width / BlockSize;
            return new Rectangle(
                (num % count) * BlockSize,
                (num / count) * BlockSize, BlockSize, BlockSize);
        }
        /// <summaru>
        /// Установка лимитов на движение камеры
        /// </summary>
        /// <param name = "width">Количество блоков по горизонтали</param>
        /// <param name = "height">Количество блоков по вертикали</param>
        internal static void SetLimits(int width, int height)
        {
            RightMapPixelPixel = width * BlockSize - 1;
            BottomMapPixel = height * BlockSize - 1;
            RightLimit = width * BlockSize - Width;
            BottomLimit = height * BlockSize - Height;
        }
    }
}
