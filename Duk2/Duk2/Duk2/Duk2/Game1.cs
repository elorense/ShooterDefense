using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Duk2
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        
        private const int BackBufferWidth = 1280;
        private const int BackBufferHeight = 780;
        int totalKills;
        SpriteBatch sb;
        Texture2D backGround;
        CursorComponent crossHair1, crossHair2;
        Enemy enemy;
        WorldComponent world;
        MenuScreen menuScreen;
        bool gameBegin;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = BackBufferHeight;
            graphics.PreferredBackBufferWidth = BackBufferWidth;
            Content.RootDirectory = "Content";
            world = new WorldComponent(this);
            crossHair1 = new CursorComponent(this, 0, world, Content);
            crossHair2 = new CursorComponent(this, 1, world, Content);
            enemy = new Enemy(this, world.treasureBounds, 0);
            menuScreen = new MenuScreen(this, enemy);
            Components.Add(crossHair1);
            Components.Add(crossHair2);
            crossHair1.mEnemy = enemy;
            crossHair2.mEnemy = enemy;
            crossHair1.menuScreen = menuScreen;
            crossHair2.menuScreen = menuScreen;
          
            //graphics.ToggleFullScreen();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //don't use "new" here
     
            world.Initialize();
            menuScreen.Initialize();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
           
            sb = new SpriteBatch(GraphicsDevice);
            backGround = Content.Load<Texture2D>("SnowBackground");
           
            base.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            KeyboardState keyBoard = Keyboard.GetState();
            
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                    keyBoard.IsKeyDown(Keys.Escape))
                this.Exit();

            menuScreen.Update(gameTime);
            
            if(menuScreen.playerNumber == 1) Components.Remove(crossHair2);

            if (menuScreen.restart)
            { 
                //add a reset function
                enemy = new Enemy(this, world.treasureBounds, 0);
                menuScreen.restart = false;
                gameBegin = true;

            }
            world.Update(gameTime);
            if(!menuScreen.inMenu)
                enemy.Update(gameTime);
            crossHair1.mEnemy = enemy;
            crossHair2.mEnemy = enemy;
            crossHair1.menuScreen = menuScreen;
            crossHair2.menuScreen = menuScreen;
            base.Update(gameTime);
        }
        public void getThis() { }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SkyBlue);
            sb.Begin();
            sb.Draw(backGround, new Vector2(0, 0), Color.White);  

            if (gameBegin)
            {
                if (enemy.lifeCount > 0 && enemy.enemyKilled <= 15)
                {   
                    world.Draw(sb, enemy.lifeCount);
                    enemy.Draw(sb);
                }
                else
                {
                    if (enemy.stage == 1)
                    {
                        
                        menuScreen.Draw(sb, 1, totalKills + enemy.enemyKilled, enemy.lifeCount);
                    }
                    else
                    {
                        totalKills += enemy.enemyKilled;
                        enemy.stage++;
                        enemy.enemyKilled = 0;
                        enemy.basicEnemyCount = 0;
                        enemy.bossCount = 0;
                    }
                }
            }
            else
            {

                menuScreen.Draw(sb, 0, enemy.enemyKilled, enemy.lifeCount);
              
            }

            sb.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
