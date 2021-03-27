using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GraphicsProgramming
{
    class Lesson3 : Lesson
	{
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct VertexPositionColorNormal : IVertexType
		{
			public Vector3 Position;
			public Color Color;
			public Vector3 Normal;
			public Vector2 Texture;

			static readonly VertexDeclaration _vertexDeclaration = new VertexDeclaration
			(
				new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
				new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0),
				new VertexElement(16, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
				new VertexElement(28, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
			);

			VertexDeclaration IVertexType.VertexDeclaration => _vertexDeclaration;

			public VertexPositionColorNormal(Vector3 position, Color color, Vector3 normal, Vector2 texture)
			{
				Position = position;
				Color = color;
				Normal = normal;
				Texture = texture;
			}
		}

		Vector3 LightPosition = Vector3.Right * 2 + Vector3.Up * 2;

		private Effect myEffect;

		Model sphere, cube;
		Texture2D day, night, clouds;

		Texture2D moon;

		TextureCube sky;

		float yaw, pitch;
		int prevX, prevY;

        public override void Update(GameTime gameTime)
        {
			MouseState mState = Mouse.GetState();

			if(mState.LeftButton == ButtonState.Pressed)
            {
				//Update yaw and pitch
				yaw += (mState.X - prevX) *0.01f;
				pitch += (mState.Y - prevY) * 0.01f;

				pitch = MathF.Min(MathF.Max(pitch, -MathF.PI * 0.49f), MathF.PI * 0.49f);
			}

			prevX = mState.X;
			prevY = mState.Y;
		}

        public override void LoadContent(ContentManager Content, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
		{
			myEffect = Content.Load<Effect>("Lesson3");

			day = Content.Load<Texture2D>("day");
			night = Content.Load<Texture2D>("night");
			clouds = Content.Load<Texture2D>("clouds");
			moon = Content.Load<Texture2D>("2K_moon");
			sky = Content.Load<TextureCube>("sky_cube");

			sphere = Content.Load<Model>("uv_sphere");

			foreach(ModelMesh mesh in sphere.Meshes)
            {
				foreach(ModelMeshPart meshPart in mesh.MeshParts)
                {
					meshPart.Effect = myEffect;
                }
            }

			cube = Content.Load<Model>("cube");

			foreach (ModelMesh mesh in cube.Meshes)
			{
				foreach (ModelMeshPart meshPart in mesh.MeshParts)
				{
					meshPart.Effect = myEffect;
				}
			}
		}

		public override void Draw(GameTime gameTime, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
		{
			GraphicsDevice device = graphics.GraphicsDevice;

			float time = (float)gameTime.TotalGameTime.TotalSeconds;
			LightPosition = Vector3.Left * 200;//new Vector3(MathF.Cos(time), 0, MathF.Sin(time)) * 200;

			Vector3 cameraPos = -Vector3.Forward * 10;// + Vector3.Up * 5; //+ -Vector3.Right * 15;
			cameraPos = Vector3.Transform(cameraPos, Quaternion.CreateFromYawPitchRoll(-yaw, -pitch, 0));

			Matrix World = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
			Matrix View = Matrix.CreateLookAt(cameraPos, Vector3.Zero, Vector3.Up);


			myEffect.Parameters["World"].SetValue(World);
			myEffect.Parameters["View"].SetValue(View);
			myEffect.Parameters["Projection"].SetValue(Matrix.CreatePerspectiveFieldOfView((MathF.PI / 180f) * 35f, device.Viewport.AspectRatio, 0.001f, 1000f));

			myEffect.Parameters["DayTex"].SetValue(day);
			myEffect.Parameters["NightTex"].SetValue(night);
			myEffect.Parameters["CloudsTex"].SetValue(clouds);
			myEffect.Parameters["MoonTex"].SetValue(moon);
			myEffect.Parameters["SkyTex"].SetValue(sky);


			myEffect.Parameters["LightPosition"].SetValue(LightPosition);
			myEffect.Parameters["CameraPosition"].SetValue(cameraPos);

			myEffect.Parameters["Time"].SetValue(time);

			myEffect.CurrentTechnique.Passes[0].Apply();

			device.Clear(Color.Black);

			//Sky
			myEffect.CurrentTechnique = myEffect.Techniques["Sky"];

			device.DepthStencilState = DepthStencilState.None;
			device.RasterizerState = RasterizerState.CullNone;

			RenderModel(cube, Matrix.CreateTranslation(cameraPos));

			device.RasterizerState = RasterizerState.CullCounterClockwise;
			device.DepthStencilState = DepthStencilState.Default;

			//Earth
			myEffect.CurrentTechnique = myEffect.Techniques["Earth"];
			RenderModel(sphere, Matrix.CreateScale(0.01f) * Matrix.CreateRotationZ(time) * Matrix.CreateRotationY(MathF.PI/180 * 23) * World);

			//Moon
			myEffect.CurrentTechnique = myEffect.Techniques["Moon"];
			RenderModel(sphere, Matrix.CreateTranslation(new Vector3(8f, 0, 0)) * Matrix.CreateScale(0.0033f) * Matrix.CreateRotationZ(time - time * 0.0333333f) * World);

			//Moon2
			myEffect.CurrentTechnique = myEffect.Techniques["Moon2"];
			RenderModel(sphere, Matrix.CreateTranslation(new Vector3(12f, 9, 0)) * Matrix.CreateScale(0.0033f) * Matrix.CreateRotationZ(time - time * 0.0333333f) * World);
		}

		void RenderModel(Model m, Matrix parentMatrix)
		{
			Matrix[] transforms = new Matrix[m.Bones.Count];
			m.CopyAbsoluteBoneTransformsTo(transforms);

			myEffect.CurrentTechnique.Passes[0].Apply();

			foreach (ModelMesh mesh in m.Meshes)
			{
				myEffect.Parameters["World"].SetValue(parentMatrix * transforms[mesh.ParentBone.Index]);

				mesh.Draw();
			}
		}
	}
}
