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
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class CursorComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Keys[] myKeys;
        Vector2[] gunPointers;
        SpriteBatch mSpriteBatch;
        SoundEffect gunShot, gunShot2, explodeSound;
        PlayerIndex playerIndex;
        Game game;
        KeyboardState currentKeyPress;
        WorldComponent world;
        Texture2D cursorTexture;
        int shotDmg = 10;
        int playerNum;
        int gunChoice;
        MouseState currentMouseState;
        public MenuScreen menuScreen { get; set; }
        public Enemy mEnemy { get; set; }
        public Vector2 cursorPos;
        ContentManager content;
        float gunCool = 500;
        float gunCool2 = 1500;
        float explodeCool = 4000;
        Texture2D gunChoiceText, gunChoiceText2;
        GamePadState currPadOne = GamePad.GetState(PlayerIndex.One);

        public Vector2 getCursorPosition()
        {
            return cursorPos;
        }


        public  CursorComponent(Game game, int playerNum, WorldComponent world, ContentManager content)
            : base(game)
        {
            this.content = content;
            cursorPos = new Vector2(640, 360);
            this.world = world;
            

            if (playerNum == 0)
            {
                playerIndex = PlayerIndex.One;
                myKeys = new Keys[6] { Keys.W, Keys.S, Keys.A, Keys.D, Keys.Space, Keys.Tab };
                gunPointers = new Vector2[3] { new Vector2(10, 718), new Vector2(55, 718), new Vector2(100, 718) };
                
            }
            else
            {
                playerIndex = PlayerIndex.Two;
                myKeys = new Keys[6] { Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.Enter, Keys.RightShift };
                gunPointers = new Vector2[3] { new Vector2(24, 718), new Vector2(69, 718) , new Vector2(114,718)};
               
            }
            this.game = game;
            this.playerNum = playerNum;
            currentMouseState = Mouse.GetState();
            
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        protected override void LoadContent() 
        {   
            gunChoiceText = game.Content.Load<Texture2D>("gunChoice");
            gunChoiceText2 = game.Content.Load<Texture2D>("gunChoice2");
            gunShot = game.Content.Load<SoundEffect>("shot");
            gunShot2 = game.Content.Load<SoundEffect>("shot2");
            explodeSound = game.Content.Load<SoundEffect>("explode");
            mSpriteBatch = new SpriteBatch(GraphicsDevice);
            String playerTexture;
            if (playerNum == 0)
                playerTexture = "crosshair";
            else
                playerTexture = "crossHair2";
            cursorTexture = game.Content.Load<Texture2D>(playerTexture);
            base.LoadContent();
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            
            KeyboardState lastKeyPress = currentKeyPress;
            currentKeyPress = Keyboard.GetState();
            if (gunCool >= 0) gunCool -= gameTime.ElapsedGameTime.Milliseconds;
            if (gunCool2 >= 0) gunCool2 -= gameTime.ElapsedGameTime.Milliseconds;
            if (explodeCool >= 0) explodeCool -= gameTime.ElapsedGameTime.Milliseconds;

            if (explodeCool < 0) explodeCool = 0;
            if (gunCool < 0) gunCool = 0;
            if (gunCool2 < 0) gunCool2 = 0;
            MouseState lastMouseState = currentMouseState;

            GamePadState prevStat = currPadOne;
            currPadOne = GamePad.GetState(playerIndex);
            currentMouseState = Mouse.GetState();

            if (menuScreen.inMenu) return;

            if ((GamePad.GetState(playerIndex).ThumbSticks.Left.X > 0 || currentKeyPress.IsKeyDown(myKeys[3]))
                    && (cursorPos.X + cursorTexture.Width < 1280))
                cursorPos.X += 10;
            if ((GamePad.GetState(playerIndex).ThumbSticks.Left.X < 0 || currentKeyPress.IsKeyDown(myKeys[2]))
                    && (cursorPos.X > 0))
                cursorPos.X -= 10;
            if ((GamePad.GetState(playerIndex).ThumbSticks.Left.Y < 0 || currentKeyPress.IsKeyDown(myKeys[1]))
                    && (cursorPos.Y + cursorTexture.Height < 720))
                cursorPos.Y += 10;
            if ((GamePad.GetState(playerIndex).ThumbSticks.Left.Y > 0 || currentKeyPress.IsKeyDown(myKeys[0]))
                    && (cursorPos.Y > 0))
                cursorPos.Y -= 10;
 
            if ((lastKeyPress.IsKeyUp(myKeys[4]) && currentKeyPress.IsKeyDown(myKeys[4]))
                    || (prevStat.Buttons.RightShoulder == ButtonState.Released && currPadOne.Buttons.RightShoulder == ButtonState.Pressed))
            {
                
                if (gunChoice == 0)
                {
                    if (gunCool <= 0)
                    {
                        gunShot.Play();
                        gunCool = 300;
                    }
                    else
                    {
                        shotDmg = 0;
                    }
                  
                }
                else if (gunChoice == 1)
                {

                    if (gunCool2 <= 0)
                    {
                        gunShot2.Play();
                        gunCool2 = 2000;
                    }
                    else
                    {
                        shotDmg = 0;
                    }
                }
                else if (gunChoice == 2)
                {

                    if (explodeCool <= 0)
                    {
                        explodeSound.Play();
                        explodeCool = 4000;

                        for (int i = mEnemy.basicEnemyList.Count - 1; i >= 0; i--)
                        {
                            
                            mEnemy.basicEnemyList[i].enemyLife -= shotDmg;
                            if (mEnemy.basicEnemyList[i].enemyLife <= 0)
                            {
                                mEnemy.basicEnemyList.RemoveAt(i);
                                mEnemy.enemyKilled++;
                            }
                        }
                    }
                    else
                    {
                        shotDmg = 0;
                    }
                }

                for ( int i = mEnemy.basicEnemyList.Count - 1; i >= 0; i--)
                {
                    Rectangle mEnemyBounds = mEnemy.basicEnemyList[i].enemyTexture.Bounds;
                    Rectangle cursorBounds = cursorTexture.Bounds;
                    
                    mEnemyBounds.X = (int) mEnemy.basicEnemyList[i].enemyPosition.X;
                    mEnemyBounds.Y = (int) mEnemy.basicEnemyList[i].enemyPosition.Y;
                    cursorBounds.X = (int) cursorPos.X;
                    cursorBounds.Y = (int) cursorPos.Y;

                    if (mEnemyBounds.Contains(cursorBounds.Center))
                    {
                          mEnemy.basicEnemyList[i].enemyLife -= shotDmg;
                        if (mEnemy.basicEnemyList[i].enemyLife <= 0) 
                        {
                            mEnemy.basicEnemyList.RemoveAt(i);
                            mEnemy.enemyKilled++;
                        }
                        
                    }

                }
            }

            if ((lastKeyPress.IsKeyUp(myKeys[5]) && currentKeyPress.IsKeyDown(myKeys[5]))
                    || (prevStat.Buttons.A == ButtonState.Released && currPadOne.Buttons.A == ButtonState.Pressed))
            {
                gunChoice++;
                if (gunChoice > 2)
                    gunChoice = 0;
            }

            if (gunChoice == 0)
            {
                shotDmg = 10;
                
            }
            else if (gunChoice == 1)
            {
                shotDmg = 30;
            }
            else if (gunChoice == 2)
            {
                shotDmg = 20;
            }
            
            base.Update(gameTime);
        }

        private Vector2 rotate(Vector2 v, float theta)
        {
            return new Vector2((float)(v.X * Math.Cos(theta) + v.Y * Math.Sin(theta)), (float)(v.X * Math.Sin(theta) - v.Y * Math.Cos(theta)));

        }

        public override void Draw(GameTime gameTime)
        {
            if (menuScreen.inMenu) return;
            mSpriteBatch.Begin();

            if (playerNum == 0)
            {
                mSpriteBatch.Draw(gunChoiceText, gunPointers[gunChoice], Color.White);
            }
            else
            {
                mSpriteBatch.Draw(gunChoiceText2, gunPointers[gunChoice], Color.White);
            }

            if (gunChoice == 0)
            {
                mSpriteBatch.Draw(gunChoiceText, new Vector2(10, 718), Color.White);
            }
            else if (gunChoice == 1)
            {
                mSpriteBatch.Draw(gunChoiceText, new Vector2(55, 718), Color.White);
            }
            if (playerNum == 0)
            {
                mSpriteBatch.DrawString(menuScreen.menuFont, "P1:  Basic Gun - " + gunCool / 1000 + " seconds left", new Vector2(140, 735),
                                        Color.Black, 0.0f, new Vector2(0, 0), 0.2f, SpriteEffects.None, 0);
                mSpriteBatch.DrawString(menuScreen.menuFont, "Super Gun - " + gunCool2 / 1000 + " seconds left", new Vector2(175, 755),
                                            Color.Black, 0.0f, new Vector2(0, 0), 0.2f, SpriteEffects.None, 0);
                mSpriteBatch.DrawString(menuScreen.menuFont, "Boom! - " + explodeCool / 1000 + " seconds left", new Vector2(435, 735),
                                            Color.Black, 0.0f, new Vector2(0, 0), 0.2f, SpriteEffects.None, 0);
            }
            else
            {
                mSpriteBatch.DrawString(menuScreen.menuFont, "P2:  Basic Gun - " + gunCool / 1000 + " seconds left", new Vector2(715, 735),
                                       Color.Black, 0.0f, new Vector2(0, 0), 0.2f, SpriteEffects.None, 0);
                mSpriteBatch.DrawString(menuScreen.menuFont, "Super Gun - " + gunCool2 / 1000 + " seconds left", new Vector2(753, 755),
                                            Color.Black, 0.0f, new Vector2(0, 0), 0.2f, SpriteEffects.None, 0);
                mSpriteBatch.DrawString(menuScreen.menuFont, "Boom! - " + explodeCool / 1000 + " seconds left", new Vector2(1010, 735),
                                            Color.Black, 0.0f, new Vector2(0, 0), 0.2f, SpriteEffects.None, 0);
             
            
            }
           
            //rotation += 0.01f;
            mSpriteBatch.Draw(cursorTexture,
                                      cursorPos, Color.White);
                
            mSpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
