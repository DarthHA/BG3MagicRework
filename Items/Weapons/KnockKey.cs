using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Channel;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Items.Weapons
{
    public class KnockKey : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 28;
            Item.rare = ItemRarityID.LightPurple;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.LazyRemakeMagicItem();
        }
    }

    public class KnockItem : BaseWeaponModifier
    {
        public override int Type => ModContent.ItemType<KnockKey>();
        public override string SpellName => "Knock";
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int protmp = NormalKeyChannel.Launch(player, item.type, SpellName, 2, Color.LightCyan, 1f, 50);
            return false;
        }
    }
}
