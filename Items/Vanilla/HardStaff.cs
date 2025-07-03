using BG3MagicRework.BaseType;
using BG3MagicRework.Concentrations;
using BG3MagicRework.Projectiles.Channel;
using BG3MagicRework.Projectiles.Ring3;
using BG3MagicRework.Spells.Ring3;
using BG3MagicRework.Static;
using BG3MagicRework.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Items.Vanilla
{
    public class SkyFracture : BaseWeaponModifier
    {
        public override int Type => ItemID.SkyFracture;
        public override string SpellName => "SpiritGuardians";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalStaffChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(3), Color.Orange, 0.6f, 25);
            return false;
        }
    }

    public class CrystalSerpent : BaseWeaponModifier
    {
        public override int Type => ItemID.CrystalSerpent;
        public override string SpellName => "LightningBolt";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalStaffChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(3), Color.Blue, 0.6f, 45);
            return false;
        }
    }

    public class CrystalVileShard : BaseWeaponModifier
    {
        public override int Type => ItemID.CrystalVileShard;
        public override string SpellName => "GlyphOfWarding";
        public override bool AlterFunctionUse => true;
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int gowType = player.GetModPlayer<DNDMagicPlayer>().GoWType;
            Color lightColor = SomeUtils.GetColor(GlyphOfWardingSpell.GetDamageType(gowType));
            if (player.altFunctionUse != 2)
            {
                NormalStaffChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(3), lightColor, 0.6f);
            }
            else
            {
                GoWSwitchChannel.Launch(player, item.type, SpellName, lightColor, 0.6f);
            }
            return false;
        }
    }


    public class SoulDrain : BaseWeaponModifier
    {
        public override int Type => ItemID.SoulDrain;
        public override string SpellName => "VampiricTouch";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalStaffChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(3), Color.DarkGreen, 0.6f, 25);
            return false;
        }
    }

    public class ClingerStaff : BaseWeaponModifier
    {
        public override int Type => ItemID.ClingerStaff;
        public override string SpellName => "HungerOfHadar";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalStaffChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(3), Color.DarkCyan, 0.6f, 35);
            return false;
        }
    }

    public class PoisonStaff : BaseWeaponModifier
    {
        public override int Type => ItemID.PoisonStaff;
        public override string SpellName => "PoisonSpray";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem(true);
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalStaffChannel.Launch(player, item.type, SpellName, 0, Color.GreenYellow, 0.6f, 25);
            return false;
        }
    }

    public class FrostStaff : BaseWeaponModifier
    {
        public override int Type => ItemID.FrostStaff;
        public override string SpellName => "ArmorOfAgathys";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalStaffChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(1), Color.Cyan, 1.2f, 25);
            return false;
        }
    }

    public class FlowerofFrost : BaseWeaponModifier
    {
        public override int Type => ItemID.FlowerofFrost;
        public override string SpellName => "SnillocsSnowballSwarm";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalStaffChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(2), Color.Cyan, 1.2f, 25);
            return false;
        }
    }

    public class MeteorStaff : BaseWeaponModifier
    {
        public override int Type => ItemID.MeteorStaff;
        public override string SpellName => "MelfsMinuteMeteors";
        public override bool AlterFunctionUse => true;
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool CanUse(Item item, Player player, bool AlterFunctionUse = false)
        {
            if (AlterFunctionUse)
            {
                return player.GetProj(ModContent.ProjectileType<MelfsMinuteMeteorsController>()) != -1;
            }
            return true;
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                NormalStaffChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(3), Color.Brown, 1.2f, 25);
            }
            else
            {
                int protmp0 = player.GetProj(ModContent.ProjectileType<MelfsMinuteMeteorsController>());
                int ring = (Main.projectile[protmp0].ModProjectile as MelfsMinuteMeteorsController).CurrentRing;
                int protmp = NormalStaffChannel.Launch(player, item.type, "MelfsMinuteMeteorsRelease", ring, Color.Brown, 1.2f, 25);
                if (protmp >= 0 && protmp <= 1000)
                {
                    (Main.projectile[protmp].ModProjectile as BaseChannel).InstantAndFree = true;
                }
            }
            return false;
        }
    }

    public class IceRod : BaseWeaponModifier
    {
        public override int Type => ItemID.IceRod;
        public override string SpellName => "SleetStorm";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalStaffChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(3), Color.Cyan, 1.2f, 25);
            return false;
        }
    }

    public class NimbusRod : BaseWeaponModifier
    {
        public override int Type => ItemID.NimbusRod;
        public override string SpellName => "CallLightning";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool AlterFunctionUse => true;
        public override bool CanUse(Item item, Player player, bool AlterFunctionUse = false)
        {
            if (AlterFunctionUse)
            {
                return player.GetConcentration<ConCallLightning>() != -1;
            }
            return true;
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                NormalStaffChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(3), Color.Blue, 0.6f, 25);
            }
            else
            {
                ConCallLightning con = player.GetModPlayer<DNDMagicPlayer>().ConcentrationSlot[player.GetConcentration<ConCallLightning>()] as ConCallLightning;
                int ring = con.Ring;
                int protmp = NormalStaffChannel.Launch(player, item.type, "CallLightningRepeat", ring, Color.Blue, 0.6f, 25);
                if (protmp >= 0 && protmp <= 1000)
                {
                    (Main.projectile[protmp].ModProjectile as BaseChannel).InstantAndFree = true;
                }
            }
            return false;
        }
    }
}
