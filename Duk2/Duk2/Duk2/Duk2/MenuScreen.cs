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
    public class MenuScreen : Microsoft.Xna.Framework.DrawableGameComponent
    {
        KeyboardState currentKeyPress = Keyboard.GetState();
        public int playerNumber;
        float firstChoiceSize = 0.5f, secondChoiceSize = 0.3f, thirdChoiceSize = 0.3f;
        int gameOverChosen = 1;
        public SpriteFont menuFont;
        int gameStatus;
        Enemy enemy;
        bool playing = false;
        bool gameOver = false;
        bool stopPlay;
        public bool inMenu { get; set; }
        Song menuSong, gameOverSong;
        GamePadState currPadOne = GamePad.GetState(PlayerIndex.One);
        Game game;
        public bool restart { get; set; }

        public MenuScreen(Game game, Enemy enemy)
            : base(game)
        {
            this.game = game;
            this.enemy = enemy;
            inMenu = true;
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            LoadContent();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            menuSong = game.Content.Load<Song>("menuSong");
            gameOverSong = game.Content.Load<Song>("gameOverSong");
            MediaPlayer.Play(menuSong);
            MediaPlayer.IsRepeating = true;
            //MediaPlayer.Pause();
            menuFont = game.Content.Load<SpriteFont>("Courier12");
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
            GamePadState prevStat = currPadOne;
            currPadOne = GamePad.GetState(PlayerIndex.One);

            if (stopPlay)
            {
                stopMenuSong();
            }
            if (inMenu)
            {
                if (!playing)
                {
                    playMenuSong();
                    
                }

                if ((lastKeyPress.IsKeyUp(Keys.Down) && currentKeyPress.IsKeyDown(Keys.Down))
                    || (prevStat.DPad.Down == ButtonState.Released && currPadOne.DPad.Down == ButtonState.Pressed))
                {
                    gameOverChosen++;
                }

                if ((lastKeyPress.IsKeyUp(Keys.Up) && currentKeyPress.IsKeyDown(Keys.Up))
                    || (prevStat.DPad.Up == ButtonState.Released && currPadOne.DPad.Up == ButtonState.Pressed))
                {
                    gameOverChosen--;
                }

                if (gameOverChosen > 3)
                    gameOverChosen = 1;
                else if (gameOverChosen < 1)
                    gameOverChosen = 3;

                if (gameOverChosen == 1)
                {
                    firstChoiceSize = 0.5f;
                    secondChoiceSize = 0.3f;
                    thirdChoiceSize = 0.3f;
                }
                else if (gameOverChosen == 2)
                {
                    firstChoiceSize = 0.3f;
                    secondChoiceSize = 0.5f;
                    thirdChoiceSize = 0.3f;
                }
                else if (gameOverChosen == 3)
                {
                    firstChoiceSize = 0.3f;
                    secondChoiceSize = 0.3f;
                    thirdChoiceSize = 0.5f;
                }
               

                if (((lastKeyPress.IsKeyUp(Keys.Enter) && currentKeyPress.IsKeyDown(Keys.Enter))
                    || (prevStat.Buttons.A == ButtonState.Released && currPadOne.Buttons.A == ButtonState.Pressed)))
                {
                    if (gameOverChosen == 1)
                    {
                        playerNumber = 1;
                        restart = true;
                        inMenu = false;
                        stopPlay = true;
                    }
                    else if (gameOverChosen == 2)
                    {
                        if (gameStatus == 1)
                            game.Exit();
                        playerNumber = 2;
                        restart = true;
                        inMenu = false;
                        stopPlay = true;
                    }
                    else if (gameOverChosen == 3)
                    {
                        if (gameStatus == 3)
                        {
                            gameOverChosen = 1;
                            firstChoiceSize = 0.5f;
                            thirdChoiceSize = 0.3f;
                        }
                        else
                        {
                            game.Exit();
                        }
                        
                    }
                }
            }
            base.Update(gameTime);
        }

        public void playMenuSong()
        {
            if (gameOver)
                MediaPlayer.Play(gameOverSong);
            else
                MediaPlayer.Play(menuSong);
            playing = true;
            stopPlay = false;
        }

        public void stopMenuSong()
        {
            MediaPlayer.Stop();
            playing = false;

        }

        public virtual void Draw(SpriteBatch sb, int gameStatus, int enemyKilled, int lifeCount)
        {
            this.gameStatus = gameStatus;
            if (gameStatus == 0) //Main Menu
            {
                sb.DrawString(this.menuFont, "<Insert Game Title Here>", new Vector2(100, 230), Color.DarkBlue);
                sb.DrawString(this.menuFont, "1-Player", new Vector2(585, 370), Color.Black, 0.0f, new Vector2(0, 0), firstChoiceSize, 0, 0);
                sb.DrawString(this.menuFont, "2-Players", new Vector2(585, 440), Color.Black, 0.0f, new Vector2(0, 0), secondChoiceSize, 0, 0);
                sb.DrawString(this.menuFont, "Quit", new Vector2(585, 510), Color.Black, 0.0f, new Vector2(0, 0), thirdChoiceSize, 0, 0);
            }
            else if (gameStatus == 1) //Game Over Menu
            {
                if (enemyKilled >= 16)
                {
                    inMenu = true;
                    sb.DrawString(this.menuFont, "You Win!", new Vector2(420, 180), Color.Red);
                    sb.DrawString(this.menuFont, "You stopped " + (enemyKilled - (3 - lifeCount))
                                        + "/32" + " of the enemies."
                                        , new Vector2(280, 315), Color.Black, 0.0f, new Vector2(0, 0), 0.5f, 0, 0);
                    
                }
                else
                {
                    inMenu = true;
                    gameOver = true;
                    sb.DrawString(this.menuFont, "Game Over!", new Vector2(420, 230), Color.Red);
                }
                sb.DrawString(this.menuFont, "Replay", new Vector2(585, 420), Color.Black, 0.0f, new Vector2(0,0), firstChoiceSize, 0, 0);
                sb.DrawString(this.menuFont, "Quit", new Vector2(585,500), Color.Black, 0.0f, new Vector2(0, 0), secondChoiceSize, 0, 0);
            } 
        }
    }
}
