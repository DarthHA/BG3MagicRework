using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Channel;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace BG3MagicRework.Items.Vanilla
{
    public class CursedFlames : BaseWeaponModifier
    {
        public override int Type => ItemID.CursedFlames;
        public override string SpellName => "Fireball";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalBookChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(3), Color.Orange, 1.2f);
            return false;
        }
    }

    public class GoldenShower : BaseWeaponModifier
    {
        public override int Type => ItemID.GoldenShower;
        public override string SpellName => "MelfsAcidArrow";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalBookChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(2), Color.YellowGreen, 1.2f);
            return false;
        }
    }


    public class CrystalStorm : BaseWeaponModifier
    {
        public override int Type => ItemID.CrystalStorm;
        public override string SpellName => "MirrorImage";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalBookChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(2), Color.Gold, 1.2f);
            return false;
        }
    }
}
