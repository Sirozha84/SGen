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
            BinaryReader file = new BinaryReader(new FileStream(MapFile, System.IO.FileMode.Open));
            file.ReadString();
            Screen.BlockSize = file.ReadInt32();
            file.ReadInt32();
            file.ReadInt32();
            Layers = file.ReadByte();
            Width = file.ReadUInt16();
            Height = file.ReadUInt16();
            M = null;
            M = new ushort[Layers, Width, Height];
            kX = null;
            kX = new float[Layers];
            kY = null;
            kY = new float[Layers];
            Main = 0;
            //Загружаем карту
            for (byte l = 0; l < Layers; l++)
            {
                ushort firststring = file.ReadUInt16();
                ushort laststring = file.ReadUInt16();
                for (int i = firststring; i <= laststring; i++)
                {
                    ushort firstrow = file.ReadUInt16();
                    ushort lasttrow = file.ReadUInt16();
                    for (int j = firstrow; j <= lasttrow; j++)
                    {
                        M[l, j, i] = file.ReadUInt16();
                    }
                }
                kX[l] = file.ReadSingle();
                kY[l] = file.ReadSingle();
                //Устанавливаем главный слой, если коэффициенты равны 1
                if (kX[l] == 1 & kY[l] == 1 & Main == 0) Main = l;
            }
            file.Close();
            //Подготавливаем лимиты движения камеры
            Screen.SetLimits(Width, Height);
        }
    }
}
