using BG3MagicRework.Buffs;
using BG3MagicRework.Static;
using BG3MagicRework.Static.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.VirtualEffects
{
    public class IceShieldEffect : ModProjectile
    {
        public List<SmokeParticle> smokeParticles = new();

        public override string Texture => "BG3MagicRework/Images/PlaceHolder";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.timeLeft = 999999;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.Opacity = 1;
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.IsDead())
            {
                Projectile.Kill();
                return;
            }
            if (!owner.HasBuff(ModContent.BuffType<ArmorOfAgathysBuff>()))
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = owner.Center;

            if (Main.rand.NextBool(4))
            {
                Vector2 ShootVel = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat() * 4 + owner.velocity;
                smokeParticles.NewParticle(Projectile.Center, ShootVel, Main.rand.NextFloat() * 0.25f + 0.25f, Color.LightBlue * 0.3f);
            }
            smokeParticles.UpdateParticle(0.93f, 2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            EasyDraw.AnotherDraw(BlendState.Additive);
            smokeParticles.DrawParticle(false);
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }
    }
}
