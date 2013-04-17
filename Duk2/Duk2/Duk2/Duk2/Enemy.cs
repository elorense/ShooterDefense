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
    public class BasicEnemy
    {
        List<List<Vector2[]>> stages;
        Vector2[] path;
        int pathIndex;
        String fileName;
        float enemySpeed = 0.3f;
        
        int spawnPoint { get; set; }
        Rectangle chest;
        int lifeWidth { get; set; }
        int enemyMaxHealth = 30;
        public int stage { get; set; }
        Texture2D lifeBar;
        Rectangle lifeSize;

        public Vector2 enemyPosition { get; set; }
        public Texture2D enemyTexture { get; set; }
        public bool hitMiners;
        public int enemyLife { get; set; }
        public static int basicEnemyHeight { get; set; }
        public static int basicEnemyWidth { get; set; }
        
        //Animation Variables
        public int Width { get; private set; }
        public bool Looping { get; set; }
        public Vector2 Position { get; set; }
        public float TimePerFrame { get; set; }
        private int mCurrentFrame;
        private float mCurrentFrameTime;
        private int mNumFrames;

        public BasicEnemy(String fileName, ContentManager content, int spawnPoint, Rectangle treasureBounds, int stage)
        {
            this.stage = stage;
            stages = new List<List<Vector2[]>>();
            createPaths();
            enemyTexture = content.Load<Texture2D>(fileName);
            lifeBar = content.Load<Texture2D>("life");
            basicEnemyWidth = enemyTexture.Width;
            lifeWidth = (basicEnemyWidth / 2);
            basicEnemyHeight = enemyTexture.Height;
            this.spawnPoint = spawnPoint;
            chest = new Rectangle(560, 310, 100, 100);
            pathIndex = 0;
            this.fileName = fileName;
            enemyInit();
            
            //Animation Variables
            Width = 100;
            Looping = true;
            mCurrentFrame = 0;
            TimePerFrame = 0.3f;
            mCurrentFrameTime = 0;
            mNumFrames = enemyTexture.Width / 100;
            Position = new Vector2(0, 0);
        }

        public void enemyInit()
        {
            if (fileName.ToLower().Contains("super"))
            {
                enemyLife = 60;
                enemyMaxHealth = 60;
                enemySpeed = 0.08f;
                path = stages.ElementAt(stage).ElementAt(spawnPoint +4);
                enemyPosition = path.ElementAt(0);
                pathIndex++;
            }
            else 
            {
                enemyLife = 30;
                enemyMaxHealth = 30;
                enemySpeed = 0.25f;
                path = stages.ElementAt(stage).ElementAt(spawnPoint);
                enemyPosition = path.ElementAt(0);
                pathIndex++;
            }
            
        }

        public void createPaths() 
        {
            List<Vector2[]> stagePaths = new List<Vector2[]>();
            //0-3 for basic enemy
            stagePaths.Add(new Vector2[3] { new Vector2(0 ,0), new Vector2(560, 0), new Vector2(560, 340) });
            stagePaths.Add(new Vector2[3] { new Vector2(0, 660), new Vector2(560, 660), new Vector2(560, 340) });
            stagePaths.Add(new Vector2[3] { new Vector2(1120, 660), new Vector2(560, 660), new Vector2(560, 340) });
            stagePaths.Add(new Vector2[3] { new Vector2(1120, 0), new Vector2(560, 0), new Vector2(560, 340) });
            
            //4-6 for bosses
            stagePaths.Add(new Vector2[2] { new Vector2(0, 340), new Vector2(560, 340) });
            stagePaths.Add(new Vector2[2] { new Vector2(1120, 340), new Vector2(560, 340) });

            stages.Add(stagePaths);
            stagePaths = new List<Vector2[]>();
            stagePaths.Add(new Vector2[3] { new Vector2(0, 0), new Vector2(0, 340), new Vector2(560, 340) });
            stagePaths.Add(new Vector2[3] { new Vector2(0, 660), new Vector2(0, 340), new Vector2(560, 340) });
            stagePaths.Add(new Vector2[3] { new Vector2(1120, 660), new Vector2(1120, 340), new Vector2(560, 340) });
            stagePaths.Add(new Vector2[3] { new Vector2(1120, 0), new Vector2(1120, 340), new Vector2(560, 340) });

            stagePaths.Add(new Vector2[2] { new Vector2(560, 0), new Vector2(560, 340) });
            stagePaths.Add(new Vector2[2] { new Vector2(560, 620), new Vector2(560, 340) });
            stages.Add(stagePaths);
        }

        public void LoadContent(ContentManager content)
        {
        }

        public Rectangle FrameToRectangle(int frameNum)
        {
            return new Rectangle(frameNum * Width, 0, Width, enemyTexture.Height);
        }

        public void enemyUpdate(GameTime gameTime) 
        {
            
            Rectangle basicEnBounds = new Rectangle(0, 0, 25, 25);
        
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
           
            Vector2 place = enemyPosition; 
            float distToTravel = enemySpeed * (float)gameTime.ElapsedGameTime.Milliseconds;
            if (distToTravel > (place - path[pathIndex]).Length())
            {
                place = path[pathIndex];
                distToTravel -= (place - path[pathIndex]).Length();
                pathIndex++;
                if (pathIndex >= path.Length)
                    hitMiners = true;
            }
            if (pathIndex < path.Length) 
            {
                Vector2 disp = path[pathIndex] - place;
                disp.Normalize();
                place += disp * distToTravel;
            }
          
            enemyPosition = place;  
        }

        public void enemyDraw(SpriteBatch sb)
        {
            Rectangle reshape = new Rectangle((int)enemyPosition.X, (int) enemyPosition.Y, (int) (enemyTexture.Width), (int)(enemyTexture.Height * 1.2));
            float calcLifeWidth = ((float) enemyLife / (float) enemyMaxHealth) * ((float) basicEnemyWidth / 2);
            lifeWidth = (int) calcLifeWidth;
            lifeSize = new Rectangle((int)enemyPosition.X + (basicEnemyWidth / 4), (int)enemyPosition.Y, lifeWidth, 5);
            
            sb.Draw(enemyTexture, reshape, FrameToRectangle(mCurrentFrame), Color.White);
            sb.Draw(lifeBar, lifeSize, Color.White);

        }
        
    }

    static class Shared
    {
        public static readonly Random Random = new Random();
    }

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Enemy : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public List<BasicEnemy> basicEnemyList;
        private float elapsed, bossElapsed;
        public int basicEnemyCount;
        public int bossCount;
        public int enemyKilled { get; set; }
        public int lifeCount { get; set; }
        public Rectangle treasureBounds;
        public int stage { get; set; }

        public Enemy(Game game, Rectangle treasureBounds, int stage)
            : base(game)
        {
            this.stage = stage;
            lifeCount = 3;
            basicEnemyList = new List<BasicEnemy>();
            LoadContent();
            this.treasureBounds = treasureBounds;
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            int posDecider = Shared.Random.Next(0, 4);
            if(posDecider == 0 || posDecider == 1)
                basicEnemyList.Add(new BasicEnemy("basicYeti", Game.Content, posDecider, treasureBounds, stage));
            else
                basicEnemyList.Add(new BasicEnemy ("basicYetiFlip", Game.Content, posDecider, treasureBounds, stage));
            base.Initialize();
        }

        protected override void LoadContent()
        {
            //foreach (BasicEnemy basicEn in basicEnemyList)
            //{
            //    basicEn.LoadContent(Game.Content);
            //}
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            int spawnTime = 1600;
            int bossSpawn = 3750;
            elapsed += gameTime.ElapsedGameTime.Milliseconds;
            bossElapsed += gameTime.ElapsedGameTime.Milliseconds;

            if (bossElapsed > bossSpawn && bossCount < 4)
            {
                bossElapsed = 0;
                int posDecider = Shared.Random.Next(0, 2);

                if (posDecider == 0 )
                    basicEnemyList.Add(new BasicEnemy("SuperYetiFlip", Game.Content, posDecider, treasureBounds, stage));
                else
                    basicEnemyList.Add(new BasicEnemy("superYeti", Game.Content, posDecider, treasureBounds, stage));

                bossCount++;
                
            }

            if (elapsed > spawnTime && basicEnemyCount < 12)
            {
                elapsed = 0;
                int posDecider = Shared.Random.Next(0, 4);
                
                if (posDecider == 0 || posDecider == 1)
                    basicEnemyList.Add(new BasicEnemy("basicYeti", Game.Content, posDecider, treasureBounds, stage));
                else
                    basicEnemyList.Add(new BasicEnemy("basicYetiFlip", Game.Content, posDecider, treasureBounds, stage));

                
                basicEnemyCount++;

            }

            for (int i = basicEnemyList.Count - 1; i >= 0; i--)
            {
                if (basicEnemyList[i].hitMiners)
                {
                    basicEnemyList.RemoveAt(i);
                    enemyKilled++;
                    lifeCount--;
                }
            }

            foreach (BasicEnemy basicEn in basicEnemyList)
            {
                basicEn.stage = stage;
                basicEn.enemyUpdate(gameTime);
                
            }

            base.Update(gameTime);
        }

        public virtual void Draw(SpriteBatch sb)
        {
            foreach(BasicEnemy basicEn in basicEnemyList)
            {
                basicEn.enemyDraw(sb);
            }
            
        }
    }
}
