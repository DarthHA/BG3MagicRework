using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Systems
{
    public class DamageModifyProj : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public float DamageMult = 1f;

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= DamageMult;
        }

        public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.FinalDamage *= DamageMult;
        }
    }
}
