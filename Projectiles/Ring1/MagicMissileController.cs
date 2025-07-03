using BG3MagicRework.BaseType;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring1
{
    public class MagicMissileController : BaseMagicProj
    {
        public Vector2 RelaPos = Vector2.Zero;
        public int numOfShoots = 3;
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
        }


        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.IsDead())
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = owner.Center + RelaPos;
            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 5)
            {
                //发射一发魔法飞弹
                Vector2 Vel = Vector2.Normalize(Main.MouseWorld - Projectile.Center) * 20;
                int protmp = owner.NewMagicProj(Projectile.Center, Vel.RotatedBy((Main.rand.NextFloat() * 2 - 1) * MathHelper.Pi / 3f), ModContent.ProjectileType<MagicMissileProj>(), diceDamage, Projectile.knockBack, CurrentRing);
                if (protmp >= 0 && protmp < 1000)
                {
                    (Main.projectile[protmp].ModProjectile as BaseMagicProj).CopyMetaMagicFrom(this);
                }
                Projectile.ai[0] = 0;
                numOfShoots--;
            }
            if (numOfShoots <= 0)
            {
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {

            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

    }
}
