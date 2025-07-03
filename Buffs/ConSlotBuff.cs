using BG3MagicRework.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BG3MagicRework.Buffs
{
    public abstract class ConSlotBuff : ModBuff
    {
        public override string Texture => "BG3MagicRework/Buffs/BuffPlaceHolder";
        public virtual int Index => 0;

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override bool RightClick(int buffIndex)
        {
            DNDMagicPlayer modplayer = Main.LocalPlayer.GetModPlayer<DNDMagicPlayer>();
            if (modplayer.ConcentrationSlot.Count > Index)
            {
                modplayer.ConcentrationSlot[Index].TimeLeft = 0;
            }
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
        {
            DNDMagicPlayer modplayer = Main.LocalPlayer.GetModPlayer<DNDMagicPlayer>();
            if (modplayer.ConcentrationSlot.Count > Index)
            {
                drawParams.Texture = modplayer.ConcentrationSlot[Index].GetIcon();
            }
            drawParams.DrawColor = Color.White;
            return true;
        }
        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            DNDMagicPlayer modplayer = Main.LocalPlayer.GetModPlayer<DNDMagicPlayer>();
            if (modplayer.ConcentrationSlot.Count > Index)
            {
                buffName = modplayer.ConcentrationSlot[Index].GetName(true);
                tip = modplayer.ConcentrationSlot[Index].GetDesc() + "\n" + Language.GetTextValue("Mods.BG3MagicRework.RightClickToEndCon");
            }
        }

        public override bool ReApply(Player player, int time, int buffIndex)
        {
            player.buffTime[buffIndex] = time;
            return true;
        }
    }

    public class ConSlotBuff0 : ConSlotBuff
    {
        public override int Index => 0;
    }
    public class ConSlotBuff1 : ConSlotBuff
    {
        public override int Index => 1;
    }
    public class ConSlotBuff2 : ConSlotBuff
    {
        public override int Index => 2;
    }
    public class ConSlotBuff3 : ConSlotBuff
    {
        public override int Index => 3;
    }
    public class ConSlotBuff4 : ConSlotBuff
    {
        public override int Index => 4;
    }
    public class ConSlotBuff5 : ConSlotBuff
    {
        public override int Index => 5;
    }
    public class ConSlotBuff6 : ConSlotBuff
    {
        public override int Index => 6;
    }
    public class ConSlotBuff7 : ConSlotBuff
    {
        public override int Index => 7;
    }
    public class ConSlotBuff8 : ConSlotBuff
    {
        public override int Index => 8;
    }
    public class ConSlotBuff9 : ConSlotBuff
    {
        public override int Index => 9;
    }

}