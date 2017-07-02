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
using System.Threading;

namespace Flocking
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Rectangle backgroundRectangle;
        Texture2D CanyonEmpty;
        Texture2D CanyonNarrowing;
        Texture2D CanyonSplit;
        Texture2D SavannahPlain;
        Texture2D SavannahRiver;
        Texture2D Arrow;
        Texture2D RedArrow;
        Texture2D BlueArrow;
        List<Herd> Flock = new List<Herd>();
        System.Random r = new System.Random();
        public int HerdCount = 0; 
        public int ScreenWidth = 0; 
        public int ScreenHeight = 0;
        public int millisecondsPerFrame = 50; //1 seconf
        public int timeSinceLastUpdate = 0; //Accumulate the elapsed time
         

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            CanyonEmpty = Content.Load<Texture2D>("CanyonEmpty");
            CanyonNarrowing = Content.Load<Texture2D>("CanyonNarrowing");
            CanyonSplit = Content.Load<Texture2D>("CanyonSplit");
            SavannahPlain = Content.Load<Texture2D>("SavannahPlain");
            SavannahRiver = Content.Load<Texture2D>("SavannahRiver");
            Arrow = Content.Load<Texture2D>("Arrow");
            RedArrow = Content.Load<Texture2D>("RedArrow");
            BlueArrow = Content.Load<Texture2D>("ArrowBlue");
            backgroundRectangle = new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height); // size of screen display
            ScreenWidth = Window.ClientBounds.Width;
            ScreenHeight = Window.ClientBounds.Height;
            for ( int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Flock.Add(new Herd());
                    HerdCount++;
                    Flock[HerdCount - 1].HerdVector.Y += (i * 10);
                    Flock[HerdCount - 1].HerdVector.X += (j * 50);
                    Flock[HerdCount - 1].Hunger = r.Next(5000) + 5000;
                } 
            }
        }


        protected override void UnloadContent()
        {

        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            timeSinceLastUpdate += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timeSinceLastUpdate >= millisecondsPerFrame)
            {
                timeSinceLastUpdate = 0;
                Cohesion();
                Alignment();
                Seperate();
                foreach (Herd _Herd in Flock)
                {
                    if (_Herd.CurrentState == FlockState.Flock)
                    {
                        _Herd.ChangeInX += 1;
                        _Herd.Hunger -= 1;
                        if (_Herd.Hunger < 50) _Herd.CurrentState = FlockState.Graze;
                    }
                    else if (_Herd.CurrentState == FlockState.Graze)
                    {
                        _Herd.Hunger += 10;
                        if (_Herd.Hunger > 1000) _Herd.CurrentState = FlockState.Flock;
                    }
                    _Herd.HerdVector.X += _Herd.ChangeInX;
                    _Herd.HerdVector.Y += _Herd.ChangeInY;
                    if (_Herd.ChangeInX != 0 && _Herd.ChangeInY != 0)
                    {
                        _Herd.RotationAngle = Math.Atan((double)_Herd.ChangeInY / (double)_Herd.ChangeInX);
                    }
                    else if (_Herd.ChangeInX == 0 && _Herd.ChangeInY > 0) _Herd.RotationAngle = Math.PI / 2;
                    else if (_Herd.ChangeInX == 0 && _Herd.ChangeInY < 0) _Herd.RotationAngle = Math.PI * 1.5;
                    else if (_Herd.ChangeInY == 0 && _Herd.ChangeInX > 0) _Herd.RotationAngle = 0;
                    else if (_Herd.ChangeInY == 0 && _Herd.ChangeInX < 0) _Herd.RotationAngle = Math.PI;
                    _Herd.ChangeInX = 0;
                    _Herd.ChangeInY = 0;
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            spriteBatch.Draw(SavannahRiver, backgroundRectangle, Color.White);
            for (int i = 0; i < HerdCount; i++) 
            {
                if (i == 1) spriteBatch.Draw(RedArrow, Flock[i].HerdRectangle, null, Color.White, (float)Flock[i].RotationAngle, Flock[i].Center, SpriteEffects.None, 0);
                else spriteBatch.Draw(Arrow, Flock[i].HerdRectangle, null, Color.White, (float)Flock[i].RotationAngle, Flock[i].Center, SpriteEffects.None, 0);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public void Seperate() 
        {
            int i = 0;
            foreach (Herd _Herd in Flock)
            {
                for (int j = 0; j < HerdCount; j++)
                {
                    if (i != j && _Herd.CurrentState != FlockState.Graze)// && Flock[j].CurrentState != FlockState.Graze) 
                    {
                        float _x = 0, _y = 0;
                        if (_Herd.HerdVector.X > Flock[j].HerdVector.X) _x = _Herd.HerdVector.X - Flock[j].HerdVector.X;
                        else _x = Flock[j].HerdVector.X - _Herd.HerdVector.X;
                        if (_Herd.HerdVector.Y > Flock[j].HerdVector.Y) _y = _Herd.HerdVector.Y - Flock[j].HerdVector.Y;
                        else _y = Flock[j].HerdVector.Y - _Herd.HerdVector.Y;
                        float distance = (float)Math.Sqrt(_x * _x + _y * _y);
                        if (distance < 11) 
                        {
                            if (Flock[j].CurrentState != FlockState.Graze)
                            {
                                float SeperateX = 0, SeperateY = 0;
                                SeperateX = _x / 2;
                                SeperateY = _y / 2;
                                if (_Herd.HerdVector.X > Flock[j].HerdVector.X)
                                {
                                    _Herd.ChangeInX += SeperateX;
                                    Flock[j].ChangeInX -= SeperateX;
                                    _Herd.Hunger -= (int)SeperateX;
                                    Flock[j].Hunger -= (int)SeperateX;
                                }
                                else if (_Herd.HerdVector.X < Flock[j].HerdVector.X)
                                {
                                    _Herd.ChangeInX -= SeperateX;
                                    Flock[j].ChangeInX += SeperateX;
                                    _Herd.Hunger -= (int)SeperateX;
                                    Flock[j].Hunger -= (int)SeperateX;
                                }
                                else
                                {
                                    _Herd.ChangeInX -= 5;
                                    Flock[j].ChangeInX += 5; ;
                                    _Herd.Hunger -= 5;
                                    Flock[j].Hunger -= 5;
                                }

                                if (_Herd.HerdVector.Y > Flock[j].HerdVector.Y)
                                {
                                    _Herd.ChangeInY += SeperateY;
                                    Flock[j].ChangeInY -= SeperateY;
                                    _Herd.Hunger -= (int)SeperateY;
                                    Flock[j].Hunger -= (int)SeperateY;
                                }
                                else if (_Herd.HerdVector.Y < Flock[j].HerdVector.Y)
                                {
                                    _Herd.ChangeInY -= SeperateY;
                                    Flock[j].ChangeInY += SeperateY;
                                    _Herd.Hunger -= (int)SeperateY;
                                    Flock[j].Hunger -= (int)SeperateY;
                                }
                                else
                                {
                                    _Herd.ChangeInY -= 5;
                                    Flock[j].ChangeInY += 5;
                                    _Herd.Hunger -= 5;
                                    Flock[j].Hunger -= 5;
                                }
                            }
                            else 
                            {
                                if (_Herd.HerdVector.X > Flock[j].HerdVector.X)
                                {
                                    _Herd.ChangeInX += _x;
                                    _Herd.Hunger -= (int)_x;
                                }
                                else if (_Herd.HerdVector.X < Flock[j].HerdVector.X)
                                {
                                    _Herd.ChangeInX -= _x;
                                    _Herd.Hunger -= (int)_x;
                                }
                                else
                                {
                                    _Herd.ChangeInX -= 10;
                                    _Herd.Hunger -= 10;
                                }
                                if (_Herd.HerdVector.Y > Flock[j].HerdVector.Y)
                                {
                                    _Herd.ChangeInY += _y;
                                    _Herd.Hunger -= (int)_y;
                                }
                                else if (_Herd.HerdVector.Y < Flock[j].HerdVector.Y)
                                {
                                    _Herd.ChangeInY -= _y;
                                    _Herd.Hunger -= (int)_y;
                                }
                                else
                                {
                                    _Herd.ChangeInY -= 10;
                                    _Herd.Hunger -= 10;
                                }  
                            }
                        }
                    }
                }
                _Herd.UpdateRectangle(ScreenWidth, ScreenHeight);
                i++;
            }
        }

        public void Cohesion() 
        {
            int i = 0;
            foreach (Herd _Herd in Flock)
            {
                if (_Herd.CurrentState != FlockState.Graze)
                {
                    List<Herd> Locals = new List<Herd>();
                    int NieghbourCount = 1;
                    float TotalX = 0, TotalY = 0, AverageX = 0, AverageY = 0;
                    for (int j = 0; j < HerdCount; j++)
                    {
                        if (i != j && Flock[j].CurrentState != FlockState.Graze)
                        {
                            float _x = 0, _y = 0;
                            if (_Herd.HerdVector.X > Flock[j].HerdVector.X) _x = _Herd.HerdVector.X - Flock[j].HerdVector.X;
                            else _x = Flock[j].HerdVector.X - _Herd.HerdVector.X;
                            if (_Herd.HerdVector.Y > Flock[j].HerdVector.Y) _y = _Herd.HerdVector.Y - Flock[j].HerdVector.Y;
                            else _y = Flock[j].HerdVector.Y - _Herd.HerdVector.Y;
                            float distance = (float)Math.Sqrt(_x * _x + _y * _y);
                            if (distance < 30)
                            {
                                NieghbourCount++;
                                TotalX += Flock[j].HerdVector.X;
                                TotalY += Flock[j].HerdVector.Y;
                            }
                        }
                    }
                    TotalX = TotalX + _Herd.HerdVector.X;
                    TotalY = TotalY + _Herd.HerdVector.Y;
                    AverageX = TotalX / (float)NieghbourCount;
                    AverageY = TotalY / (float)NieghbourCount;
                    if (AverageX > _Herd.HerdVector.X) _Herd.ChangeInX += 1;
                    else if (AverageX < _Herd.HerdVector.X) _Herd.ChangeInX -= 1; 
                    if (AverageY > _Herd.HerdVector.Y) _Herd.ChangeInY += 1; 
                    else if (AverageY < _Herd.HerdVector.Y) _Herd.ChangeInY -= 1; 
                    _Herd.Hunger -= 1;
                    i++;
                }
            }
        }

        public void Alignment() 
        {
            float TotalX = 0, TotalY = 0, AverageX = 0, AverageY = 0;
            foreach (Herd _Herd in Flock)
            {
                if (_Herd.CurrentState != FlockState.Graze)
                {
                    TotalX = TotalX + _Herd.HerdVector.X;
                    TotalY = TotalY + _Herd.HerdVector.Y;
                }
            }
            AverageX = TotalX / (float)HerdCount;
            AverageY = TotalY / (float)HerdCount;
            foreach (Herd _Herd in Flock)
            {
                if (_Herd.CurrentState != FlockState.Graze)
                {
                    if (AverageX > _Herd.HerdVector.X) _Herd.ChangeInX += 1; 
                    else if (AverageX < _Herd.HerdVector.X) _Herd.ChangeInX -= 1; 
                    if (AverageY > _Herd.HerdVector.Y) _Herd.ChangeInY += 1; 
                    else if (AverageY < _Herd.HerdVector.Y) _Herd.ChangeInY -= 1; 
                    _Herd.Hunger -= 1;
                }
            }
        }
    }
}
