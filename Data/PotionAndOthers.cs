using BG3MagicRework.BaseType;
using BG3MagicRework.Static;
using Terraria.ID;

namespace BG3MagicRework.Data
{
    public class PotionAndOthers
    {
        public class MagicPowerPotion : BaseItemModifier
        {
            public override int Type => ItemID.MagicPowerPotion;
            public override string GetTooltip()
            {
                return string.Format(GetLocalization("IncreaseSorceryPointByX"), CombatStat.MagicPowerValue);
            }
        }

        public class ManaRegenerationPotion : BaseItemModifier
        {
            public override int Type => ItemID.ManaRegenerationPotion;
            public override string GetTooltip()
            {
                return string.Format(GetLocalization("IncreaseSpellSlotStartRecoverRateByX"), CombatStat.ManaRegenPotionValue * 100);
            }
        }

        public class ManaCrystal : BaseItemModifier
        {
            public override int Type => ItemID.ManaCrystal;
            public override string GetTooltip()
            {
                return GetLocalization("NewManaCrystalTooltip");
            }
        }


        public class ArcaneCrystal : BaseItemModifier
        {
            public override int Type => ItemID.ArcaneCrystal;
            public override string GetTooltip()
            {
                return GetLocalization("NewArcaneCrystalTooltip");
            }
        }
    }
}
