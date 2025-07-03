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
    public class FrogStaff : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 48;
            Item.rare = ItemRarityID.Green;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.LazyRemakeMagicItem();
        }
    }

    public class EnhanceLeapItem : BaseWeaponModifier
    {
        public override int Type => ModContent.ItemType<FrogStaff>();
        public override string SpellName => "EnhanceLeap";
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalStaffChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(1), Color.White, 1f);
            return false;
        }
    }
}
