using BG3MagicRework.Static;
using BG3MagicRework.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Buffs
{
    public class HeightenedSpellMMBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
        {
            DNDMagicPlayer modplayer = Main.LocalPlayer.GetModPlayer<DNDMagicPlayer>();
            Texture2D bg = ModContent.Request<Texture2D>("BG3MagicRework/Buffs/MetaMagic").Value;
            Color alpha;
            if (modplayer.HeightenedSpell)
            {
                alpha = Color.White;
            }
            else
            {
                alpha = drawParams.DrawColor;
            }
            spriteBatch.Draw(bg, drawParams.Position, drawParams.SourceRectangle, alpha);
            EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
            spriteBatch.Draw(drawParams.Texture, drawParams.Position, drawParams.SourceRectangle, alpha);
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }

        public override bool RightClick(int buffIndex)
        {
            DNDMagicPlayer modplayer = Main.LocalPlayer.GetModPlayer<DNDMagicPlayer>();
            modplayer.HeightenedSpell = !modplayer.HeightenedSpell;
            return false;
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            rare = ItemRarityID.LightRed;
            DNDMagicPlayer modplayer = Main.LocalPlayer.GetModPlayer<DNDMagicPlayer>();
            tip += "\n" + (modplayer.HeightenedSpell ? LangLibrary.MetaMagicOn : LangLibrary.MetaMagicOff);
        }
    }

    public class DistantSpellMMBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
        {
            DNDMagicPlayer modplayer = Main.LocalPlayer.GetModPlayer<DNDMagicPlayer>();
            Texture2D bg = ModContent.Request<Texture2D>("BG3MagicRework/Buffs/MetaMagic").Value;
            Color alpha;
            if (modplayer.DistantSpellMM)
            {
                alpha = Color.White;
            }
            else
            {
                alpha = drawParams.DrawColor;
            }
            spriteBatch.Draw(bg, drawParams.Position, drawParams.SourceRectangle, alpha);
            EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
            spriteBatch.Draw(drawParams.Texture, drawParams.Position, drawParams.SourceRectangle, alpha);
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }

        public override bool RightClick(int buffIndex)
        {
            DNDMagicPlayer modplayer = Main.LocalPlayer.GetModPlayer<DNDMagicPlayer>();
            modplayer.DistantSpellMM = !modplayer.DistantSpellMM;
            return false;
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            rare = ItemRarityID.LightRed;
            DNDMagicPlayer modplayer = Main.LocalPlayer.GetModPlayer<DNDMagicPlayer>();
            tip += "\n" + (modplayer.DistantSpellMM ? LangLibrary.MetaMagicOn : LangLibrary.MetaMagicOff);
        }
    }

    public class ExtendedSpellMMBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
        {
            DNDMagicPlayer modplayer = Main.LocalPlayer.GetModPlayer<DNDMagicPlayer>();
            Texture2D bg = ModContent.Request<Texture2D>("BG3MagicRework/Buffs/MetaMagic").Value;
            Color alpha;
            if (modplayer.ExtendedSpellMM)
            {
                alpha = Color.White;
            }
            else
            {
                alpha = drawParams.DrawColor;
            }
            spriteBatch.Draw(bg, drawParams.Position, drawParams.SourceRectangle, alpha);
            EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
            spriteBatch.Draw(drawParams.Texture, drawParams.Position, drawParams.SourceRectangle, alpha);
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }

        public override bool RightClick(int buffIndex)
        {
            DNDMagicPlayer modplayer = Main.LocalPlayer.GetModPlayer<DNDMagicPlayer>();
            modplayer.ExtendedSpellMM = !modplayer.ExtendedSpellMM;
            return false;
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            rare = ItemRarityID.LightRed;
            DNDMagicPlayer modplayer = Main.LocalPlayer.GetModPlayer<DNDMagicPlayer>();
            tip += "\n" + (modplayer.ExtendedSpellMM ? LangLibrary.MetaMagicOn : LangLibrary.MetaMagicOff);
        }
    }

    public class CarefulSpellMMBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
        {
            DNDMagicPlayer modplayer = Main.LocalPlayer.GetModPlayer<DNDMagicPlayer>();
            Texture2D bg = ModContent.Request<Texture2D>("BG3MagicRework/Buffs/MetaMagic").Value;
            Color alpha;
            if (modplayer.CarefulSpellMM)
            {
                alpha = Color.White;

            }
            else
            {
                alpha = drawParams.DrawColor;
            }
            spriteBatch.Draw(bg, drawParams.Position, drawParams.SourceRectangle, alpha);
            EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
            spriteBatch.Draw(drawParams.Texture, drawParams.Position, drawParams.SourceRectangle, alpha);
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }

        public override bool RightClick(int buffIndex)
        {
            DNDMagicPlayer modplayer = Main.LocalPlayer.GetModPlayer<DNDMagicPlayer>();
            modplayer.CarefulSpellMM = !modplayer.CarefulSpellMM;
            return false;
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            rare = ItemRarityID.LightRed;
            DNDMagicPlayer modplayer = Main.LocalPlayer.GetModPlayer<DNDMagicPlayer>();
            tip += "\n" + (modplayer.CarefulSpellMM ? LangLibrary.MetaMagicOn : LangLibrary.MetaMagicOff);
        }
    }

    public class TwinnedSpellMMBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
        {
            DNDMagicPlayer modplayer = Main.LocalPlayer.GetModPlayer<DNDMagicPlayer>();
            Texture2D bg = ModContent.Request<Texture2D>("BG3MagicRework/Buffs/MetaMagic").Value;
            Color alpha;
            if (modplayer.TwinnedSpellMM)
            {
                alpha = Color.White;
            }
            else
            {
                alpha = drawParams.DrawColor;
            }
            spriteBatch.Draw(bg, drawParams.Position, drawParams.SourceRectangle, alpha);
            EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
            spriteBatch.Draw(drawParams.Texture, drawParams.Position, drawParams.SourceRectangle, alpha);
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }

        public override bool RightClick(int buffIndex)
        {
            DNDMagicPlayer modplayer = Main.LocalPlayer.GetModPlayer<DNDMagicPlayer>();
            modplayer.TwinnedSpellMM = !modplayer.TwinnedSpellMM;
            return false;
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            rare = ItemRarityID.LightRed;
            DNDMagicPlayer modplayer = Main.LocalPlayer.GetModPlayer<DNDMagicPlayer>();
            tip += "\n" + (modplayer.TwinnedSpellMM ? LangLibrary.MetaMagicOn : LangLibrary.MetaMagicOff);
        }
    }
}
