using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Ring3;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Concentrations
{
    public class ConPlantGrowth : BaseConcentration
    {
        public override string Name => "PlantGrowth";
        public override bool UpdateAndDecide(Player player)
        {
            if (projIndex == -1) return false;
            if (!Main.projectile[projIndex].active || Main.projectile[projIndex].type != ModContent.ProjectileType<PlantGrowthProj>()) return false;
            return true;
        }
    }
}
