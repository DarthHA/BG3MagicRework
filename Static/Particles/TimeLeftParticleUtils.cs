using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace BG3MagicRework.Static.Particles
{
    public static class TimeLeftParticleUtils
    {
        public static void NewParticle(this List<TimeLeftParticle> particles, Vector2 Pos, int timeLeft)
        {
            particles.Add(new TimeLeftParticle(Pos, timeLeft));
        }

        public static void UpdateParticle(this List<TimeLeftParticle> particles, bool tileCollide = false)
        {
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                particles[i].TimeLeft--;
                if (particles[i].TimeLeft <= 0)
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

        public static void DrawParticle(this List<TimeLeftParticle> particles, Texture2D texture, Color color, bool IgnoreLight = true, float scaleModifier = 0.045f)
        {
            foreach (TimeLeftParticle particle in particles)
            {
                float scale = scaleModifier * particle.TimeLeft / particle.FullTime;
                Color color1 = IgnoreLight ? color : Lighting.GetColor((int)(particle.Position.X / 16f), (int)(particle.Position.Y / 16f), color);
                Main.spriteBatch.Draw(texture, particle.Position - Main.screenPosition, null, color1, 0, texture.Size() / 2f, scale, SpriteEffects.None, 0);
            }
        }
    }

    public class TimeLeftParticle(Vector2 pos, int timeLeft = 15)
    {
        public Vector2 Position = pos;
        public float TimeLeft = timeLeft;
        internal float FullTime = timeLeft;
    }
}
