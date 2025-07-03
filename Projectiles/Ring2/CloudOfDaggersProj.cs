using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.VirtualEffects;
using BG3MagicRework.Spells.Ring2;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring2
{
    public class CloudOfDaggersProj : BaseMagicProj
    {
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
            Projectile.localNPCHitCooldown = 60;
        }


        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];


            if (Projectile.ai[0] == 0)  //出现并维持
            {
                if (Projectile.ai[1] < 30) Projectile.ai[1]++;
                if (Projectile.ai[1] == 1)
                {
                    int protmp = Projectile.NewProjectile(owner.GetSource_FromThis("BG3Magic"), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<RotateDaggers>(), 0, 0, owner.whoAmI);
                    Main.projectile[protmp].ai[0] = Projectile.whoAmI;
                }

                if (owner.IsDead())
                {
                    Projectile.ai[0] = 1;
                    return;
                }
                if (owner.GetConcentration(ConUUID) == -1)
                {
                    Projectile.ai[0] = 1;
                    return;
                }
                if (owner.Distance(Projectile.Center) > GetSpellRange<CloudOfDaggersSpell>() * 16f * 2f)
                {
                    Projectile.ai[0] = 1;
                    Projectile.Kill();
                    return;
                }

            }
            else if (Projectile.ai[0] == 1) //消失
            {
                Projectile.ai[1]--;
                if (Projectile.ai[1] <= 0)
                {
                    Projectile.Kill();
                    return;
                }
            }

        }

        public override bool? SafeCanHitNPC(NPC target)
        {
            if (Projectile.ai[1] >= 30) return null;
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Rectangle rect = new((int)Projectile.Center.X - GetAOERadius<CloudOfDaggersSpell>() * 16, (int)Projectile.Center.Y - GetAOERadius<CloudOfDaggersSpell>() * 16, GetAOERadius<CloudOfDaggersSpell>() * 32, GetAOERadius<CloudOfDaggersSpell>() * 32);
            return targetHitbox.Intersects(rect) && (CarefulSpellMM || Collision.CanHit(targetHitbox.TopLeft(), targetHitbox.Width, targetHitbox.Height, rect.TopLeft(), rect.Width, rect.Height));
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
