using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SGen
{
    public class Map
    {
        /// <summary>
        /// Количество слоёв
        /// </summary>
        public static byte Layers;
        /// <summary>
        /// Ширина карты в блоках
        /// </summary>
        public static ushort Width;
        /// <summary>
        /// Высота карты в блоках
        /// </summary>
        public static ushort Height;
        /// <summary>
        /// Массив карты [слой, x, y]
        /// </summary>
        public static ushort[, ,] M;
        /// <summary>
        /// Коэффициент смещения по горизонтали (0 - стоит на месте, 1 - движется вместе с картой,
        /// больше 1 - движится быстрей
        /// </summary>
        internal static float[] kX;
        /// <summary>
        /// Коэффициент смещения по вертикали (0 - стоит на месте, 1 - движется вместе с картой,
        /// больше 1 - движится быстрей
        /// </summary>
        internal static float[] kY;
        /// <summary>
        /// "Главный" слой. Слой после которого выводятся объекты, затем рисуются более близкие слои
        /// Выбирается автоматически первый у которого коэффициенты смещения 1, 1
        /// </summary>
        public static byte Main;
        /// <summary>
        /// Текстура со спрайтами мира
        /// </summary>
        public static Texture2D Texture;
        
        /// <summary>
        /// Загрузка карты и графики для неё
        /// </summary>
        /// <param name="MapFile">Файл карты в формате SGMap, создаррая редактором Map Editor</param>
        public static void LoadMap(string MapFile)
        {
            M = null;
            kX = null;
            kY = null;
            BinaryReader file = new BinaryReader(new FileStream(MapFile, FileMode.Open));
            file.ReadString();
            Screen.BlockSize = file.ReadUInt16();
            file.ReadUInt16();
            file.ReadUInt16();
            Layers = file.ReadByte();
            Width = file.ReadUInt16();
            Height = file.ReadUInt16();
            M = new ushort[Layers + 1, Width, Height];
            kX = new float[Layers + 1];
            kY = new float[Layers + 1];
            Main = 0;
            //Загружаем карту
            for (byte l = 0; l <= Layers; l++)
            {
                file.ReadString();
                bool OK = true;
                do
                {
                    int j = file.ReadUInt16();
                    if (j != 0xFFFF)
                    {
                        int FirstBlock = file.ReadUInt16();
                        int LastBlock = file.ReadUInt16();
                        for (int i = FirstBlock; i <= LastBlock; i++) M[l, i, j] = file.ReadUInt16();
                    }
                    else OK = false;
                } while (OK);
                kX[l] = file.ReadSingle();
                kY[l] = file.ReadSingle();
                //Устанавливаем главный слой, если коэффициенты равны 1
                if (kX[l] == 1 & kY[l] == 1 & Main == 0) Main = l;
            }
            //Загружаем правила анимации
            file.ReadString();
            //Правила анимации
            int count = file.ReadInt32();
            for (int i = 0; i < count; i++)
                MapAnimation.List.Add(new MapAnimation(file.ReadUInt16(), file.ReadByte(), file.ReadByte(), (MapAnimation.Types)file.ReadByte()));
            file.Close();
            //Подготавливаем лимиты движения камеры
            Screen.SetLimits(Width, Height);
        }
    }
}
