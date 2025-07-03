using BG3MagicRework.BaseType;
using BG3MagicRework.Static;
using BG3MagicRework.Systems;
using Terraria;
using Terraria.ID;

namespace BG3MagicRework.Data
{
    public class BandofStarpower : BaseItemModifier
    {
        public override int Type => ItemID.BandofStarpower;
        public override void UpdateEquip(Item item, Player player)
        {
            player.AddSpellSlot(1, 1);
        }
        public override string GetTooltip()
        {
            return string.Format(GetLocalization("GainXXRingSlot"), 1, 1);
        }
    }

    public class ManaRegenerationBand : BaseItemModifier
    {
        public override int Type => ItemID.ManaRegenerationBand;
        public override void UpdateEquip(Item item, Player player)
        {
            player.AddSpellSlot(1, 1);
            player.GetModPlayer<DNDMagicPlayer>().SpellSlotRecoveryStartRate += CombatStat.ManaRegenBandValue1;
            player.GetModPlayer<DNDMagicPlayer>().SpellSlotRecoveryRate += CombatStat.ManaRegenBandValue2;
        }
        public override string GetTooltip()
        {
            return string.Format(GetLocalization("GainXXRingSlot"), 1, 1) + "\n" +
            string.Format(GetLocalization("IncreaseSpellSlotStartRecoverRateByX"), CombatStat.ManaRegenBandValue1 * 100) + "\n" +
            string.Format(GetLocalization("IncreaseSpellSlotRecoverRateByX"), CombatStat.ManaRegenBandValue2 * 100);

        }
    }
}
