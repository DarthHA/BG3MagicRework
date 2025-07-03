using BG3MagicRework.BaseType;
using BG3MagicRework.Spells.Ring3;
using BG3MagicRework.Static;
using BG3MagicRework.Static.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring3
{
    public class FireBallProj : BaseMagicProj
    {
        public List<TmpParticle> tmpParticles = new();
        public List<TmpParticle> TrailParticles = new();
        public List<Vector2> TrailPos = new();
        public List<FlameParticle> flameParticles = new();
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            //Projectile.ignoreWater = true;
            Projectile.noEnchantmentVisuals = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 9999;
        }
        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[1]++;

                Vector2 trailPos = Projectile.Center - Vector2.Normalize(Projectile.velocity) * 10 + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(10);
                Vector2 trailVel = Vector2.Normalize(Projectile.velocity);
                TrailParticles.NewParticle(trailPos, trailVel, 0.3f + 0.3f * Main.rand.NextFloat());

                if (((Collision.SolidTiles(Projectile.position, Projectile.width, Projectile.height) || (Projectile.wet && !Projectile.lavaWet)) && !CarefulSpellMM)
                     || TravelDistance > GetSpellRange<FireballSpell>() * 16f)
                {
                    Projectile.ai[0] = 1;
                    Projectile.ai[1] = 0;
                    Projectile.velocity = Vector2.Zero;
                }
            }
            else if (Projectile.ai[0] == 1)
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] == 1)
                {
                    Vector2 Center = Projectile.Center;
                    Projectile.width = 16 * GetAOERadius<FireballSpell>() * 2;
                    Projectile.height = 16 * GetAOERadius<FireballSpell>() * 2;
                    Projectile.Center = Center;

                    for (int i = 0; i < 100; i++)
                    {
                        Vector2 ShootVel = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(1, 12);
                        float scale = 0.45f + Main.rand.NextFloat() * 0.45f;
                        flameParticles.NewParticle(Projectile.Center, ShootVel, scale);
                    }
                    for (int i = 0; i < 80; i++)
                    {
                        float rot = Main.rand.NextFloat() * MathHelper.TwoPi;
                        Vector2 Pos = Projectile.Center + rot.ToRotationVector2() * Main.rand.Next(5, 40);
                        Vector2 Vel = rot.ToRotationVector2() * Main.rand.Next(10, 25);
                        float scale = 0.5f + Main.rand.NextFloat() * 0.5f;
                        tmpParticles.NewParticle(Pos, Vel, scale);
                    }
                }

                if (Projectile.ai[1] > 30) Projectile.Kill();
            }
            if (Projectile.velocity == Vector2.Zero)
            {
                TrailPos.Add(Projectile.Center);
            }
            else
            {
                Vector2 UnitY = Vector2.Normalize(Projectile.velocity).RotatedBy(MathHelper.Pi / 2f);
                TrailPos.Add(Projectile.Center + UnitY * (Main.rand.NextFloat() * 2 - 1) * 5);
            }
            if (TrailPos.Count > 15)
            {
                TrailPos.RemoveAt(0);
            }
            tmpParticles.UpdateParticle(0.95f, 0.95f);
            for (int i = 0; i < 4; i++) flameParticles.UpdateParticle(0.93f, false);
            TrailParticles.UpdateParticle(1, 0.92f);
        }


        public override void SafeOnHit(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[0] = 1;
                Projectile.ai[1] = 0;
                Projectile.velocity = Vector2.Zero;
            }
        }

        public override bool? SafeCanHitNPC(NPC target)
        {
            if (Projectile.ai[0] == 1 && Projectile.ai[1] >= 3) return false;
            return null;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!CarefulSpellMM && !Collision.CanHit(Projectile.Center, 1, 1, targetHitbox.TopLeft(), targetHitbox.Width, targetHitbox.Height)) return false;
            return null;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texBlobGlow = TextureLibrary.BlobGlow;
            Texture2D texRibbon = TextureLibrary.Ribbon;
            Texture2D texExtra = TextureLibrary.Extra;
            Texture2D texHollowCircleSoftEdge = TextureLibrary.HollowCircleSoftEdge;

            EasyDraw.AnotherDraw(BlendState.Additive);
            TrailParticles.DrawParticle(texExtra, Color.Orange, true, new Vector2(3, 1));
            TrailParticles.DrawParticle(texExtra, Color.White, true, new Vector2(2, 0.5f));

            List<CustomVertexInfo> barsTrail1 = GetBars(Projectile.Center, TrailPos, 25, Color.White);
            List<CustomVertexInfo> barsTrail2 = GetBars(Projectile.Center, TrailPos, 17, Color.White);
            DrawUtils.DrawRoSLaser(texRibbon, barsTrail1, Color.DarkOrange, 0.4f, 1f, -Projectile.ai[1] / 10f, BlendState.Additive);
            DrawUtils.DrawRoSLaser(texRibbon, barsTrail2, Color.White, 0.4f, 1f, -Projectile.ai[1] / 10f, BlendState.Additive);


            if (Projectile.ai[0] == 0)
            {
                float rot = Projectile.velocity.ToRotation();
                EasyDraw.AnotherDraw(BlendState.Additive);
                for (int i = 0; i <= 10; i++)
                {
                    float scale0 = MathHelper.Lerp(1, 0.4f, i / 10f);
                    Color colorLerp = Color.Lerp(Color.DarkOrange, Color.White, i / 10f) * 0.4f;
                    Main.spriteBatch.Draw(texBlobGlow, Projectile.Center - Main.screenPosition, null, colorLerp, rot, new Vector2(texBlobGlow.Width / 3 * 2, texBlobGlow.Height / 2), 0.4f * scale0, SpriteEffects.FlipHorizontally, 0);
                }
            }
            else if (Projectile.ai[0] == 1)
            {
                if (Projectile.ai[1] <= 20)
                {
                    float scale = MathHelper.Lerp(0, 1, Projectile.ai[1] / 20f);
                    float light = MathHelper.Lerp(1, 0, MathHelper.Clamp((Projectile.ai[1] - 5) / 15f, 0, 1));
                    EasyDraw.AnotherDraw(BlendState.Additive);
                    Main.spriteBatch.Draw(texHollowCircleSoftEdge, Projectile.Center - Main.screenPosition, null, Color.Orange * light, 0, texHollowCircleSoftEdge.Size() / 2f, scale * 1.5f, SpriteEffects.None, 0);
                }
            }
            EasyDraw.AnotherDraw(BlendState.Additive);
            flameParticles.DrawParticle();
            tmpParticles.DrawParticle(texExtra, Color.DarkOrange, true, new Vector2(2, 1));
            tmpParticles.DrawParticle(texExtra, Color.White, true, new Vector2(1.5f, 0.5f));

            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }



        public List<CustomVertexInfo> GetBars(Vector2 Pos, List<Vector2> OldPos, int width, Color drawColor)
        {
            List<CustomVertexInfo> bars = new();
            List<Vector2> NonZeroPos = new();
            Vector2? LastValue = null;
            foreach (Vector2 oldpos in OldPos)
            {
                if (LastValue.HasValue && oldpos == LastValue)       //去除相同的点
                {
                    continue;
                }
                NonZeroPos.Add(oldpos);
                LastValue = oldpos;
            }
            if (NonZeroPos.Count > 0 && NonZeroPos[NonZeroPos.Count - 1] == Pos) NonZeroPos.RemoveAt(NonZeroPos.Count - 1);
            if (NonZeroPos.Count > 0)
            {
                for (int i = 0; i < NonZeroPos.Count; i++)
                {
                    Vector2 UnitY;
                    if (i < NonZeroPos.Count - 1)
                    {
                        UnitY = Vector2.Normalize(NonZeroPos[i + 1] - NonZeroPos[i]).RotatedBy(MathHelper.Pi / 2f);
                    }
                    else
                    {
                        UnitY = Vector2.Normalize(Pos - NonZeroPos[i]).RotatedBy(MathHelper.Pi / 2f);
                    }
                    bars.Add(new CustomVertexInfo(NonZeroPos[i] - UnitY * width - Main.screenPosition, drawColor, new Vector3(1f - (float)i / (NonZeroPos.Count + 5), 0f, 1)));
                    bars.Add(new CustomVertexInfo(NonZeroPos[i] + UnitY * width - Main.screenPosition, drawColor, new Vector3(1f - (float)i / (NonZeroPos.Count + 5), 1f, 1)));
                }
                if (Projectile.velocity.Length() > 0)
                {
                    Vector2 UnitX = Vector2.Normalize(Projectile.velocity);
                    Vector2 _UnitY = UnitX.RotatedBy(MathHelper.Pi / 2f);
                    for (int i = 0; i <= 5; i++)
                    {
                        bars.Add(new CustomVertexInfo(Pos + UnitX * 10 * i - _UnitY * width - Main.screenPosition, drawColor, new Vector3(1f - (float)(NonZeroPos.Count + i) / (NonZeroPos.Count + 5), 0f, 1)));
                        bars.Add(new CustomVertexInfo(Pos + UnitX * 10 * i + _UnitY * width - Main.screenPosition, drawColor, new Vector3(1f - (float)(NonZeroPos.Count + i) / (NonZeroPos.Count + 5), 1f, 1)));
                    }
                }
                else
                {
                    bars.Add(new CustomVertexInfo(Pos - Main.screenPosition, drawColor, new Vector3(0f, 0f, 1)));
                    bars.Add(new CustomVertexInfo(Pos - Main.screenPosition, drawColor, new Vector3(0f, 1f, 1)));
                }
            }
            return bars;
        }
    }

}