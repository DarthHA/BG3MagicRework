using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Channel;
using BG3MagicRework.Spells.Ring1;
using BG3MagicRework.Static;
using BG3MagicRework.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace BG3MagicRework.Items.Vanilla
{
    public class WaterBolt : BaseWeaponModifier
    {
        public override int Type => ItemID.WaterBolt;
        public override string SpellName => "CreateWater";
        public override bool AlterFunctionUse => true;
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem();
        }
        public override bool CanUse(Item item, Player player, bool AlterFunctionUse = false)
        {
            if (AlterFunctionUse)
            {
                return player.GetAvailableRings(EverythingLibrary.GetSpell<DestroyWaterSpell>().InitialRing).Count > 0;
            }
            return true;
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                NormalBookChannel.Launch(player, item.type, SpellName, player.GetSmallestAvailableRings(1), Color.Blue, 1f);
            }
            else
            {
                NormalBookChannel.Launch(player, item.type, "DestroyWater", player.GetSmallestAvailableRings(1), Color.Blue, 1f);
            }
            return false;
        }
    }

    public class BookofSkulls : BaseWeaponModifier
    {
        public override int Type => ItemID.BookofSkulls;
        public override string SpellName => "TollTheDead";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeMagicItem(true);
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NormalBookChannel.Launch(player, item.type, SpellName, 0, Color.Green, 1.2f);
            return false;
        }
    }

    public class DemonScythe : BaseWeaponModifier
    {
        public override int Type => ItemID.DemonScythe;
        public override string SpellName => "HellishRebuke";
        public override void SetDefaults(Item item)
        {
            item.LazyRemakeReactionItem();
        }
    }
}
