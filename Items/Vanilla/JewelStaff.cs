using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Channel;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace BG3MagicRework.Items.Vanilla
{
    public class AmethystStaff : BaseWeaponModifier
    {
        public override int Type => ItemID.AmethystStaff;
        public override string SpellName => "ThunderWave";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalStaffChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(1), Color.White, 2f);
            return false;
        }
    }

    public class RubyStaff : BaseWeaponModifier
    {
        public override int Type => ItemID.RubyStaff;
        public override string SpellName => "BurningHands";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalStaffChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(1), Color.Red, 1.5f);
            return false;
        }
    }

    public class EmeraldStaff : BaseWeaponModifier
    {
        public override int Type => ItemID.EmeraldStaff;
        public override string SpellName => "RayOfSickness";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalStaffChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(1), Color.Green, 1.5f);
            return false;
        }
    }

    public class AmberStaff : BaseWeaponModifier
    {
        public override int Type => ItemID.AmberStaff;
        public override string SpellName => "MageArmor";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalStaffChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(1), Color.LightPink, 1.5f);
            return false;
        }
    }

    public class SapphireStaff : BaseWeaponModifier
    {
        public override int Type => ItemID.SapphireStaff;
        public override string SpellName => "IceKnife";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalStaffChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(1), Color.Cyan, 1.5f);
            return false;
        }
    }

    public class TopazStaff : BaseWeaponModifier
    {
        public override int Type => ItemID.TopazStaff;
        public override string SpellName => "GuidingBolt";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalStaffChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(1), Color.LightYellow, 1.5f);
            return false;
        }
    }

    public class DiamondStaff : BaseWeaponModifier
    {
        public override int Type => ItemID.DiamondStaff;
        public override string SpellName => "ColorSpray";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalStaffChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(1), Color.White, 1.5f);
            return false;
        }
    }
}