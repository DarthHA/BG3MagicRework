using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
namespace BG3MagicRework.Static.Particles
{
    public static class SmokeParticleUtils
    {
        public static void NewParticle(this List<SmokeParticle> particles, Vector2 Pos, Vector2 Velocity, float Scale, Color Color)
        {
            particles.Add(new SmokeParticle(Pos, Velocity, Scale, Color));
        }

        public static void UpdateParticle(this List<SmokeParticle> particles, float VelDecrease = 0.93f, float TimeSpeed = 1, bool tileCollide = false)
        {
            foreach (SmokeParticle particle in particles)
            {
                particle.Rotation += 0.01f;
                particle.Timer += TimeSpeed;
                particle.Frame = (int)(particle.Timer * 0.4f);
                particle.Scale = MathHelper.Lerp(1f, 0.5f, particle.Timer / 60f) * particle.InitialScale;
                particle.Velocity *= VelDecrease;

                particle.Position += particle.Velocity;
            }
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                if (particles[i].Timer > 60)
                {
                    particles.RemoveAt(i);
                    continue;
                }
                if (tileCollide && Collision.SolidCollision(particles[i].Position, 1, 1))
                {
                    particles.RemoveAt(i);
                    continue;
                }
            }
        }

        public static void DrawParticle(this List<SmokeParticle> particles, bool IgnoreLight = false, float scaleModifier = 1f)
        {
            foreach (SmokeParticle particle in particles)
            {
                Texture2D tex = TextureLibrary.RandomSmoke;
                Rectangle rectangle = GetTexRect(particle.Frame);
                float opalcity = MathHelper.Lerp(0, 1, particle.Timer / 60f);
                Color lightColor = IgnoreLight ? particle.InitialColor : Lighting.GetColor((int)(particle.Position.X / 16f), (int)(particle.Position.Y / 16f), particle.InitialColor);
                Main.spriteBatch.Draw(tex, particle.Position - Main.screenPosition, rectangle, lightColor, particle.Rotation, rectangle.Size() / 2, particle.Scale * 1.2f * scaleModifier, SpriteEffects.None, 0);
            }
        }

        private static Rectangle GetTexRect(int index)
        {
            Texture2D tex = TextureLibrary.RandomSmoke;
            index = Math.Clamp(index, 0, 24);
            int dwidth = tex.Width / 5;
            int dheight = tex.Height / 5;
            int x = index % 5;
            int y = index / 5;
            return new Rectangle(x * dwidth, y * dheight, dwidth, dheight);
        }

    }

    public class SmokeParticle(Vector2 pos, Vector2 vel, float scale, Color color)
    {
        public Vector2 Position = pos;
        public Vector2 Velocity = vel;
        public float Rotation = Main.rand.NextFloat() * MathHelper.TwoPi;
        public float InitialScale = scale;
        public float Scale = 1f;
        public int Frame = 0;
        public float Timer = 0;
        public Color InitialColor = color;
    }
}
