using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Channel;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace BG3MagicRework.Items.Vanilla
{
    public class ThunderStaff : BaseWeaponModifier
    {
        public override int Type => ItemID.ThunderStaff;
        public override string SpellName => "WitchBolt";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalStaffChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(1), Color.Cyan, 0.6f);
            return false;
        }
    }

    public class Vilethorn : BaseWeaponModifier
    {
        public override int Type => ItemID.Vilethorn;
        public override string SpellName => "SpikeGrowth";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalStaffChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(2), Color.DarkOrange, 0.8f);
            return false;
        }
    }

    public class CrimsonRod : BaseWeaponModifier
    {
        public override int Type => ItemID.CrimsonRod;
        public override string SpellName => "PhantasmalForce";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalStaffChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(2), Color.Purple, 0.8f);
            return false;
        }
    }

    public class AquaScepter : BaseWeaponModifier
    {
        public override int Type => ItemID.AquaScepter;
        public override string SpellName => "Silence";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int protmp = NormalStaffChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(2), Color.Red, 0.8f);
            if (protmp >= 0 && protmp < 1000)
            {
                (Main.projectile[protmp].ModProjectile as BaseStaffChannel).IsStaff = false;
            }
            return false;
        }
    }

    public class MagicMissile : BaseWeaponModifier
    {
        public override int Type => ItemID.MagicMissile;
        public override string SpellName => "MagicMissile";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalStaffChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(1), Color.Red, 0.8f);
            return false;
        }
    }

    public class WeatherPain : BaseWeaponModifier
    {
        public override int Type => ItemID.WeatherPain;
        public override string SpellName => "Shatter";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalStaffChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(2), Color.White, 0.8f);
            return false;
        }
    }

    public class Flamelash : BaseWeaponModifier
    {
        public override int Type => ItemID.Flamelash;
        public override string SpellName => "ScorchingRay";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalStaffChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(2), Color.OrangeRed, 1.5f);
            return false;
        }
    }

    public class FlowerofFire : BaseWeaponModifier
    {
        public override int Type => ItemID.FlowerofFire;
        public override string SpellName => "FlamingSphere";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalStaffChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(2), Color.OrangeRed, 1.5f);
            return false;
        }
    }
}
