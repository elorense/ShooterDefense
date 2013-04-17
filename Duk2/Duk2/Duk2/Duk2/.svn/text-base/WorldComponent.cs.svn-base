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
    class AnimatedObject
    {
        public int Width { get; private set; }
        public bool Looping { get; set; }
        public Vector2 Position { get; set; }
        public float TimePerFrame { get; set; }
        private Texture2D mTexture;
        private int mCurrentFrame;
        private float mCurrentFrameTime;
        private int mNumFrames;

        public AnimatedObject(int imageWidth, Texture2D texture, bool looping)
        {
            Width = imageWidth;
            mTexture = texture;
            Looping = looping;
            mCurrentFrame = 0;
            TimePerFrame = 0.3f;
            mCurrentFrameTime = 0;
            mNumFrames = texture.Width / imageWidth;
            Position = new Vector2(0, 0);
        }

        virtual public void Update(GameTime gameTime)
        {
            mCurrentFrameTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (mCurrentFrameTime >= TimePerFrame)
            {
                mCurrentFrameTime = 0;
                if (mCurrentFrame == mNumFrames - 1)
                {
                    if (Looping)
                    {
                        mCurrentFrame = 0;
                    }
                }
                else
                {
                    mCurrentFrame++;
                }
            }
        }

        Rectangle FrameToRectangle(int frameNum)
        {
            return new Rectangle(frameNum * Width, 0, Width, mTexture.Height);
        }

        virtual public void Draw(SpriteBatch s)
        {
           s.Draw(mTexture, Position, FrameToRectangle(mCurrentFrame), Color.White);
        } 
    }

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class WorldComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {

        public Texture2D treasureChest, minerSprite;
        public List<Texture2D> minerList;
        public Rectangle treasureBounds { get; set; }
        public int lifeCount { get; set; }
        //public ProjectileManager Projectiles { get; set; }
        private AnimatedObject miners, otherMiner;

        Texture2D gunIco, superGunIco, whiteBar, explosion;

        public WorldComponent(Game game)
            : base(game)
        {   
           // Projectiles = new ProjectileManager();
            LoadContent();
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            
            treasureChest = Game.Content.Load<Texture2D>("treasure");
            
            Texture2D minerRTexture = Game.Content.Load<Texture2D>("minerAnimation");
            Texture2D minerLTexture = Game.Content.Load<Texture2D>("minerAnimFlipped");

            gunIco = Game.Content.Load<Texture2D>("gun");
            superGunIco = Game.Content.Load<Texture2D>("supergun");
            explosion = Game.Content.Load<Texture2D>("explosion");
            whiteBar = Game.Content.Load<Texture2D>("white");
            miners = new AnimatedObject(100, minerRTexture, true);
            otherMiner = new AnimatedObject(100, minerLTexture, true);

            miners.Position = new Vector2(565, 340);
            otherMiner.Position = new Vector2(642, 340); 
            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            //Projectiles.LoadContent(Game.Content);
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            //Projectiles.Update(gameTime);
            miners.Update(gameTime);
            otherMiner.Update(gameTime);
            base.Update(gameTime);
            
        }

        public virtual void Draw(SpriteBatch sb, int lifeCount)
        {
            if( lifeCount > 0)
                sb.Draw(treasureChest, new Vector2(610, 340), Color.White);
            if(lifeCount > 1)
                miners.Draw(sb);
            if(lifeCount > 2)
                otherMiner.Draw(sb);
            sb.Draw(whiteBar, new Vector2(0, 735), Color.White);
            sb.Draw(gunIco, new Vector2(0, 735), Color.White);
            sb.Draw(superGunIco, new Vector2(45, 735), Color.White);
            sb.Draw(explosion, new Vector2(90, 735), Color.White);
            //Projectiles.Draw(sb);
        }
    }
}
