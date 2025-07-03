using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Channel;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace BG3MagicRework.Items.Vanilla
{
    public class MagicDagger : BaseWeaponModifier
    {
        public override int Type => ItemID.MagicDagger;
        public override string SpellName => "CloudOfDaggers";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalDaggerChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(2), Color.LightSkyBlue, 1.5f);
            return false;
        }
    }

    public class SpiritFlame : BaseWeaponModifier
    {
        public override int Type => ItemID.SpiritFlame;
        public override string SpellName => "FaerieFire";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalKeyChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(1), Color.Purple, 2f);
            return false;
        }
    }

    public class ShadowFlameHexDoll : BaseWeaponModifier
    {
        public override int Type => ItemID.ShadowFlameHexDoll;
        public override string SpellName => "Fear";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalKeyChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(3), Color.Purple, 3f, 40);
            return false;
        }
    }

    public class SharpTears : BaseWeaponModifier
    {
        public override int Type => ItemID.SharpTears;
        public override string SpellName => "PlantGrowth";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalKeyChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(3), Color.Red, 2f);
            return false;
        }
    }
}
