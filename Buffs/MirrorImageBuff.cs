using BG3MagicRework.Projectiles.VirtualEffects;
using BG3MagicRework.Static;
using BG3MagicRework.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Buffs
{
    public class MirrorImageBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.GetProj(ModContent.ProjectileType<MirrorShadow>()) == -1)
            {
                int protmp = Projectile.NewProjectile(player.GetSource_FromThis("BG3Magic"), player.Center, Vector2.Zero, ModContent.ProjectileType<MirrorShadow>(), 0, 0);     //注意，这个不是BaseMagicProj！
                if (protmp >= 0 && protmp < 1000)
                {
                    (Main.projectile[protmp].ModProjectile as MirrorShadow).MaxCount = player.GetModPlayer<DNDMagicPlayer>().MirrorImageCount;
                    (Main.projectile[protmp].ModProjectile as MirrorShadow).CurrentCount = player.GetModPlayer<DNDMagicPlayer>().MirrorImageCount;
                }
            }
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            rare = ItemRarityID.Cyan;
            /*
            if (Main.LocalPlayer.TryGetModPlayer(out DNDMagicPlayer result))
            {
                tip = string.Format(tip, 20 * result.MageArmorLevel);
            }
            */
        }
    }
}