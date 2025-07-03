using BG3MagicRework.BaseType;
using BG3MagicRework.Static;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Items.Weapons
{
    public class ShieldBook : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.rare = ItemRarityID.Yellow;
            Item.LazyRemakeReactionItem();
        }
    }

    public class ShieldSpellBook : BaseWeaponModifier
    {
        public override int Type => ModContent.ItemType<ShieldBook>();
        public override string SpellName => "Shield";
    }
}
