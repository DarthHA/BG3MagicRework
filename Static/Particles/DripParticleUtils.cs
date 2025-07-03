using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
namespace BG3MagicRework.Static.Particles
{
    public static class DripParticleUtils
    {
        public static void NewParticle(this List<DripParticle> particles, Vector2 Pos, Vector2 Velocity, float Scale)
        {
            particles.Add(new DripParticle(Pos, Velocity, Scale));
        }

        public static void UpdateParticle(this List<DripParticle> particles, Vector2 Center, bool tileCollide = false)
        {
            foreach (DripParticle particle in particles)
            {
                particle.Position += particle.Velocity;
                if (particle.Timer > 0) particle.Timer++;
                if (particle.Timer == 0 && particle.Position.Y > Center.Y - Main.screenHeight / 2f)
                {
                    if (Collision.SolidCollision(particle.Position, 1, (int)(7f * particle.Scale)) && tileCollide)
                    {
                        particle.Velocity = Vector2.Zero;
                        particle.Timer++;
                    }
                }
            }

            for (int i = particles.Count - 1; i >= 0; i--)
            {
                if (particles[i].Timer > 30)
                {
                    particles.RemoveAt(i);
                }
                else if (particles[i].Timer == 0)
                {
                    int TilePosX = (int)(particles[i].Position.X / 16f);
                    int TilePosY = (int)(particles[i].Position.Y / 16f);
                    bool lavaWet = false;
                    if (TilePosX >= 0 && TilePosX < Main.maxTilesX && TilePosY >= 0 && TilePosY < Main.maxTilesY)
                    {
                        lavaWet = Main.tile[TilePosX, TilePosY].LiquidType == LiquidID.Lava && Main.tile[TilePosX, TilePosY].LiquidAmount > 0;
                    }
                    if (particles[i].Position.Y > Center.X + Main.screenHeight || lavaWet)
                    {
                        particles.RemoveAt(i);
                    }
                }
            }
        }

        public static void DrawParticle(this List<DripParticle> particles)
        {
            foreach (DripParticle particle in particles)
            {
                if (particle.Timer == 0)
                {
                    Color color1 = Lighting.GetColor((int)(particle.Position.X / 16f), (int)(particle.Position.Y / 16f), Color.LightBlue);
                    Color color2 = Lighting.GetColor((int)(particle.Position.X / 16f), (int)(particle.Position.Y / 16f), Color.DeepSkyBlue);
                    Utils.DrawLine(Main.spriteBatch,
                        particle.Position + new Vector2(0, -6) * particle.Scale
                        , particle.Position + new Vector2(0, 6) * particle.Scale,
                        color1, color2, 3);
                }
                else
                {
                    float alpha = MathHelper.Lerp(1, 0, particle.Timer / 30f);
                    float scale = MathHelper.Lerp(0, 1, particle.Timer / 30f);
                    Color color = Lighting.GetColor((int)(particle.Position.X / 16f), (int)(particle.Position.Y / 16f), Color.Cyan * alpha);
                    DrawRing(particle.Position, 40 * scale, 20 * scale * particle.Scale, color);
                }
            }

        }
        private static void DrawRing(Vector2 Center, float radius, float width, Color color)
        {
            Texture2D tex = TextureLibrary.Ribbon;
            List<CustomVertexInfo> bars = new();
            for (int i = 0; i < 60; i++)
            {
                float rot = MathHelper.TwoPi / 60f * i;
                Vector2 Pos1 = rot.ToRotationVector2() * (radius + width / 2f);
                Vector2 Pos2 = rot.ToRotationVector2() * (radius - width / 2f);
                Pos1.Y *= 0.3f;
                Pos2.Y *= 0.3f;
                bars.Add(new CustomVertexInfo(Center + Pos1 - Main.screenPosition, Color.White, new Vector3(1 / 60f * i, 0, 1f)));
                bars.Add(new CustomVertexInfo(Center + Pos2 - Main.screenPosition, Color.White, new Vector3(1 / 60f * i, 1, 1f)));
            }
            bars.Add(new CustomVertexInfo(Center + new Vector2(radius + width / 2f, 0) - Main.screenPosition, Color.White, new Vector3(1f, 0, 1f)));
            bars.Add(new CustomVertexInfo(Center + new Vector2(radius - width / 2f, 0) - Main.screenPosition, Color.White, new Vector3(1f, 1, 1f)));
            float len = radius * MathHelper.TwoPi;
            DrawUtils.DrawLoopTrail(tex, bars, color, 0.33f, 0, BlendState.Additive);
        }


    }

    public class DripParticle(Vector2 pos, Vector2 vel, float scale)
    {
        public Vector2 Position = pos;
        public Vector2 Velocity = vel;
        public float Scale = scale;
        public float Timer = 0;
    }
}
