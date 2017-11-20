using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SGen;

namespace SGenExample
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Screen screen, screen2;
        MyWorld world;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //Настраиваем экран
            Screen.Set(graphics, 1200, 800, 32, 0);
            graphics.IsFullScreen = false;
            //Назначаем блоки, через которые нельзя проходить
            Box.Grounds = new int[] { 1 };
            //Box.Platforms //Блоки, через которые нельзя падать, если таких нет, то можно не объявлять
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Screen.spriteBatch = spriteBatch;   //Обязательно. Передаём ссылку на устройство рисования классу экрана
            Box.spriteBatch = spriteBatch;      //и классу блока (для рисования)
            
            //Загрузка спрайтов
            Player.Texture = Content.Load<Texture2D>("player");
            Ball.Texture = Content.Load<Texture2D>("ball");
            //Подготовка игрового мира
            world = new MyWorld("\\map.map", this);
            //Установка экрана (экранов может быть несколько, например, сплитскрин в мультиплеере
            //screen = new Screen(World.Players[0]);
            screen = new Screen(World.Players[0], 0, 0, 599, 800);
            screen2 = new Screen(World.Objects[50], 601, 0, 599, 800);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            //Обновление игрового процесса
            World.Update();
            //Обновление камеры (если экранов несколько, обновление надо сделать для всех)
            screen.Update();
            screen2.Update();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //Рисование сцены
            screen.Draw(GraphicsDevice);
            screen2.Draw(GraphicsDevice);
            base.Draw(gameTime);
        }
    }
}
