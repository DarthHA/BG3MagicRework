using BG3MagicRework.Projectiles.VirtualEffects;
using BG3MagicRework.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Buffs
{
    public class MageArmorBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override bool RightClick(int buffIndex)
        {
            AdvancedCombatText.NewText(Main.LocalPlayer.getRect(), Color.White, Lang.GetBuffName(ModContent.BuffType<MageArmorBuff>()), true);
            Main.LocalPlayer.GetModPlayer<DNDMagicPlayer>().MageArmorLevel = 0;
            return true;
        }
        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            rare = ItemRarityID.Yellow;
            if (Main.LocalPlayer.TryGetModPlayer(out DNDMagicPlayer result))
            {
                tip = string.Format(tip, 20 * result.MageArmorLevel);
            }
        }
    }
}