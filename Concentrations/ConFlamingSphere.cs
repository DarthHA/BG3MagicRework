using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Ring2;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Concentrations
{
    public class ConFlamingSphere : BaseConcentration
    {
        public override string Name => "FlamingSphere";
        public override bool UpdateAndDecide(Player player)
        {
            if (projIndex == -1) return false;
            if (!Main.projectile[projIndex].active || Main.projectile[projIndex].type != ModContent.ProjectileType<FlamingSphereProj>()) return false;
            return true;
        }
    }
}
