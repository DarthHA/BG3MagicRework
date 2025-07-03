using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Ring1;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Concentrations
{
    public class ConWitchBolt : BaseConcentration
    {
        public override string Name => "WitchBolt";
        public override bool UpdateAndDecide(Player player)
        {
            if (projIndex == -1) return false;
            if (!Main.projectile[projIndex].active || Main.projectile[projIndex].type != ModContent.ProjectileType<WitchBoltProj>()) return false;
            return true;
        }
    }
}
