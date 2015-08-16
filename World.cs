using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SGen
{
    public abstract class World
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
        /// Текстура для задника
        /// </summary>
        public static Texture2D Background;
        /// <summary>
        /// Текстура со спрайтами мира
        /// </summary>
        public static Texture2D Texture;
        /// <summary>
        /// Текстура с передником
        /// </summary>
        public static Texture2D Front;
        /// <summary>
        /// Список объектов
        /// </summary>
        public static List<Box> Objects = new List<Box>();
        /// <summary>
        /// Загрузка карты и графики для неё
        /// </summary>
        /// <param name="MapFile">Файл карты в формате SGMap, создаррая редактором Map Editor</param>
        public static void LoadMap(string MapFile, Game game)
        {
            
            M = null;
            kX = null;
            kY = null;
            BinaryReader file = new BinaryReader(new FileStream(MapFile, FileMode.Open));
            file.ReadString();
            Screen.TileSize = file.ReadUInt16();
            file.ReadUInt16();
            file.ReadUInt16();
            Width = file.ReadUInt16();
            Height = file.ReadUInt16();
            Layers = file.ReadByte();
            Main = file.ReadByte();
            M = new ushort[Layers + 1, Width, Height];
            kX = new float[Layers + 1];
            kY = new float[Layers + 1];
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
                        //Битность
                        byte bpt = file.ReadByte();
                        if (bpt == 1)
                        {
                            //Однобайтовая версия
                            byte p = 0;
                            for (int i = 0; i < Width; i++)
                            {
                                byte n = file.ReadByte();
                                if (n != 0xFF)
                                {
                                    p = n;
                                    M[l, i, j] = p;
                                }
                                else
                                {
                                    int c = file.ReadUInt16();
                                    i--;
                                    for (int s = 1; s < c; s++) M[l, i + s, j] = p;
                                    i += c - 1;
                                }
                            }
                        }
                        else
                        {
                            //Двухбайтовая версия
                            ushort p = 0;
                            for (int i = 0; i < Width; i++)
                            {
                                ushort n = file.ReadUInt16();
                                if (n != 0xFFFF)
                                {
                                    p = n;
                                    M[l, i, j] = p;
                                }
                                else
                                {
                                    i--;
                                    int c = file.ReadUInt16();
                                    for (int s = 1; s < c; s++) M[l, i + s, j] = p;
                                    i += c - 1;
                                }
                            }
                        }
                    }
                    else OK = false;
                } while (OK);
                kX[l] = file.ReadSingle();
                kY[l] = file.ReadSingle();
            }
            //Загружаем текстуры
            file.ReadString();
            Background = game.Content.Load<Texture2D>(Path.GetFileNameWithoutExtension(file.ReadString()));
            Texture = game.Content.Load<Texture2D>(Path.GetFileNameWithoutExtension(file.ReadString()));
            Front = game.Content.Load<Texture2D>(Path.GetFileNameWithoutExtension(file.ReadString()));
            file.ReadString();
            //Загружаем правила анимации
            file.ReadString();
            int count = file.ReadInt32();
            for (int i = 0; i < count; i++)
                MapAnimation.List.Add(new MapAnimation(file.ReadUInt16(), file.ReadByte(), file.ReadByte(), (MapAnimation.Types)file.ReadByte()));
            file.Close();
        }

        /// <summary>
        /// Обработка игровых объектов
        /// </summary>
        public static void Update()
        {
            //Обработка объектов
            Objects.ForEach(o =>
            {
                o.Update();
                if (o.CollisionTests) Objects.ForEach(s => o.CollisionTest(s));
            });
            //Уничтожение всех вылетевших за предел экрана или уничтоженных объектов
            Objects.RemoveAll(o => o.Out() | o.Destroyed);
        }

        /// <summary>
        /// Добавление объекта в игровой мир
        /// </summary>
        /// <param name="code"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        abstract public void AddObject(ushort code, int x, int y);
    }
}
