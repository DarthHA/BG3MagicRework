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
    public class OilBook : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.rare = ItemRarityID.White;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.LazyRemakeMagicItem();
        }
    }

    public class GreaseItem : BaseWeaponModifier
    {
        public override int Type => ModContent.ItemType<OilBook>();
        public override string SpellName => "Grease";
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalBookChannel.Launch(player, item.type, SpellName, 1, Color.Orange, 1f);
            return false;
        }
    }
}
