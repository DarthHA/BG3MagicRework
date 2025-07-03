using BG3MagicRework.BaseType;
using Terraria;

namespace BG3MagicRework.Concentrations
{
    public class ConEnhanceLeap : BaseConcentration
    {
        public override string Name => "EnhanceLeap";
        public override bool UpdateAndDecide(Player player)
        {
            player.autoJump = true;
            player.jumpSpeedBoost += 2.5f;
            player.noFallDmg = true;
            return true;
        }
    }
}
