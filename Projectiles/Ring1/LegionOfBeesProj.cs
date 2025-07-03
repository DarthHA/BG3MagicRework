using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.VirtualEffects;
using BG3MagicRework.Static;
using BG3MagicRework.Static.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring1
{
    public class LegionOfBeesProj : BaseMagicProj
    {
        public List<TmpParticle> Particles = new();
        public List<Vector2> TrailPos = new();
        public int Target = -1;
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
            if (Projectile.ai[0] == 0)               //飞行阶段
            {
                if (Target != -1 && (Main.npc[Target].CanBeChasedBy() || Main.npc[Target].immortal))
                {
                    Vector2 MoveVel = Main.npc[Target].Center - Projectile.Center;
                    if (MoveVel.Length() > 50)
                    {
                        Projectile.velocity = Vector2.Normalize(MoveVel) * 50;
                    }
                    else
                    {
                        Projectile.Center = Main.npc[Target].Center;
                        Projectile.ai[0] = 1;
                        Projectile.ai[1] = 0;
                        Projectile.velocity = Vector2.Zero;
                        return;
                    }
                }
                else
                {
                    Projectile.ai[0] = 1;
                    Projectile.ai[1] = 0;
                    Projectile.velocity = Vector2.Zero;
                    return;
                }

                Projectile.ai[1]++;
                Vector2 UnitY = Vector2.Normalize(Projectile.velocity).RotatedBy(MathHelper.Pi / 2f);
                TrailPos.Add(Projectile.Center + UnitY * (Main.rand.NextFloat() * 2 - 1) * 5);
                if (TrailPos.Count > 15)
                {
                    TrailPos.RemoveAt(0);
                }
                if ((Collision.SolidTiles(Projectile.position, Projectile.width, Projectile.height) || Projectile.wet) && !CarefulSpellMM)
                {
                    Projectile.ai[0] = 1;
                    Projectile.ai[1] = 0;
                    Projectile.velocity = Vector2.Zero;
                    return;
                }
            }
            else if (Projectile.ai[0] == 1)           //爆炸阶段
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] == 1)
                {
                    Vector2 Center = Projectile.Center;
                    Projectile.width = 32;
                    Projectile.height = 32;
                    Projectile.Center = Center;
                    for (int i = 0; i < 20; i++)
                    {
                        Vector2 Vel = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(5, 10);
                        Vector2 Pos = Projectile.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(1, 10);
                        float scale = 0.25f + 0.25f * Main.rand.NextFloat();
                        Particles.NewParticle(Pos, Vel, scale);
                    }
                }

                TrailPos.Add(Projectile.Center);
                if (TrailPos.Count > 15)
                {
                    TrailPos.RemoveAt(0);
                }

                if (Projectile.ai[1] > 30) Projectile.Kill();
            }
            Particles.UpdateParticle(0.9f, 0.9f);
        }


        public override void SafeOnHit(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[0] = 1;
                Projectile.ai[1] = 0;
                Projectile.velocity = Vector2.Zero;
                //TrailPos.Clear();
                int protmp = Projectile.NewProjectile(Main.player[Projectile.owner].GetSource_FromThis("BG3Magic"), target.Center, Vector2.Zero, ModContent.ProjectileType<RotateBees>(), 0, 0, Projectile.owner);
                if (protmp >= 0 && protmp < 1000)
                {
                    (Main.projectile[protmp].ModProjectile as RotateBees).TargetNPC = target.whoAmI;
                    (Main.projectile[protmp].ModProjectile as RotateBees).CarefulSpellMM = CarefulSpellMM;
                }

                int protmp1 = Main.player[Projectile.owner].NewMagicProj(target.Center, Vector2.Zero, ModContent.ProjectileType<LegionOfBeesDamageProj>(), diceDamage, 0, CurrentRing);
                if (protmp1 >= 0 && protmp1 < 1000)
                {
                    (Main.projectile[protmp1].ModProjectile as LegionOfBeesDamageProj).TargetNPC = target.whoAmI;
                    (Main.projectile[protmp1].ModProjectile as BaseMagicProj).CopyMetaMagicFrom(this);
                }
            }
        }

        public override bool? SafeCanHitNPC(NPC target)
        {
            if (Projectile.ai[0] == 1 && Projectile.ai[1] >= 3) return false;
            return null;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texRibbon = TextureLibrary.Ribbon;
            Texture2D LightTex = TextureLibrary.BloomFlare;

            if (Projectile.ai[0] == 0)
            {
                List<CustomVertexInfo> bars1 = GetBars(Projectile.Center, TrailPos, 9, Color.White);
                List<CustomVertexInfo> bars2 = GetBars(Projectile.Center, TrailPos, 6, Color.White);
                DrawUtils.DrawRoSLaser(texRibbon, bars1, Color.DarkOrange, 0.4f, 1f, -Projectile.ai[1] / 10f, BlendState.Additive);
                DrawUtils.DrawRoSLaser(texRibbon, bars2, Color.White, 0.4f, 1f, -Projectile.ai[1] / 10f, BlendState.Additive);
            }
            else if (Projectile.ai[0] == 1)
            {
                List<CustomVertexInfo> bars1 = GetBars(Projectile.Center, TrailPos, 9, Color.White);
                List<CustomVertexInfo> bars2 = GetBars(Projectile.Center, TrailPos, 6, Color.White);

                DrawUtils.DrawRoSLaser(texRibbon, bars1, Color.DarkOrange, 0.4f, 1f, -Projectile.ai[1] / 10f, BlendState.Additive);
                DrawUtils.DrawRoSLaser(texRibbon, bars2, Color.White, 0.4f, 1f, -Projectile.ai[1] / 10f, BlendState.Additive);

                if (Projectile.ai[1] <= 20)
                {
                    float scale = 3;
                    float light = MathHelper.Lerp(1, 0, Projectile.ai[1] / 20f);
                    EasyDraw.AnotherDraw(BlendState.Additive);
                    Main.spriteBatch.Draw(LightTex, Projectile.Center - Main.screenPosition, null, Color.DarkOrange * light, Projectile.rotation, LightTex.Size() / 2f, 0.06f * scale, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(LightTex, Projectile.Center - Main.screenPosition, null, Color.White * light, Projectile.rotation, LightTex.Size() / 2f, 0.04f * scale, SpriteEffects.None, 0);
                }

                EasyDraw.AnotherDraw(BlendState.Additive);
                Texture2D tex = TextureLibrary.Extra;
                Particles.DrawParticle(tex, Color.Orange, true, new Vector2(1, 1));
            }

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