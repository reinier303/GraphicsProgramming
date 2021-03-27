using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Threading;

namespace GraphicsProgramming
{
    class Lesson1 : Lesson
    {
        VertexPositionColor[] vertices = 
        {
            //Square Front
            new VertexPositionColor(new Vector3(-0.5f, 0.5f, 0.5f), Color.Turquoise),
            new VertexPositionColor(new Vector3(0.5f, -0.5f, 0.5f), Color.Pink),
            new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0.5f), Color.Blue),
            new VertexPositionColor(new Vector3(0.5f, 0.5f, 0.5f), Color.Coral),

            //Square Back
            new VertexPositionColor(new Vector3(-0.5f, 0.5f, -0.5f), Color.Turquoise),
            new VertexPositionColor(new Vector3(0.5f, -0.5f, -0.5f), Color.Pink),
            new VertexPositionColor(new Vector3(-0.5f, -0.5f, -0.5f), Color.Blue),
            new VertexPositionColor(new Vector3(0.5f, 0.5f, -0.5f), Color.Coral),

            //Square Left
            new VertexPositionColor(new Vector3(-0.5f, 0.5f, 0.5f), Color.Turquoise),
            new VertexPositionColor(new Vector3(-0.5f, -0.5f, -0.5f), Color.Pink),
            new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0.5f), Color.Blue),
            new VertexPositionColor(new Vector3(-0.5f, 0.5f, -0.5f), Color.Coral),

            //Square Right
            new VertexPositionColor(new Vector3(0.5f, 0.5f, 0.5f), Color.Turquoise),
            new VertexPositionColor(new Vector3(0.5f, -0.5f, -0.5f), Color.Pink),
            new VertexPositionColor(new Vector3(0.5f, -0.5f, 0.5f), Color.Blue),
            new VertexPositionColor(new Vector3(0.5f, 0.5f, -0.5f), Color.Coral),

            //Square Up
            new VertexPositionColor(new Vector3(-0.5f, 0.5f, -0.5f), Color.Turquoise),
            new VertexPositionColor(new Vector3(0.5f, 0.5f, 0.5f), Color.Pink),
            new VertexPositionColor(new Vector3(-0.5f, 0.5f, 0.5f), Color.Blue),
            new VertexPositionColor(new Vector3(0.5f, 0.5f, -0.5f), Color.Coral),

            //Square Up
            new VertexPositionColor(new Vector3(-0.5f, -0.5f, -0.5f), Color.Turquoise),
            new VertexPositionColor(new Vector3(0.5f, -0.5f, 0.5f), Color.Pink),
            new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0.5f), Color.Blue),
            new VertexPositionColor(new Vector3(0.5f, -0.5f, -0.5f), Color.Coral),

        };

        int[] indices =
        {
            //Square Front F
            0,1,2,
            0,3,1,
            //Square Front B
            2,1,0,
            1,3,0,

            //Square Back F
            4,5,6,
            4,7,5,
            //Square Back B
            6,5,4,
            5,7,4,

            //Square Left F
            8,9,10,
            8,11,9,
            //Square Left B
            10,9,8,
            9,11,8,

            //Square Right F
            12,13,14,
            12,15,13,
            //Square Right B
            14,13,12,
            13,15,12,

            //Square Up F
            16,17,18,
            16,19,17,
            //Square Up B
            18,17,16,
            17,19,16,

            //Square Down F
            20,21,22,
            20,23,21,
            //Square Down B
            22,21,20,
            21,23,20,
        };

        BasicEffect effect;

        public override void LoadContent(ContentManager Content, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
        {
            effect = new BasicEffect(graphics.GraphicsDevice);
        }

        public override void Draw(GameTime gameTime, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
        {
            GraphicsDevice device = graphics.GraphicsDevice;

            device.Clear(Color.Black);

            effect.World = Matrix.Identity * Matrix.CreateRotationY((float)gameTime.TotalGameTime.TotalSeconds * 1.5f);

            effect.View = Matrix.CreateLookAt(-Vector3.Forward * 2 + Vector3.Up, Vector3.Zero, Vector3.Up);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView((MathF.PI / 180f) * 65f, device.Viewport.AspectRatio, 0.1f, 100f);
            
            effect.VertexColorEnabled = true;

            foreach(EffectPass pass in effect.CurrentTechnique.Passes)
            {
                effect.CurrentTechnique.Passes[0].Apply();
                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3);
            }

        }
    }
}
