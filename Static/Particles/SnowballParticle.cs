using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace BG3MagicRework.Static.Particles
{
    public static class SnowballParticleUtils
    {
        public static void NewParticle(this List<SnowballParticle> particles, Vector2 Pos, Vector2 Velocity, float Scale)
        {
            particles.Add(new SnowballParticle(Pos, Velocity, Scale));
        }

        public static void UpdateParticle(this List<SnowballParticle> particles, float VelDecrease = 0.97f, float Gravity = 1, float TimeSpeed = 1, bool tileCollide = false, List<TmpParticle> dusts = null)
        {
            foreach (SnowballParticle particle in particles)
            {
                particle.Velocity.X *= VelDecrease;
                particle.Velocity.Y += Gravity;
                particle.Position += particle.Velocity;
                particle.Timer += TimeSpeed;
                if (particle.Timer > 40)
                {
                    particle.Scale -= 0.01f;
                }
            }
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                if (tileCollide)
                {
                    if (Collision.SolidCollision(particles[i].Position - new Vector2(5, 5), 10, 10) || Collision.LavaCollision(particles[i].Position - new Vector2(5, 5), 10, 10))
                    {
                        if (dusts != null)
                        {
                            int count = Main.rand.Next(4) + 4;
                            for (int j = 0; j < count; j++)
                            {
                                Vector2 Vel = ((-particles[i].Velocity).ToRotation() + (Main.rand.NextFloat() - 0.5f) * MathHelper.Pi / 2f).ToRotationVector2() * Main.rand.Next(5, 10);
                                Vector2 Pos = particles[i].Position + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(1, 10);
                                float scale = 0.15f + 0.15f * Main.rand.NextFloat();
                                dusts.NewParticle(Pos, Vel, scale);
                            }
                        }
                        particles.RemoveAt(i);
                        continue;
                    }
                }
                if (particles[i].Scale < 0.01f)
                {
                    particles.RemoveAt(i);
                    continue;
                }
            }
        }

        public static void DrawParticle(this List<SnowballParticle> particles, bool IgnoreLight = false, float scaleModifier = 1f)
        {
            foreach (SnowballParticle particle in particles)
            {
                Texture2D tex = TextureLibrary.CrispCircle;
                Color drawColor1 = IgnoreLight ? Color.Cyan : Lighting.GetColor((int)(particle.Position.X / 16f), (int)(particle.Position.Y / 16f), Color.Cyan);
                Color drawColor2 = IgnoreLight ? Color.White : Lighting.GetColor((int)(particle.Position.X / 16f), (int)(particle.Position.Y / 16f), Color.White);
                Main.spriteBatch.Draw(tex, particle.Position - Main.screenPosition, null, drawColor1 * 0.6f, 0, tex.Size() / 2f, particle.Scale * scaleModifier, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(tex, particle.Position - Main.screenPosition, null, drawColor2 * 0.6f, 0, tex.Size() / 2f, particle.Scale * 0.9f * scaleModifier, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(tex, particle.Position - Main.screenPosition, null, drawColor2 * 0.5f, 0, tex.Size() / 2f, particle.Scale * 0.8f * scaleModifier, SpriteEffects.None, 0);
            }
        }

    }

    public class SnowballParticle(Vector2 pos, Vector2 vel, float scale)
    {
        public Vector2 Position = pos;
        public Vector2 Velocity = vel;
        public float Scale = scale;
        public float Timer = 0;
        public List<Vector2> OldPos = new();
    }
}
