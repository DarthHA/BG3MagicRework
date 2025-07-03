using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Channel;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace BG3MagicRework.Items.Vanilla
{
    public class SPACEGUN : BaseWeaponModifier
    {
        public override int Type => ItemID.SpaceGun;
        public override string SpellName => "LaserAttack";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem(true);
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalRepeaterChannel.Launch(player, item.type, SpellName, 0, Color.Green, 1f, new Vector2(-5, -5), 20, 5, -2);
            return false;
        }
    }

    public class ZapinatorGray : BaseWeaponModifier
    {
        public override int Type => ItemID.ZapinatorGray;
        public override string SpellName => "ZapinatorAttack";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem(true);
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalRepeaterChannel.Launch(player, item.type, SpellName, 0, Color.White, 1f, new Vector2(-5, -5), 20, 5, -4);
            return false;
        }
    }

    public class BeeGun : BaseWeaponModifier
    {
        public override int Type => ItemID.BeeGun;
        public override string SpellName => "LegionOfBees";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeReactionItem();
        }
    }
}
