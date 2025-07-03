using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
namespace BG3MagicRework.Static.Particles
{
    public static class IceDripParticleUtils
    {
        public static void NewParticle(this List<IceDripParticle> particles, Vector2 Pos, Vector2 Velocity, float Scale, bool Snow = false)
        {
            particles.Add(new IceDripParticle(Pos, Velocity, Scale, Snow));
        }

        public static void UpdateParticle(this List<IceDripParticle> particles, Vector2 Center, bool tileCollide = false, List<TmpParticle> dusts = null)
        {
            foreach (IceDripParticle particle in particles)
            {
                particle.Position += particle.Velocity;
                if (particle.Timer > 0) particle.Timer++;
                if (particle.Timer == 0 && particle.Position.Y > Center.Y - Main.screenHeight / 2f)
                {
                    if (Collision.SolidCollision(particle.Position, 1, (int)(7f * particle.Scale)) && tileCollide)
                    {
                        if (Main.rand.NextBool())
                        {
                            particle.Velocity = Vector2.Zero;
                            particle.Timer++;
                        }
                        else
                        {
                            if (dusts != null)
                            {
                                int count = Main.rand.Next(4) + 4;
                                for (int j = 0; j < count; j++)
                                {
                                    Vector2 Vel = ((-particle.Velocity).ToRotation() + (Main.rand.NextFloat() - 0.5f) * MathHelper.Pi / 2f).ToRotationVector2() * Main.rand.Next(5, 10);
                                    Vector2 Pos = particle.Position + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(1, 10);
                                    float scale = 0.15f + 0.15f * Main.rand.NextFloat();
                                    dusts.NewParticle(Pos, Vel, scale);
                                }
                            }
                            particle.Velocity = Vector2.Zero;
                            particle.Timer += 31;
                        }
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

        public static void DrawParticle(this List<IceDripParticle> particles)
        {
            foreach (IceDripParticle particle in particles)
            {
                if (particle.Timer == 0)
                {
                    if (particle.Frame >= 3)
                    {
                        EasyDraw.AnotherDraw(BlendState.Additive);
                        Color color = Lighting.GetColor((int)(particle.Position.X / 16f), (int)(particle.Position.Y / 16f), Color.SkyBlue);
                        Texture2D tex = TextureLibrary.SnowFlake;
                        Main.spriteBatch.Draw(tex, particle.Position - Main.screenLastPosition, null, color, 0, tex.Size() / 2f, 0.1f, SpriteEffects.None, 0);
                        EasyDraw.AnotherDraw(BlendState.AlphaBlend);
                    }
                    else
                    {
                        Texture2D tex = TextureLibrary.IceShard;
                        Rectangle rect = new(0, tex.Height / 3 * particle.Frame, tex.Width, tex.Height / 3);
                        Color color = Lighting.GetColor((int)(particle.Position.X / 16f), (int)(particle.Position.Y / 16f), Color.White);
                        Main.spriteBatch.Draw(tex, particle.Position - Main.screenPosition, rect, color, 0, rect.Size() / 2f, 0.4f, SpriteEffects.None, 0);
                    }
                }
                else
                {
                    if (particle.Timer < 30)
                    {
                        Texture2D tex3 = TextureLibrary.SnowFlake;
                        float alpha = 1;
                        float radius = 1;
                        if (particle.Timer < 20)
                        {
                            radius = MathHelper.Lerp(0, 1, particle.Timer / 20f);
                        }
                        else
                        {
                            alpha = MathHelper.Lerp(1, 0, (particle.Timer - 20) / 10f);
                        }
                        Color color = Lighting.GetColor((int)(particle.Position.X / 16f), (int)(particle.Position.Y / 16f), Color.SkyBlue * alpha);
                        DrawUtils.DrawGradientCircle(tex3, particle.Position, color * alpha, 0, new Vector2(1f, 0.3f), BlendState.Additive, radius, 0.5f);
                    }
                }
            }

        }

    }

    public class IceDripParticle(Vector2 pos, Vector2 vel, float scale, bool snow = false)
    {
        public Vector2 Position = pos;
        public Vector2 Velocity = vel;
        public float Scale = scale;
        public float Timer = 0;
        public int Frame = snow ? 3 : Main.rand.Next(3);
    }
}
