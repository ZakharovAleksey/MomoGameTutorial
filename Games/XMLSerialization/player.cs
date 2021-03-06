﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLSerialization
{
    enum AnimationType
    {
        GO_RIGHT = 0,
        GO_LEFT = 1,
        GO_TOP = 2,
        GO_BOTTOM = 3
    }

    [Serializable]
    public class Player
    {
        #region Fields
        [NonSerialized]
        int possibleAnimationCount;

        [NonSerialized]
        Animation[] animationArray;

        [NonSerialized]
        int currentAnimationID = 0;

        [NonSerialized]
        Vector2 position;

        [NonSerialized]
        Vector2 velocity = new Vector2();
        [NonSerialized]
        bool isMoveing = false;


        // Point to move
        [NonSerialized]
        Rectangle destRectangle;
        Texture2D cube;


        #endregion

        #region Constructor

        public Player() { }

        public Player(int possibleAnimationCount, Vector2 position)
        {
            this.possibleAnimationCount = possibleAnimationCount;
            animationArray = new Animation[this.possibleAnimationCount];

            this.position = position;
        }

        #endregion

        #region Properties

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public int CurrentSpriteID
        {
            get { return currentAnimationID; }
            set { currentAnimationID = value; }
        }

        public bool IsMoveing
        {
            get { return isMoveing; }
            set { isMoveing = value; }
        }

        Rectangle RectangleOnScreen
        {
            get { return new Rectangle((int)position.X, (int)position.Y, animationArray[currentAnimationID].Width, animationArray[currentAnimationID].Height); }
        }

        #endregion

        #region Methods

        public void Initilize(ContentManager Content)
        {
            animationArray[(int)AnimationType.GO_RIGHT] = new Animation(Content, @"player\goRight", 4);
            animationArray[(int)AnimationType.GO_LEFT] = new Animation(Content, @"player\goLeft", 4);
            animationArray[(int)AnimationType.GO_TOP] = new Animation(Content, @"player\goUp", 4);
            animationArray[(int)AnimationType.GO_BOTTOM] = new Animation(Content, @"player\goDown", 4);


            // Point to move
            cube = Content.Load<Texture2D>(@"cube");
        }

        #region Movement Implementation


        #region KeyBoard Actions

        void goRight()
        {
            velocity.X = GameConstants.playerVelocityX;
            velocity.Y = 0;
            currentAnimationID = (int)AnimationType.GO_RIGHT;
            isMoveing = true;
        }

        void goLeft()
        {
            velocity.X = - GameConstants.playerVelocityX;
            velocity.Y = 0;
            currentAnimationID = (int)AnimationType.GO_LEFT;
            isMoveing = true;
        }

        void goTop()
        {
            velocity.Y = - GameConstants.playerVelocityX;
            velocity.X = 0;
            currentAnimationID = (int)AnimationType.GO_TOP;
            isMoveing = true;
        }

        void goBottom()
        {
            velocity.Y = GameConstants.playerVelocityX;
            velocity.X = 0;
            currentAnimationID = (int)AnimationType.GO_BOTTOM;
            isMoveing = true;
        }

        void idleImplamentation()
        {
            velocity.X = velocity.Y = 0.0f;
            isMoveing = false;
        }

        #endregion

        void boundaryConditionImplementation()
        {
            if (position.X <= 0)
                position.X = 0;
            if (position.X >= GameConstants.WindowWidth - animationArray[currentAnimationID].Width)
                position.X = GameConstants.WindowWidth - animationArray[currentAnimationID].Width;
            if (position.Y <= 0)
                position.Y = 0;
            if (position.Y >= GameConstants.WindowHeight - animationArray[currentAnimationID].Height)
                position.Y = GameConstants.WindowHeight - animationArray[currentAnimationID].Height;


        }

        #region Mouse Actions Implementation

        void OnMouseClick()
        {
            float weight = 0.1f;
            destRectangle = new Rectangle(Mouse.GetState().Position.X, Mouse.GetState().Position.Y, cube.Width, cube.Height);

            if (Mouse.GetState().Position.X > position.X)
                currentAnimationID = (int)AnimationType.GO_RIGHT;
            else if (Mouse.GetState().Position.X < position.X)
                currentAnimationID = (int)AnimationType.GO_LEFT;


            float tan = (Mouse.GetState().Position.Y - position.Y) / (Mouse.GetState().Position.X - position.X);
            float alpha = (float)Math.Atan2((Mouse.GetState().Position.Y - position.Y), (Mouse.GetState().Position.X - position.X));

            velocity.X = weight * (float)Math.Cos(alpha);
            velocity.Y = weight * (float)Math.Sin(alpha);

            isMoveing = true;
        }

        void stop()
        {
            if (isMoveing)
            {
                if (destRectangle.Intersects(this.RectangleOnScreen))
                {
                    velocity.X = 0.0f;
                    velocity.Y = 0.0f;
                }
            }
        }

        #endregion

        #endregion

        public void Update(GameTime gameTime)
        {
            position += velocity * gameTime.ElapsedGameTime.Milliseconds;

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                goRight();
            else if (Keyboard.GetState().IsKeyDown(Keys.Left))
                goLeft();
            else if (Keyboard.GetState().IsKeyDown(Keys.Up))
                goTop();
            else if (Keyboard.GetState().IsKeyDown(Keys.Down))
                goBottom();
            else
                idleImplamentation();

            // TODO: Mouse click implementation            
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                OnMouseClick();


            stop();
            boundaryConditionImplementation();

            animationArray[currentAnimationID].UpdateAnimation(gameTime, isMoveing);

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            animationArray[currentAnimationID].Draw(spriteBatch, position);
            spriteBatch.Draw(cube, destRectangle, Color.White);
        }

        #endregion
    }
}
