using Microsoft.Xna.Framework;
using SGen;

namespace SGenExample
{
    //Дочерний класс нужен только для заполнения объектов
    class MyWorld : World
    {
        public MyWorld(string MapFile, Game game) : base(MapFile, game, typeof(Player)) { }
        public override void AddObject(ushort code, int x, int y)
        {
            if (code == 2) Objects.Add(new Ball(x, y));     //При желании можно добавлять несколько объектов на одну клетку
            if (code == 3) { Box o = new Player(x, y); Objects.Add(o); Players.Add(o); }   //например монетки или мухи :-)
        }
    }
}
