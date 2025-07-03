using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Channel;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace BG3MagicRework.Items.Vanilla
{
    public class WandofSparking : BaseWeaponModifier
    {
        public override int Type => ItemID.WandofSparking;
        public override string SpellName => "FireBolt";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem(true);
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalStaffChannel.Launch(player, item.type, SpellName, 0, Color.Orange, 0.6f);
            return false;
        }
    }

    public class WandofFrosting : BaseWeaponModifier
    {
        public override int Type => ItemID.WandofFrosting;
        public override string SpellName => "RayOfFrost";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem(true);
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalStaffChannel.Launch(player, item.type, SpellName, 0, Color.Cyan, 0.6f);
            return false;
        }
    }

}
