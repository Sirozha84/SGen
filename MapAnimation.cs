using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SGen
{
    class MapAnimation
    {
        /// <summary>
        /// Список правил анимации
        /// </summary>
        public static List<MapAnimation> List = new List<MapAnimation>();
        /// <summary>
        /// Типы анимации. FW - прамая, FWBF - прамая/обратная, RND - рандомные кадры
        /// </summary>
        public enum Types { FW, FWBF, RND, RNDall }
        /// <summary>
        /// Код тайла, для которого применяется
        /// </summary>
        public ushort Code;
        /// <summary>
        /// Количество кадров
        /// </summary>
        public byte Frames;
        /// <summary>
        /// Количество кадров игры на каждый кадр анимации
        /// </summary>
        public byte Time;
        /// <summary>
        /// Тип анимации
        /// </summary>
        public Types Type;
        /// <summary>
        /// Кадр
        /// </summary>
        int Frame;
        /// <summary>
        /// Инкремент (+ или -)
        /// </summary>
        int FrameInc;
        /// <summary>
        /// Счётчик времени
        /// </summary>
        byte time;
        Random RND = new Random();

        /// <summary>
        /// Конструктор новой анимации
        /// </summary>
        /// <param name="code">Код (первый кадр)</param>
        /// <param name="frames">Количество кадров</param>
        /// <param name="time">Время показа кадра</param>
        /// <param name="type">Тип анимации</param>
        public MapAnimation(ushort code, byte frames, byte time, Types type)
        {
            Code = code;
            Frames = frames;
            Frame = Code;
            FrameInc = 1;
            Time = time;
            Type = type;
        }

        /// <summary>
        /// Перевод типа анимации на человеческий язык
        /// </summary>
        /// <returns>Строка с названием</returns>
        public string TypeString()
        {
            if (Type == Types.FW) return "Прамая";
            if (Type == Types.FWBF) return "Прамая/обратная";
            if (Type == Types.RND) return "Случайные кадры";
            return "Случайные кадры все";
        }

        /// <summary>
        /// Расчёт текущего кадра анимации. Вызывается на каждый кадр игры.
        /// </summary>
        public void Update()
        {
            if (time++ >= Time)
            {
                time = 0;
                Next(0, true);
            }
        }

        /// <summary>
        /// Проверка на то, подходит ли код под правило анимации
        /// </summary>
        /// <param name="code">Код</param>
        /// <returns>Подходит</returns>
        public bool Included(int code)
        {
            return code >= Code & code < Code + Frames;
        }

        /// <summary>
        /// Высчитываем следующий кадр
        /// </summary>
        /// <param name="frames">На сколько кадров вперёд делать расчёт</param>
        /// <param name="global">Глобальное ли это изменение, или только для конкретного тайла</param>
        /// <returns>Следующий</returns>
        int Next(int frames, bool global)
        {
            //Временные значения
            int frame = Frame;
            int inc = FrameInc;
            for (int i = 0; i <= frames; i++)
            {
                switch (Type)
                {
                    case Types.FW:
                        frame++;
                        if (frame == Code + Frames) frame = Code;
                        break;
                    case Types.FWBF:
                        frame += inc;
                        if (frame == Code) inc = 1;
                        if (frame == Code + Frames - 1) inc = -1;
                        break;
                    case Types.RND:
                        if (global) frame = Code + RND.Next(Frames);
                        break;
                    default:
                        frame = Code + RND.Next(Frames);
                        break;
                }
            }
            //Глобальное ли это изменение? если да, временные значения превращаем в постоянные
            if (global)
            {
                Frame = frame;
                FrameInc = inc;
            }
            return frame;
        }

        /// <summary>
        /// Получение кадра анимации
        /// </summary>
        /// <param name="code">Код тайла</param>
        /// <returns></returns>
        public int GetFrame(int code)
        {
            return Next(code - Code, false);
        }
    }
}
