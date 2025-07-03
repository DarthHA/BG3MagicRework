using BG3MagicRework.BaseType;
using Terraria;

namespace BG3MagicRework.Concentrations
{
    public class ConCallLightning : BaseConcentration
    {
        public int UseTime = 2;
        public override string Name => "CallLightning";
        public override bool UpdateAndDecide(Player player)
        {
            if (UseTime <= 0) return false;
            return true;
        }
        public override void ModifyDesc(ref string desc)
        {
            desc = string.Format(desc, UseTime);
        }
    }
}
