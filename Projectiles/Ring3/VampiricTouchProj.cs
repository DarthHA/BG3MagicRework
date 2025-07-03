using BG3MagicRework.BaseType;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring3
{
    public class VampiricTouchProj : BaseMagicProj
    {
        public int TargetNPC = -1;
        public override int MaxHits => 1;
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 99999;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 9999;
        }

        public override void AI()
        {
            if (TargetNPC != -1)
            {
                if (Main.npc[TargetNPC].CanBeChasedBy())
                {
                    Projectile.Center = Main.npc[TargetNPC].Center;
                }
            }
            else
            {
                Projectile.Kill();
                return;
            }

            Projectile.ai[0]++;
            if (Projectile.ai[0] > 30) Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texLight = TextureLibrary.BloomFlare;
            if (Projectile.ai[0] <= 10)
            {
                float radius = 300;
                float scale1 = MathHelper.Lerp(1, 0, Projectile.ai[0] / 10f);
                Draw710(Projectile.Center, radius * scale1, -Projectile.ai[0] / 200f, Color.White, Projectile.ai[0] / 200f, DrawUtils.ReverseSubtract);
                Draw710(Projectile.Center, radius * scale1 * 0.75f, -Projectile.ai[0] / 200f, Color.LightGreen, Projectile.ai[0] / 200f, BlendState.Additive);
            }
            if (Projectile.ai[0] >= 10 && Projectile.ai[0] < 25)
            {
                float alpha2 = 1;
                float scale2 = MathHelper.Lerp(0, 2, (Projectile.ai[0] - 10) / 15f);
                if (Projectile.ai[0] > 15)
                {
                    alpha2 = MathHelper.Lerp(1, 0, (Projectile.ai[0] - 15) / 10f);
                }
                Draw710(Projectile.Center, 150 * scale2, -Projectile.ai[0] / 200f, Color.White * alpha2, Projectile.ai[0] / 200f, DrawUtils.ReverseSubtract);
                Draw710(Projectile.Center, 150 * 0.75f * scale2, -Projectile.ai[0] / 200f, Color.LightGreen * alpha2, Projectile.ai[0] / 200f, BlendState.Additive);
                EasyDraw.AnotherDraw(BlendState.Additive);
                Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.LightGreen * alpha2 * 1.2f, 0, texLight.Size() / 2f, 0.2f * scale2, SpriteEffects.None, 0);
            }
            if (Projectile.ai[0] > 10 && Projectile.ai[0] < 20)
            {
                Texture2D texHollowCircleSoftEdge = TextureLibrary.HollowCircleSoftEdge;
                float scale = MathHelper.Lerp(0, 1, (Projectile.ai[0] - 10) / 10f);
                float light = MathHelper.Lerp(2, 0, (Projectile.ai[0] - 10) / 10f);
                Main.spriteBatch.Draw(texHollowCircleSoftEdge, Projectile.Center - Main.screenPosition, null, Color.LightGreen * light, 0, texHollowCircleSoftEdge.Size() / 2f, scale * 0.75f, SpriteEffects.None, 0);
            }

            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }

        public override bool? SafeCanHitNPC(NPC target)
        {
            if (Projectile.ai[0] < 12) return false;
            if (target.whoAmI != TargetNPC) return false;
            return null;
        }

        public override void SafeOnHit(NPC target, NPC.HitInfo hit, int damageDone)
        {
            float HealingAmount = damageDone / (float)(Math.Pow(2, CurrentRing - 3)) / CombatStat.Ring3Damage;
            if (CurrentRing > 3)
            {
                int increase = CurrentRing - 3;
                for (int i = 0; i < increase; i++) HealingAmount += Main.rand.Next(6) + 1;
            }
            Player owner = Main.player[Projectile.owner];
            int protmp = owner.NewMagicProj(Projectile.Center, Vector2.Zero, ModContent.ProjectileType<VampiricTouchHealingProj>(), CurrentRing);
            if (protmp >= 0 && protmp < 1000)
            {
                (Main.projectile[protmp].ModProjectile as VampiricTouchHealingProj).HealingAmount = (int)HealingAmount;
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).CopyMetaMagicFrom(this);
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public void Draw710(Vector2 Center, float radius, float progress, Color color, float rot, BlendState blendState)
        {
            List<CustomVertexInfo> bars = new();
            for (int i = 0; i <= 240; i++)
            {
                float r = i * MathHelper.TwoPi / 240f + rot;
                Vector2 Pos1 = r.ToRotationVector2() * radius;
                Vector2 Pos2 = r.ToRotationVector2() * 1;
                bars.Add(new CustomVertexInfo(Center + Pos1 - Main.screenPosition, Color.White, new Vector3(0, i / 240f, 1f)));
                bars.Add(new CustomVertexInfo(Center + Pos2 - Main.screenPosition, Color.White, new Vector3(1, i / 240f, 1f)));
            }
            DrawUtils.DrawRoSLaser(TextureLibrary.LightField, bars, color, 0.4f, 1f, progress, blendState);
        }
    }
}
