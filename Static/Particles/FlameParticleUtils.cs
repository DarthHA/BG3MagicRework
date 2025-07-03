using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
namespace BG3MagicRework.Static.Particles
{
    public static class FlameParticleUtils
    {
        public static void NewParticle(this List<FlameParticle> particles, Vector2 Pos, Vector2 Velocity, float Scale)
        {
            particles.Add(new FlameParticle(Pos, Velocity, Scale));
        }

        public static void UpdateParticle(this List<FlameParticle> particles, float VelDecrease = 0.93f, bool tileCollide = false)
        {
            foreach (FlameParticle particle in particles)
            {
                particle.Rotation += 0.01f;
                particle.Timer++;
                particle.Frame = (int)(particle.Timer * 0.4f);
                particle.Scale = MathHelper.Lerp(0.25f, 1f, particle.Timer / 60f) * particle.InitialScale;
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

        public static void DrawParticle(this List<FlameParticle> particles, float scaleModifier = 1f)
        {
            foreach (FlameParticle particle in particles)
            {
                Texture2D tex = TextureLibrary.RandomSmoke;
                Rectangle rectangle = GetTexRect(particle.Frame);
                Color color1, color2;
                color1 = Color.Lerp(Color.Yellow, Color.OrangeRed, particle.Timer / 60f);
                color2 = Color.Lerp(Color.OrangeRed, Color.Red, particle.Timer / 60f);
                color1 = Color.White;
                Main.spriteBatch.Draw(tex, particle.Position - Main.screenPosition, rectangle, color1 * 0.85f, particle.Rotation, rectangle.Size() / 2, particle.Scale * scaleModifier, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(tex, particle.Position - Main.screenPosition, rectangle, color2 * 0.85f, particle.Rotation, rectangle.Size() / 2, particle.Scale * scaleModifier * 1.5f, SpriteEffects.None, 0);
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

    public class FlameParticle(Vector2 pos, Vector2 vel, float scale)
    {
        public Vector2 Position = pos;
        public Vector2 Velocity = vel;
        public float Rotation = Main.rand.NextFloat() * MathHelper.TwoPi;
        public float InitialScale = scale;
        public float Scale = 1f;
        public int Frame = 0;
        public int Timer = 0;
    }
}
