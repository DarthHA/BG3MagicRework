using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Ring3;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Concentrations
{
    public class ConFear : BaseConcentration
    {
        public override string Name => "Fear";
        public override bool UpdateAndDecide(Player player)
        {
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.type == ModContent.ProjectileType<FearEffectProj>() && proj.owner == player.whoAmI)
                {
                    if ((proj.ModProjectile as BaseMagicProj).ConUUID == UUID)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
