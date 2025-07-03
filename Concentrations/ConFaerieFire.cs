using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Ring1;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Concentrations
{
    public class ConFaerieFire : BaseConcentration
    {
        public override string Name => "FaerieFire";
        public override bool UpdateAndDecide(Player player)
        {
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.type == ModContent.ProjectileType<FaeireFireLight>() && proj.owner == player.whoAmI)
                {
                    if ((proj.ModProjectile as FaeireFireLight).ConUUID == UUID)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
