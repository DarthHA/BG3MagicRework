using BG3MagicRework.BaseType;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring1
{
    public class LegionOfBeesDamageProj : BaseMagicProj
    {
        public int TargetNPC = -1;
        public override int MaxHits => -1;

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.noEnchantmentVisuals = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 9999;
        }

        public override void AI()
        {
            Projectile.ai[1]++;

            if (TargetNPC != -1 && (Main.npc[TargetNPC].CanBeChasedBy() || Main.npc[TargetNPC].immortal))
            {
                NPC target = Main.npc[TargetNPC];
                Projectile.width = target.width + 200;
                Projectile.height = target.height + 100;
                Projectile.Center = target.Center;
            }

            if (Projectile.ai[1] < 150)
            {
                if (Projectile.wet && !CarefulSpellMM)
                {
                    Projectile.ai[1] = 150;
                }
            }

            if (Projectile.ai[1] > 180) Projectile.Kill();
        }

        public override bool? CanDamage()
        {
            if (Projectile.ai[1] < 150) return null;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
