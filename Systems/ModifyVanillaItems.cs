using BG3MagicRework.BaseType;
using BG3MagicRework.Buffs;
using BG3MagicRework.Buffs.Enemy;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace BG3MagicRework.Systems
{
    public class ModifyVanillaItems : GlobalItem
    {
        public override void SetDefaults(Item entity)
        {
            if (EverythingLibrary.itemModifiers.TryGetValue(entity.type, out BaseItemModifier result))
            {
                result.SetDefaults(entity);
            }
            if (EverythingLibrary.weaponModifiers.TryGetValue(entity.type, out BaseWeaponModifier result2))
            {
                result2.SetDefaults(entity);
            }
        }

        public override bool CanUseItem(Item item, Player player)
        {
            if (EverythingLibrary.weaponModifiers.TryGetValue(item.type, out BaseWeaponModifier result))
            {
                if (EverythingLibrary.spells[result.SpellName].IsReaction) return false;
                if (player.HasBuff(ModContent.BuffType<ArcaneHungerBuff>())) return false;
                if (player.altFunctionUse != 2)
                {
                    return player.GetAvailableRings(EverythingLibrary.spells[result.SpellName].InitialRing).Count > 0 && result.CanUse(item, player);
                }
                else
                {
                    return result.CanUse(item, player, true);
                }
            }
            return true;
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (EverythingLibrary.weaponModifiers.TryGetValue(item.type, out BaseWeaponModifier result))
            {
                return result.Shoot(item, player, source, position, velocity, type, damage, knockback);
            }
            return true;
        }

        public override bool AltFunctionUse(Item item, Player player)
        {
            if (EverythingLibrary.weaponModifiers.TryGetValue(item.type, out BaseWeaponModifier result))
            {
                return result.AlterFunctionUse;
            }
            return false;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            if (EverythingLibrary.itemModifiers.TryGetValue(item.type, out BaseItemModifier result))
            {
                result.UpdateEquip(item, player);
            }
        }

        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            foreach (BaseArmorSetModifier m in EverythingLibrary.armorSetModifiers)
            {
                bool bool1 = m.Head.Count == 0 || m.Head.Contains(head.type);
                bool bool2 = m.Body.Count == 0 || m.Body.Contains(body.type);
                bool bool3 = m.Legs.Count == 0 || m.Legs.Contains(legs.type);
                if (bool1 && bool2 && bool3)
                {
                    return m.Name;
                }
            }

            return string.Empty;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            foreach (BaseArmorSetModifier m in EverythingLibrary.armorSetModifiers)
            {
                if (set == m.Name)
                {
                    player.setBonus = Language.GetTextValue("Mods.BG3MagicRework.ArmorSets." + m.Name);
                    m.UpdateArmorSet(player);
                }
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (EverythingLibrary.weaponModifiers.TryGetValue(item.type, out BaseWeaponModifier result1))
            {
                if (result1.SpellName != "" && EverythingLibrary.spells.TryGetValue(result1.SpellName, out BaseSpell value))
                {
                    int FirstLine = -1;
                    for (int i = tooltips.Count - 1; i >= 0; i--)
                    {
                        if (tooltips[i].Mod == "Terraria" && (tooltips[i].Name.Contains("ItemName") || tooltips[i].Name.Contains("Favorite")))
                        {
                            FirstLine = i;
                            break;
                        }
                    }
                    if (FirstLine != -1)
                    {
                        for (int i = tooltips.Count - 1; i >= 0; i--)
                        {
                            if (tooltips[i].Mod == "Terraria" && tooltips[i].Name.Contains("Tooltip"))
                            {
                                tooltips.RemoveAt(i);
                            }
                        }

                        TooltipLine line;
                        tooltips.Insert(FirstLine + 1, new TooltipLine(Mod, "SpellDesc_OtherDescs", value.GetOtherDescs()));
                        line = new(Mod, "SpellDesc_RisingRingDesc", value.GetRisingRingDesc());
                        line.OverrideColor = Color.LightGray;
                        tooltips.Insert(FirstLine + 1, line);
                        tooltips.Insert(FirstLine + 1, new TooltipLine(Mod, "SpellDesc_RisingRingTitle", value.GetRisingRingTitle()));
                        tooltips.Insert(FirstLine + 1, new TooltipLine(Mod, "SpellDesc_TimeSpanDesc", value.GetTimeSpanDesc()));
                        line = new(Mod, "SpellDesc_Desc", value.GetDesc());
                        line.OverrideColor = Color.LightGray;
                        tooltips.Insert(FirstLine + 1, line);
                        tooltips.Insert(FirstLine + 1, new TooltipLine(Mod, "SpellDesc_ExtraDiceCalcDesc", value.GetExtraDiceCalcDesc()));
                        tooltips.Insert(FirstLine + 1, new TooltipLine(Mod, "SpellDesc_ExtraDiceTitle", value.GetExtraDiceTitle()));
                        tooltips.Insert(FirstLine + 1, new TooltipLine(Mod, "SpellDesc_BaseDiceCalcDesc", value.GetBaseDiceCalcDesc()));
                        tooltips.Insert(FirstLine + 1, new TooltipLine(Mod, "SpellDesc_DamageDesc", value.GetDamageDesc()));
                        line = new(Mod, "SpellDesc_SchoolDesc", value.GetSchoolDesc());
                        line.OverrideColor = Color.LightGray;
                        tooltips.Insert(FirstLine + 1, line);
                        line = new(Mod, "SpellName", value.GetName());
                        line.OverrideColor = value.NameColor;
                        tooltips.Insert(FirstLine + 1, line);
                    }
                }
            }
            else if (EverythingLibrary.itemModifiers.TryGetValue(item.type, out BaseItemModifier result2))
            {
                int FirstLine = -1;
                for (int i = 0; i < tooltips.Count; i++)
                {
                    if (tooltips[i].Mod == "Terraria" && tooltips[i].Name.Contains("Tooltip"))
                    {
                        FirstLine = i;
                        break;
                    }
                }
                if (FirstLine != -1)
                {
                    string newString = result2.GetTooltip();
                    if (newString != "")
                    {
                        for (int i = tooltips.Count - 1; i >= 0; i--)
                        {
                            if (tooltips[i].Mod == "Terraria" && tooltips[i].Name.Contains("Tooltip"))
                            {
                                tooltips.RemoveAt(i);
                            }
                        }
                        tooltips.Insert(FirstLine, new TooltipLine(this.Mod, "NewTooltip", newString));
                    }
                }


            }
        }

        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            if (EverythingLibrary.weaponModifiers.TryGetValue(item.type, out BaseWeaponModifier result))
            {
                if (result.SpellName != "" && EverythingLibrary.spells.TryGetValue(result.SpellName, out BaseSpell value))
                {
                    if (line.Mod == Mod.Name && line.Name == "SpellDesc_SchoolDesc")
                    {
                        line.BaseScale *= 0.9f;
                    }
                    if (line.Mod == Mod.Name && line.Name == "SpellDesc_Desc")
                    {
                        line.BaseScale *= 0.9f;
                    }
                    if (line.Mod == Mod.Name && line.Name == "SpellDesc_BaseDiceCalcDesc")
                    {
                        if (!value.BaseDamage.NoDiceDamage())
                        {
                            line.X += Main.LocalPlayer.GetDiceDamage(value.BaseDamage, value.InitialRing, value.InitialRing, value.RisingDamageAddition).GetDiceCount() == 1 ? 30 : 70;
                        }
                        line.BaseScale *= 0.8f;
                    }
                    if (line.Mod == Mod.Name && line.Name == "SpellDesc_ExtraDiceCalcDesc")
                    {
                        if (!value.ExtraDamage.NoDiceDamage())
                        {
                            line.X += Main.LocalPlayer.GetDiceDamage(value.ExtraDamage, value.InitialRing, value.InitialRing, value.RisingDamageAdditionExtra).GetDiceCount() == 1 ? 30 : 70;
                        }
                        line.BaseScale *= 0.8f;
                    }
                    if (line.Mod == Mod.Name && line.Name == "SpellDesc_RisingRingDesc")
                    {
                        line.BaseScale *= 0.9f;
                    }
                }
            }
            return true;
        }

        public override void PostDrawTooltip(Item item, ReadOnlyCollection<DrawableTooltipLine> lines)
        {
            if (EverythingLibrary.weaponModifiers.TryGetValue(item.type, out BaseWeaponModifier result))
            {
                if (result.SpellName != "" && EverythingLibrary.spells.TryGetValue(result.SpellName, out BaseSpell value))
                {
                    EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
                    if (lines.Count > 1)
                    {
                        float Y = lines[0].Y - 45;
                        float Right = 0;
                        foreach (DrawableTooltipLine line in lines)
                        {
                            float _right = line.X + ChatManager.GetStringSize(FontAssets.MouseText.Value, line.Text, new(1, 1)).X * line.BaseScale.X - 120;
                            Right = Math.Max(Right, _right);
                        }
                        Main.spriteBatch.Draw(value.GetIcon(), new Vector2(Right, Y), null, Color.White, 0, Vector2.Zero, 0.4f, SpriteEffects.None, 0);
                    }

                    foreach (DrawableTooltipLine line in lines)
                    {
                        if (line.Mod == Mod.Name && line.Name == "SpellDesc_BaseDiceCalcDesc")
                        {
                            if (!value.BaseDamage.NoDamage())
                            {
                                Main.LocalPlayer.GetDiceDamage(value.BaseDamage, value.InitialRing, value.InitialRing, value.RisingDamageAddition).DrawDice(new Vector2(line.OriginalX, line.OriginalY));
                            }
                        }
                        if (line.Mod == Mod.Name && line.Name == "SpellDesc_ExtraDiceCalcDesc")
                        {
                            if (!value.ExtraDamage.NoDamage())
                            {
                                Main.LocalPlayer.GetDiceDamage(value.ExtraDamage, value.InitialRing, value.InitialRing, value.RisingDamageAdditionExtra).DrawDice(new Vector2(line.OriginalX, line.OriginalY));
                            }
                        }
                    }
                    EasyDraw.AnotherDraw(BlendState.AlphaBlend);
                }
            }
        }

        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (EverythingLibrary.weaponModifiers.TryGetValue(item.type, out BaseWeaponModifier result))
            {
                if (result.SpellName != "" && EverythingLibrary.spells.TryGetValue(result.SpellName, out BaseSpell value))
                {
                    if (value.IsReaction)
                    {
                        Texture2D tex = TextureLibrary.ReactionIcon;
                        Vector2 DrawCenter = position + new Vector2(12f, -12f);
                        float DrawScale = 24f / tex.Width;
                        EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
                        spriteBatch.Draw(tex, DrawCenter, null, drawColor, 0, tex.Size() / 2f, DrawScale, SpriteEffects.None, 0);
                        EasyDraw.AnotherDraw(BlendState.AlphaBlend);
                    }
                }
            }
        }


        public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration)
        {
            if (player.HasBuff(ModContent.BuffType<DisadvantageTerrainBuff2>()))
            {
                speed *= 0.25f;
                acceleration *= 0.25f;
            }
            else if (player.HasBuff(ModContent.BuffType<DisadvantageTerrainBuff>()))
            {
                speed *= 0.5f;
                acceleration *= 0.5f;
            }
        }
        /// <summary>
        /// 劣势地形影响翅膀的上升速度
        /// </summary>
        /// <param name="item"></param>
        /// <param name="player"></param>
        /// <param name="ascentWhenFalling"></param>
        /// <param name="ascentWhenRising"></param>
        /// <param name="maxCanAscendMultiplier"></param>
        /// <param name="maxAscentMultiplier"></param>
        /// <param name="constantAscend"></param>
        public override void VerticalWingSpeeds(Item item, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            if (player.HasBuff(ModContent.BuffType<DisadvantageTerrainBuff2>()))
            {
                ascentWhenFalling *= 0.25f;
                ascentWhenRising *= 0.25f;
                maxAscentMultiplier *= 0.25f;
                maxCanAscendMultiplier *= 0.25f;
                constantAscend *= 0.25f;
            }
            else if (player.HasBuff(ModContent.BuffType<DisadvantageTerrainBuff>()))
            {
                ascentWhenFalling *= 0.5f;
                ascentWhenRising *= 0.5f;
                maxAscentMultiplier *= 0.5f;
                maxCanAscendMultiplier *= 0.5f;
                constantAscend *= 0.5f;
            }
        }
    }

}