using BG3MagicRework.BaseType;
using BG3MagicRework.Buffs.Enemy;
using BG3MagicRework.Spells.Ring2;
using BG3MagicRework.Static;
using BG3MagicRework.Static.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring2
{
    public class MelfsAcidArrowProj : BaseMagicProj
    {
        public List<TmpParticle> tmpParticles = new();
        public List<Vector2> TrailPos = new();
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
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
                if (((Collision.SolidTiles(Projectile.position, Projectile.width, Projectile.height) || (Projectile.wet && !Projectile.lavaWet)) && !CarefulSpellMM)
                     || TravelDistance > GetSpellRange<MelfsAcidArrowSpell>() * 16f)
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
                    Projectile.width = 16 * GetAOERadius<MelfsAcidArrowSpell>() * 2;
                    Projectile.height = 16 * GetAOERadius<MelfsAcidArrowSpell>() * 2;
                    Projectile.Center = Center;
                    for (int i = 0; i < 30; i++)
                    {
                        float rot = Main.rand.NextFloat() * MathHelper.TwoPi;
                        Vector2 Pos = Projectile.Center + rot.ToRotationVector2() * Main.rand.Next(5, 20);
                        Vector2 Vel = rot.ToRotationVector2() * Main.rand.Next(5, 20);
                        float scale = 0.25f + Main.rand.NextFloat() * 0.25f;
                        tmpParticles.NewParticle(Pos, Vel, scale);
                    }
                }

                if (Projectile.ai[1] > 30) Projectile.Kill();
            }

            TrailPos.Add(Projectile.Center);
            if (TrailPos.Count > 7)
            {
                TrailPos.RemoveAt(0);
            }
            tmpParticles.UpdateParticle(0.93f, 0.93f);
        }


        public override void SafeOnHit(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[0] = 1;
                Projectile.ai[1] = 0;
                Projectile.velocity = Vector2.Zero;
            }
            int protmp = Main.player[Projectile.owner].NewMagicProj(target.Center, Vector2.Zero, ModContent.ProjectileType<MelfsAcidArrowDamageProj>(), extraDiceDamage, 0, CurrentRing);
            if (protmp >= 0 && protmp < 1000)
            {
                (Main.projectile[protmp].ModProjectile as MelfsAcidArrowDamageProj).CopyMetaMagicFrom(this);
                (Main.projectile[protmp].ModProjectile as MelfsAcidArrowDamageProj).TargetNPC = target.whoAmI;
            }
            target.AddNormalBuff(ModContent.BuffType<AcidArrowBuff>(), GetTimeSpan<MelfsAcidArrowSpell>() * 60);
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

            List<CustomVertexInfo> barsTrail1 = GetBars(Projectile.Center, TrailPos, 12, Color.White);
            List<CustomVertexInfo> barsTrail2 = GetBars(Projectile.Center, TrailPos, 8, Color.White);
            DrawUtils.DrawRoSLaser(texRibbon, barsTrail1, Color.GreenYellow, 0.4f, 1f, -Projectile.ai[1] / 20f, BlendState.Additive);
            DrawUtils.DrawRoSLaser(texRibbon, barsTrail2, Color.White, 0.4f, 1f, -Projectile.ai[1] / 20f, BlendState.Additive);


            if (Projectile.ai[0] == 0)
            {
                float rot = Projectile.velocity.ToRotation();
                EasyDraw.AnotherDraw(BlendState.Additive);
                for (int i = 0; i <= 10; i++)
                {
                    float scale0 = MathHelper.Lerp(1, 0.4f, i / 10f);
                    Color colorLerp = Color.Lerp(Color.Green, Color.Yellow, i / 10f) * 0.4f;
                    Main.spriteBatch.Draw(texBlobGlow, Projectile.Center - Main.screenPosition, null, colorLerp, rot, new Vector2(texBlobGlow.Width / 3 * 2, texBlobGlow.Height / 2), 0.2f * scale0, SpriteEffects.FlipHorizontally, 0);
                }
            }
            else if (Projectile.ai[0] == 1)
            {
                Texture2D tex = TextureLibrary.Extra;
                tmpParticles.DrawParticle(tex, Color.Green, false, new Vector2(1, 1));
                float radius = GetAOERadius<MelfsAcidArrowSpell>() * 16f;
                float scale = MathHelper.Lerp(0.1f, 1.4f, MathHelper.Clamp(Projectile.ai[1] / 20, 0, 1));
                float alpha = MathHelper.Lerp(1, 0, MathHelper.Clamp((Projectile.ai[1] - 5) / 25f, 0, 1));
                Color color1 = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, Color.Green);
                Color color2 = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, Color.GreenYellow);
                Color color3 = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, Color.Yellow);
                Draw710(Projectile.Center, radius * scale, -Projectile.ai[1] / 200f, color1 * alpha, Projectile.ai[1] / 200f);
                Draw710(Projectile.Center, radius * scale, -Projectile.ai[1] / 200f, color2 * alpha, Projectile.ai[1] / 200f + 1);
                Draw710(Projectile.Center, radius * scale, -Projectile.ai[1] / 200f, color3 * alpha, Projectile.ai[1] / 200f + 2);
            }
            EasyDraw.AnotherDraw(BlendState.Additive);
            tmpParticles.DrawParticle(texExtra, Color.YellowGreen, true, new Vector2(2, 1));
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
                        bars.Add(new CustomVertexInfo(Pos + UnitX * 2 * i - _UnitY * width - Main.screenPosition, drawColor, new Vector3(1f - (float)(NonZeroPos.Count + i) / (NonZeroPos.Count + 5), 0f, 1)));
                        bars.Add(new CustomVertexInfo(Pos + UnitX * 2 * i + _UnitY * width - Main.screenPosition, drawColor, new Vector3(1f - (float)(NonZeroPos.Count + i) / (NonZeroPos.Count + 5), 1f, 1)));
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

        public void Draw710(Vector2 Center, float radius, float progress, Color color, float rot = 0)
        {
            List<CustomVertexInfo> bars = new();
            for (int i = 0; i <= 240; i++)
            {
                float r = i * MathHelper.TwoPi / 240f + rot;
                Vector2 Pos1 = r.ToRotationVector2() * 1;
                Vector2 Pos2 = r.ToRotationVector2() * radius;
                bars.Add(new CustomVertexInfo(Center + Pos1 - Main.screenPosition, Color.White, new Vector3(0, i / 240f, 1f)));
                bars.Add(new CustomVertexInfo(Center + Pos2 - Main.screenPosition, Color.White, new Vector3(1, i / 240f, 1f)));
            }
            DrawUtils.DrawRoSLaser(TextureLibrary.Perlin, bars, color, 0.2f, 0.6f, progress, BlendState.Additive);
        }
    }
}
