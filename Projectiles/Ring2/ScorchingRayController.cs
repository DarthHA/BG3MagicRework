using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.VirtualEffects;
using BG3MagicRework.Spells.Ring2;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring2
{
    public class ScorchingRayController : BaseMagicProj
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
                //发射一发灼热射线
                Vector2 SourcePos = Projectile.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(30);
                Vector2 TargetPos;
                if (CarefulSpellMM)
                {
                    TargetPos = SomeUtils.GetNoBlockEndPos(SourcePos, Main.MouseWorld, GetSpellRange<ScorchingRaySpell>() * 16);
                }
                else
                {
                    TargetPos = SomeUtils.GetTileBlockedEndPos(SourcePos, Main.MouseWorld, GetSpellRange<ScorchingRaySpell>() * 16);
                }
                Vector2 vel = Vector2.Normalize(TargetPos - SourcePos);
                int protmp = owner.NewMagicProj(SourcePos, vel, ModContent.ProjectileType<ScorchingRayProj>(), diceDamage, 0, CurrentRing);
                if (protmp >= 0 && protmp < 1000)
                {
                    (Main.projectile[protmp].ModProjectile as ScorchingRayProj).TargetPos = TargetPos;
                    (Main.projectile[protmp].ModProjectile as BaseMagicProj).CopyMetaMagicFrom(this);
                }
                MachRing.Summon(SourcePos + vel * 10, vel.ToRotation(), 30, 20, Color.OrangeRed);
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
