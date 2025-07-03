using BG3MagicRework.BaseType;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BG3MagicRework.Systems
{
    public class ModifyDrawUI : ModSystem
    {
        public override void Load()
        {
            On_ClassicPlayerResourcesDisplaySet.DrawMana += On_ClassicPlayerResourcesDisplaySet_DrawMana;
            On_CommonResourceBarMethods.DrawManaMouseOver += On_CommonResourceBarMethods_DrawManaMouseOver;
            On_FancyClassicPlayerResourcesDisplaySet.DrawManaBar += On_FancyClassicPlayerResourcesDisplaySet_DrawManaBar;
            On_FancyClassicPlayerResourcesDisplaySet.DrawManaText += On_FancyClassicPlayerResourcesDisplaySet_DrawManaText;
            On_HorizontalBarsPlayerResourcesDisplaySet.DrawManaText += On_HorizontalBarsPlayerResourcesDisplaySet_DrawManaText;
            On_HorizontalBarsPlayerResourcesDisplaySet.ManaPanelDrawer += On_HorizontalBarsPlayerResourcesDisplaySet_ManaPanelDrawer;
            On_HorizontalBarsPlayerResourcesDisplaySet.ManaFillingDrawer += On_HorizontalBarsPlayerResourcesDisplaySet_ManaFillingDrawer;
            On_HorizontalBarsPlayerResourcesDisplaySet.Draw += On_HorizontalBarsPlayerResourcesDisplaySet_Draw;
            On_Main.DrawInterface_36_Cursor += On_Main_DrawInterface_36_Cursor;
        }

        public override void Unload()
        {
            On_ClassicPlayerResourcesDisplaySet.DrawMana -= On_ClassicPlayerResourcesDisplaySet_DrawMana;
            On_CommonResourceBarMethods.DrawManaMouseOver -= On_CommonResourceBarMethods_DrawManaMouseOver;
            On_FancyClassicPlayerResourcesDisplaySet.DrawManaBar -= On_FancyClassicPlayerResourcesDisplaySet_DrawManaBar;
            On_FancyClassicPlayerResourcesDisplaySet.DrawManaText -= On_FancyClassicPlayerResourcesDisplaySet_DrawManaText;
            On_HorizontalBarsPlayerResourcesDisplaySet.DrawManaText -= On_HorizontalBarsPlayerResourcesDisplaySet_DrawManaText;
            On_HorizontalBarsPlayerResourcesDisplaySet.ManaPanelDrawer -= On_HorizontalBarsPlayerResourcesDisplaySet_ManaPanelDrawer;
            On_HorizontalBarsPlayerResourcesDisplaySet.ManaFillingDrawer -= On_HorizontalBarsPlayerResourcesDisplaySet_ManaFillingDrawer;
            On_HorizontalBarsPlayerResourcesDisplaySet.Draw -= On_HorizontalBarsPlayerResourcesDisplaySet_Draw;
            On_Main.DrawInterface_36_Cursor -= On_Main_DrawInterface_36_Cursor;
        }


        private static void On_HorizontalBarsPlayerResourcesDisplaySet_ManaFillingDrawer(On_HorizontalBarsPlayerResourcesDisplaySet.orig_ManaFillingDrawer orig, HorizontalBarsPlayerResourcesDisplaySet self, int elementIndex, int firstElementIndex, int lastElementIndex, out ReLogic.Content.Asset<Texture2D> sprite, out Vector2 offset, out float drawScale, out Rectangle? sourceRect)
        {
            sprite = Terraria.GameContent.TextureAssets.MagicPixel;
            offset = Vector2.Zero;
            drawScale = 0;
            sourceRect = new Rectangle(0, 0, 0, 0);
        }

        private static void On_HorizontalBarsPlayerResourcesDisplaySet_ManaPanelDrawer(On_HorizontalBarsPlayerResourcesDisplaySet.orig_ManaPanelDrawer orig, HorizontalBarsPlayerResourcesDisplaySet self, int elementIndex, int firstElementIndex, int lastElementIndex, out ReLogic.Content.Asset<Texture2D> sprite, out Vector2 offset, out float drawScale, out Rectangle? sourceRect)
        {
            sprite = Terraria.GameContent.TextureAssets.MagicPixel;
            offset = Vector2.Zero;
            drawScale = 0;
            sourceRect = new Rectangle(0, 0, 0, 0);
        }

        private static void On_HorizontalBarsPlayerResourcesDisplaySet_DrawManaText(On_HorizontalBarsPlayerResourcesDisplaySet.orig_DrawManaText orig, SpriteBatch spriteBatch)
        {

        }

        private static void On_FancyClassicPlayerResourcesDisplaySet_DrawManaText(On_FancyClassicPlayerResourcesDisplaySet.orig_DrawManaText orig, SpriteBatch spriteBatch)
        {

        }

        private static void On_HorizontalBarsPlayerResourcesDisplaySet_Draw(On_HorizontalBarsPlayerResourcesDisplaySet.orig_Draw orig, HorizontalBarsPlayerResourcesDisplaySet self)
        {
            orig.Invoke(self);
            FieldInfo fieldInfo = typeof(HorizontalBarsPlayerResourcesDisplaySet).GetField("_drawTextStyle", BindingFlags.Instance | BindingFlags.NonPublic);
            byte _drawTextStyle = (byte)fieldInfo.GetValue(self);
            int offset = 0;
            if (_drawTextStyle == 1) offset = 4;
            if (_drawTextStyle == 2) offset = 2;
            Texture2D tex0 = ModContent.Request<Texture2D>("BG3MagicRework/Buffs/SpellSlotBuff").Value;
            if (Main.LocalPlayer.TryGetModPlayer(out DNDMagicPlayer modplayer))
            {
                float d = 0;
                float offsetX = 260;
                //if (modplayer.MaxSorceryPoint > 0) offsetX += tex0.Height + 6;
                for (int i = 1; i <= 6; i++)
                {
                    if (modplayer.MaxSpellSlot.ContainsKey(i))
                    {
                        Vector2 position = new(Main.screenWidth - tex0.Width / 2f - offsetX + (tex0.Height + 6) * d, 64 + offset);
                        DrawSpellSlot(position, i);
                        d++;
                        Rectangle rect = new((int)position.X - tex0.Width / 2, (int)position.Y - tex0.Height / 2, tex0.Width, tex0.Height);
                        if (rect.Contains(Main.mouseX, Main.mouseY))
                        {
                            if (!Main.mouseText)
                            {
                                Main.instance.MouseText(string.Format(Language.GetTextValue("Mods.BG3MagicRework.SpellSlotDesc"), i, modplayer.RemainingSpellSlot[i], modplayer.MaxSpellSlot[i]));
                                Main.mouseText = true;
                            }
                        }
                    }
                }
                if (modplayer.MaxSorceryPoint > 0)
                {
                    Vector2 position = new(Main.screenWidth - tex0.Width / 2f - offsetX + (tex0.Height + 6) * d, 64 + offset);
                    DrawSP(position);
                    Rectangle rect = new((int)position.X - tex0.Width / 2, (int)position.Y - tex0.Height / 2, tex0.Width, tex0.Height);
                    if (rect.Contains(Main.mouseX, Main.mouseY))
                    {
                        if (!Main.mouseText)
                        {
                            Main.instance.MouseText(string.Format(Language.GetTextValue("Mods.BG3MagicRework.SorceryPointDesc"), modplayer.RemainingSorceryPoint, modplayer.MaxSorceryPoint));
                            Main.mouseText = true;
                        }
                    }
                }
            }
        }



        private static void On_FancyClassicPlayerResourcesDisplaySet_DrawManaBar(On_FancyClassicPlayerResourcesDisplaySet.orig_DrawManaBar orig, FancyClassicPlayerResourcesDisplaySet self, SpriteBatch spriteBatch)
        {
            Texture2D tex0 = ModContent.Request<Texture2D>("BG3MagicRework/Buffs/SpellSlotBuff").Value;
            if (Main.LocalPlayer.TryGetModPlayer(out DNDMagicPlayer modplayer))
            {
                float d = 0;
                for (int i = 1; i <= 6; i++)
                {
                    if (modplayer.MaxSpellSlot.ContainsKey(i))
                    {
                        Vector2 position = new(Main.screenWidth - tex0.Width / 2f - 6, 45 + (tex0.Height + 6) * d);
                        DrawSpellSlot(position, i);
                        d++;
                        Rectangle rect = new((int)position.X - tex0.Width / 2, (int)position.Y - tex0.Height / 2, tex0.Width, tex0.Height);
                        if (rect.Contains(Main.mouseX, Main.mouseY))
                        {
                            if (!Main.mouseText)
                            {
                                Main.instance.MouseText(string.Format(Language.GetTextValue("Mods.BG3MagicRework.SpellSlotDesc"), i, modplayer.RemainingSpellSlot[i], modplayer.MaxSpellSlot[i]));
                                Main.mouseText = true;
                            }
                        }
                    }
                }

                if (modplayer.MaxSorceryPoint > 0)
                {
                    Vector2 position = new(Main.screenWidth - tex0.Width / 2f - 6, 45 + (tex0.Height + 6) * d);
                    DrawSP(position);
                    Rectangle rect = new((int)position.X - tex0.Width / 2, (int)position.Y - tex0.Height / 2, tex0.Width, tex0.Height);
                    if (rect.Contains(Main.mouseX, Main.mouseY))
                    {
                        if (!Main.mouseText)
                        {
                            Main.instance.MouseText(string.Format(Language.GetTextValue("Mods.BG3MagicRework.SorceryPointDesc"), modplayer.RemainingSorceryPoint, modplayer.MaxSorceryPoint));
                            Main.mouseText = true;
                        }
                    }
                }
            }
        }

        private static void On_CommonResourceBarMethods_DrawManaMouseOver(On_CommonResourceBarMethods.orig_DrawManaMouseOver orig)
        {

        }

        private static void On_ClassicPlayerResourcesDisplaySet_DrawMana(On_ClassicPlayerResourcesDisplaySet.orig_DrawMana orig, ClassicPlayerResourcesDisplaySet self)
        {
            Texture2D tex0 = ModContent.Request<Texture2D>("BG3MagicRework/Buffs/SpellSlotBuff").Value;
            if (Main.LocalPlayer.TryGetModPlayer(out DNDMagicPlayer modplayer))
            {
                float d = 0;
                for (int i = 1; i <= 6; i++)
                {
                    if (modplayer.MaxSpellSlot.ContainsKey(i))
                    {
                        Vector2 position = new(Main.screenWidth - tex0.Width / 2f - 6, 45 + (tex0.Height + 6) * d);
                        DrawSpellSlot(position, i);
                        d++;
                        Rectangle rect = new((int)position.X - tex0.Width / 2, (int)position.Y - tex0.Height / 2, tex0.Width, tex0.Height);
                        if (rect.Contains(Main.mouseX, Main.mouseY))
                        {
                            if (!Main.mouseText)
                            {
                                Main.instance.MouseText(string.Format(Language.GetTextValue("Mods.BG3MagicRework.SpellSlotDesc"), i, modplayer.RemainingSpellSlot[i], modplayer.MaxSpellSlot[i]));
                                Main.mouseText = true;
                            }
                        }
                    }
                }

                if (modplayer.MaxSorceryPoint > 0)
                {
                    Vector2 position = new(Main.screenWidth - tex0.Width / 2f - 6, 45 + (tex0.Height + 6) * d);
                    DrawSP(position);
                    Rectangle rect = new((int)position.X - tex0.Width / 2, (int)position.Y - tex0.Height / 2, tex0.Width, tex0.Height);
                    if (rect.Contains(Main.mouseX, Main.mouseY))
                    {
                        if (!Main.mouseText)
                        {
                            Main.instance.MouseText(string.Format(Language.GetTextValue("Mods.BG3MagicRework.SorceryPointDesc"), modplayer.RemainingSorceryPoint, modplayer.MaxSorceryPoint));
                            Main.mouseText = true;
                        }
                    }
                }
            }
        }

        private static void DrawSpellSlot(Vector2 Center, int ring)
        {
            int MaxCount = 0;
            foreach (KeyValuePair<int, int> item in Main.LocalPlayer.GetModPlayer<DNDMagicPlayer>().MaxSpellSlot)
            {
                if (item.Key == ring)
                {
                    MaxCount = item.Value;
                    break;
                }
            }

            int RemainingCount = 0;
            foreach (KeyValuePair<int, int> item in Main.LocalPlayer.GetModPlayer<DNDMagicPlayer>().RemainingSpellSlot)
            {
                if (item.Key == ring)
                {
                    RemainingCount = item.Value;
                    break;
                }
            }

            Texture2D tex0 = ModContent.Request<Texture2D>("BG3MagicRework/Buffs/SpellSlotBuff").Value;
            Texture2D tex1 = ModContent.Request<Texture2D>("BG3MagicRework/Buffs/SpellSlotIcon").Value;
            Texture2D tex2 = ModContent.Request<Texture2D>("BG3MagicRework/Buffs/SpellSlotIcon2").Value;
            float d = tex0.Size().X / 6f;
            Main.spriteBatch.Draw(tex0, Center, null, Color.White, 0, tex0.Size() / 2f, 1f, SpriteEffects.None, 0);
            if (RemainingCount > 4)
            {
                Main.spriteBatch.Draw(tex1, Center, null, Color.White, 0, tex1.Size() / 2f, 1f, SpriteEffects.None, 0);
                Utils.DrawBorderString(Main.spriteBatch, RemainingCount.ToString(), Center + new Vector2(0, d * 2f), Color.White, 0.75f, 0.5f, 0.5f);
            }
            else
            {
                List<Vector2> DrawPos = new();
                switch (MaxCount)
                {
                    case 0:
                        break;
                    case 1:
                        DrawPos = new() { Center };
                        break;
                    case 2:
                        DrawPos = new() { Center + new Vector2(-d, 0), Center + new Vector2(d, 0) };
                        break;
                    case 3:
                        DrawPos = new() { Center + new Vector2(-d, -d), Center + new Vector2(d, -d), Center + new Vector2(-d, d) };
                        break;
                    default:
                        DrawPos = new() { Center + new Vector2(-d, -d), Center + new Vector2(d, -d), Center + new Vector2(-d, d), Center + new Vector2(d, d) };
                        break;
                }
                if (MaxCount > 0)
                {
                    for (int i = 0; i < Math.Min(MaxCount, 4); i++)
                    {
                        Main.spriteBatch.Draw(i < RemainingCount ? tex1 : tex2, DrawPos[i], null, Color.White, 0, tex1.Size() / 2f, 1f, SpriteEffects.None, 0);
                    }
                }
            }


            Utils.DrawBorderString(Main.spriteBatch, SomeUtils.RomanNumber(ring), Center + new Vector2(0, -d * 2), Color.White, 0.75f, 0.5f, 0.5f);
        }

        private static void DrawSP(Vector2 Center)
        {
            EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
            int SPValue = Main.LocalPlayer.GetModPlayer<DNDMagicPlayer>().RemainingSorceryPoint;
            string str = SPValue.ToString();
            if (SPValue <= 1) str = "";

            Texture2D tex0 = ModContent.Request<Texture2D>("BG3MagicRework/Buffs/SpellSlotBuff").Value;
            Texture2D tex1 = ModContent.Request<Texture2D>("BG3MagicRework/Buffs/SorceryPointIcon").Value;
            if (SPValue <= 0)
            {
                tex1 = ModContent.Request<Texture2D>("BG3MagicRework/Buffs/SorceryPointIcon2").Value;
            }
            float d = tex0.Size().X / 6f;
            Main.spriteBatch.Draw(tex0, Center, null, Color.White, 0, tex0.Size() / 2f, 1f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex1, Center, null, Color.White, 0, tex1.Size() / 2f, 0.75f, SpriteEffects.None, 0);
            Utils.DrawBorderString(Main.spriteBatch, str, Center + new Vector2(0, d * 2f), Color.White, 0.75f, 0.5f, 0.5f);
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
        }

        public static string DrawSpellName = "";

        private static void On_Main_DrawInterface_36_Cursor(On_Main.orig_DrawInterface_36_Cursor orig)
        {
            orig.Invoke();
            if (Main.gameMenu || Main.LocalPlayer == null) return;
            string result = DrawSpellName;
            DrawSpellName = "";
            if (result != "" && EverythingLibrary.spells.TryGetValue(result, out BaseSpell value))
            {
                EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
                Texture2D tex = value.GetIcon();
                if (result == "GlyphOfWarding")
                {
                    tex = ModContent.Request<Texture2D>("BG3MagicRework/Images/Icons/" + result + "_" + Main.LocalPlayer.GetModPlayer<DNDMagicPlayer>().GoWType.ToString(), AssetRequestMode.ImmediateLoad).Value;
                }
                float scale = 40f / tex.Width;
                Main.spriteBatch.Draw(tex, Main.MouseWorld + new Vector2(20, 0) - Main.screenPosition, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            }
        }


    }
}
