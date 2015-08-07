using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SGen
{
    class Engine
    {
        public static List<Box> Objects = new List<Box>();

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
    }
}
