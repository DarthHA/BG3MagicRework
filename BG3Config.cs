using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace BG3MagicRework
{
    public class BG3Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(false)]
        public bool ShowCombatInfo;

        [DefaultValue(false)]
        public bool ShowRangeIndicator;

        public override ModConfig Clone()
        {
            var clone = (BG3Config)base.Clone();
            return clone;
        }
    }
}