using BG3MagicRework.Static;
using BG3MagicRework.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Buffs
{
    public class HRTrigger : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override bool RightClick(int buffIndex)
        {
            if (Main.LocalPlayer.TryGetModPlayer(out DNDMagicPlayer result))
            {
                result.HellishRebukeTrigger = !result.HellishRebukeTrigger;
            }
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
        {
            drawParams.DrawColor = Color.White;
            return true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, int buffIndex, BuffDrawParams drawParams)
        {
            if (Main.LocalPlayer.TryGetModPlayer(out DNDMagicPlayer result))
            {
                if (result.HellishRebukeTrigger)
                {
                    Texture2D tex = ModContent.Request<Texture2D>("BG3MAgicRework/Buffs/ReactionEnabled", AssetRequestMode.ImmediateLoad).Value;
                    spriteBatch.Draw(tex, drawParams.Position, drawParams.SourceRectangle, drawParams.DrawColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                }
            }
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            rare = ItemRarityID.Orange;
            if (Main.LocalPlayer.TryGetModPlayer(out DNDMagicPlayer result))
            {
                if (result.HellishRebukeTrigger)
                {
                    tip = LangLibrary.ReactionOn;
                }
                else
                {
                    tip = LangLibrary.ReactionOff;
                }
            }
        }


    }
}
