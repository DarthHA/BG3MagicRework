using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Ring2;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Concentrations
{
    public class ConPhantasmalForce : BaseConcentration
    {
        public override string Name => "PhantasmalForce";
        public override bool UpdateAndDecide(Player player)
        {
            if (projIndex == -1) return false;
            if (!Main.projectile[projIndex].active || Main.projectile[projIndex].type != ModContent.ProjectileType<PhantasmalForceDamageProj>()) return false;
            return true;
        }
    }
}
