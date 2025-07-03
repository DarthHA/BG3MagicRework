using BG3MagicRework.BaseType;
using Terraria.Localization;

namespace BG3MagicRework.Static
{
    public static class LangLibrary
    {
        public static string Root => "Mods.BG3MagicRework.";
        public static string Cantrips => Language.GetTextValue(Root + "Cantrips");
        public static string Tiles => Language.GetTextValue(Root + "Tiles");
        public static string NoSpace => Language.GetTextValue(Root + "NoSpace");
        public static string CannotSee => Language.GetTextValue(Root + "CannotSee");
        public static string OutOfRange => Language.GetTextValue(Root + "OutOfRange");
        public static string NoTarget => Language.GetTextValue(Root + "NoTarget");
        public static string XRingSpell => Language.GetTextValue(Root + "XRingSpell");
        public static string XRingSlot => Language.GetTextValue(Root + "XRingSlot");
        public static string CannotRelease => Language.GetTextValue(Root + "CannotRelease");
        public static string DebuffReason => Language.GetTextValue(Root + "DebuffReason");
        public static string HungerReason => Language.GetTextValue(Root + "HungerReason");
        public static string TriggerReaction => Language.GetTextValue(Root + "TriggerReaction");
        public static string ReactionOn => Language.GetTextValue(Root + "ReactionOn");
        public static string ReactionOff => Language.GetTextValue(Root + "ReactionOff");
        public static string MetaMagicOn => Language.GetTextValue(Root + "MetaMagicOn");
        public static string MetaMagicOff => Language.GetTextValue(Root + "MetaMagicOff");
        public static string XDealXDamageToX => Language.GetTextValue(Root + "XDealXDamageToX");
        public static string XUsedSpellX => Language.GetTextValue(Root + "XUsedSpellX");
        public static string XUsedReactionX => Language.GetTextValue(Root + "XUsedReactionX");
        public static string XGetXSecXBuff => Language.GetTextValue(Root + "XGetXSecXBuff");
        public static string XPassedXsSavingThrowDCX => Language.GetTextValue(Root + "XPassedXsSavingThrowDCX");
        public static string XNotPassedXsSavingThrowDCX => Language.GetTextValue(Root + "XNotPassedXsSavingThrowDCX");
        public static string XAddedXSecXBuff => Language.GetTextValue(Root + "XAddedXSecXBuff");
        public static string XBeginConX => Language.GetTextValue(Root + "XBeginConX");
        public static string XEndConX => Language.GetTextValue(Root + "XEndConX");

        public static string Infinite => Language.GetTextValue(Root + "Infinite");
        public static string GetLocalize(this DamageElement element)
        {
            return Language.GetTextValue(Root + "Elements." + element.ToString());
        }
        public static string GetLocalize(this SchoolOfMagic school)
        {
            return Language.GetTextValue(Root + "SchoolOfMagic." + school.ToString());
        }
    }
}
