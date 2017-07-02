using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Flocking
{
    public enum FlockState
    {
        Graze,
        Seperate,
        Flee,
        Flock,
        Move,
        Eat,
        Drink,
        TowardGoal,
        AwayThreat,
        AwayOthers
    }

    class Herd
    {
        public FlockState CurrentState = FlockState.Flock;
        public Vector2 HerdVector = new Vector2(10, 0);
        public Vector2 Center = new Vector2(0, 0);
        public Rectangle HerdRectangle = new Rectangle(10, 0, 10, 10);
        public float ChangeInX = 0, ChangeInY = 0;
        public int Hunger = 0;
        public double RotationAngle = 0;
        public Herd() 
        {
            HerdRectangle = new Rectangle((int)HerdVector.X, (int)HerdVector.Y, 10, 10);
        }

        public void UpdateRectangle(int p_Width, int p_Hieght)
        {
            if (HerdVector.X >= p_Width) 
            {
                HerdVector.X = 0;
            }
            else if (0 >= HerdVector.X) 
            {
                HerdVector.X = p_Width;
            }
            if (HerdVector.Y >= p_Hieght)
            {
                HerdVector.Y = 0;
            }
            else if (0 >= HerdVector.Y)
            {
                HerdVector.Y = p_Hieght;
            }
            HerdRectangle = new Rectangle((int)HerdVector.X, (int)HerdVector.Y, 10, 10);
            Center.X = HerdVector.X + 5;
            Center.Y = HerdVector.Y - 5;
        }
    }
}
