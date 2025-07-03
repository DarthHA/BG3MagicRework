using BG3MagicRework.Projectiles.VirtualEffects;
using BG3MagicRework.Spells.Ring1;
using BG3MagicRework.Static;
using BG3MagicRework.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Buffs
{
    public class ArmorOfAgathysBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            rare = ItemRarityID.Cyan;
        }

        public override bool RightClick(int buffIndex)
        {
            if (Main.LocalPlayer.TryGetModPlayer(out DNDMagicPlayer modplayer))
            {
                if (modplayer.ExtraLife > 0)
                {
                    AdvancedCombatText.NewText(Main.LocalPlayer.getRect(), Color.White, EverythingLibrary.GetSpell<ArmorOfAgathysSpell>().GetName(), true);
                    int oldShieldValue = modplayer.ExtraLife;
                    Main.LocalPlayer.statLife -= oldShieldValue;
                    if (Main.LocalPlayer.statLife < 1) Main.LocalPlayer.statLife = 1;
                    modplayer.ExtraLife = 0;
                    modplayer.AoALevel = 0;
                }
            }
            return true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.GetProj(ModContent.ProjectileType<IceShieldEffect>()) == -1)
            {
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<IceShieldEffect>(), 0, 0, player.whoAmI);
            }
        }
    }
}