using BG3MagicRework.Static;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BG3MagicRework.Systems
{
    public class ModifyVanillaBuffs : GlobalBuff
    {
        public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
        {
            if (type == BuffID.MagicPower)
            {
                tip = string.Format(GetLocalization("IncreaseSorceryPointByX"), CombatStat.MagicPowerValue);
            }
            if (type == BuffID.ManaRegeneration)
            {
                tip = string.Format(GetLocalization("IncreaseSpellSlotStartRecoverRateByX"), CombatStat.ManaRegenPotionValue * 100);
            }
            if (type == BuffID.StarInBottle)
            {
                tip = string.Format(GetLocalization("IncreaseSpellSlotRecoverRateByX"), CombatStat.StarInBottleValue * 100);
            }
        }

        public string GetLocalization(string key) => Language.GetTextValue("Mods.BG3MagicRework.TooltipModify." + key);
    }
}