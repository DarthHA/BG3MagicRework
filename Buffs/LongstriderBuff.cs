using BG3MagicRework.Projectiles.VirtualEffects;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Buffs
{
    public class LongstriderBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.buffTime[buffIndex] == 1)
            {
                AdvancedCombatText.NewText(player.getRect(), Color.White, Lang.GetBuffName(Type), true);
            }
        }
    }
}