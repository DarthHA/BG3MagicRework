using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
namespace BG3MagicRework.Static.Particles
{
    public static class TmpParticleUtils
    {
        public static void NewParticle(this List<TmpParticle> particles, Vector2 Pos, Vector2 Velocity, float Scale)
        {
            particles.Add(new TmpParticle(Pos, Velocity, Scale));
        }

        public static void UpdateParticle(this List<TmpParticle> particles, float VelDecrease = 1, float ScaleDecrease = 1, bool tileCollide = false)
        {
            foreach (TmpParticle particle in particles)
            {
                particle.Position += particle.Velocity;
                particle.Velocity *= VelDecrease;
                particle.Scale *= ScaleDecrease;
            }
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                if (particles[i].Scale < 0.05f)
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

        public static void DrawParticle(this List<TmpParticle> particles, Texture2D texture, Color color, bool IgnoreLight, Vector2 scaleModifier)
        {
            foreach (TmpParticle particle in particles)
            {
                Color lightcolor = IgnoreLight ? color : Lighting.GetColor((int)(particle.Position.X / 16f), (int)(particle.Position.Y / 16f), color);
                Main.spriteBatch.Draw(texture, particle.Position - Main.screenPosition, null, lightcolor, particle.Velocity.ToRotation(), texture.Size() / 2f, particle.Scale * scaleModifier, SpriteEffects.None, 0);
            }
        }
    }

    public class TmpParticle(Vector2 pos, Vector2 vel, float scale)
    {
        public Vector2 Position = pos;
        public Vector2 Velocity = vel;
        public float Scale = scale;
    }
}
