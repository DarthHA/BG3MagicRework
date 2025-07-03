using BG3MagicRework.BaseType;
using BG3MagicRework.Buffs.Enemy;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring1
{
    public class FaeireFireLight : BaseMagicProj
    {
        public int TargetNPC = -1;
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
            if (TargetNPC == -1)
            {
                Projectile.Kill();
                return;
            }
            NPC target = Main.npc[TargetNPC];
            if (!target.CanBeChasedBy(null, true) && !target.immortal)
            {
                Projectile.Kill();
                return;
            }
            if (owner.GetConcentration(ConUUID) == -1)
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = target.Center;
            target.DeepAddCCBuff(ModContent.BuffType<FaerieFireBuff>(), 2);
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
