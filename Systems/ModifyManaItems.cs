using BG3MagicRework.Buffs;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BG3MagicRework.Systems
{
    public class ModifyManaItems : GlobalItem
    {
        public override void SetDefaults(Item entity)
        {
            switch (entity.type)
            {
                case ItemID.ManaPotion:
                case ItemID.LesserManaPotion:
                case ItemID.GreaterManaPotion:
                case ItemID.SuperManaPotion:
                    //entity.buffType = ModContent.BuffType<DNDManaSickness>();
                    entity.healMana = 0;
                    entity.potion = false;
                    break;
            }
        }

        public override bool CanUseItem(Item item, Player player)
        {
            int canHealRing = GetPotionRecoverSlot(item.type);
            if (canHealRing > 0)
            {
                if (player.HasBuff(ModContent.BuffType<DNDManaSickness>()))
                {
                    return false;
                }
            }
            return true;
        }

        public override bool? UseItem(Item item, Player player)
        {
            int canHealRing = GetPotionRecoverSlot(item.type);
            if (canHealRing > 0)
            {
                int result = RecoverSpellSlot(player, canHealRing);
                player.AddBuff(ModContent.BuffType<DNDManaSickness>(), CombatStat.ManaPotionSicknessCD);
                if (result != -1)
                {
                    CombatText.NewText(player.getRect(), CombatText.HealMana, "+" + string.Format(LangLibrary.XRingSlot, result));
                }
                return true;
            }
            return null;
        }


        private static int RecoverSpellSlot(Player player, int ring)
        {
            DNDMagicPlayer modplayer = player.GetModPlayer<DNDMagicPlayer>();
            int result = -1;
            foreach (int lvl in modplayer.ConsumedSpellSlot.Keys)
            {
                if (lvl <= ring && modplayer.ConsumedSpellSlot[lvl] > 0)
                {
                    if (lvl > result) result = lvl;
                }
            }
            if (result > 0)
            {
                modplayer.ConsumedSpellSlot[result]--;
                return result;
            }
            return -1;
        }

        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            if (item.type == ItemID.Star || item.type == ItemID.SoulCake || item.type == ItemID.SugarPlum)
            {
                item.active = false;
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            int canHealRing = GetPotionRecoverSlot(item.type);
            if (canHealRing > 0)
            {
                for (int i = tooltips.Count - 1; i >= 0; i--)
                {
                    if (tooltips[i].Mod == "Terraria" && tooltips[i].Name.Contains("ItemName"))
                    {
                        tooltips.Insert(i + 1, new TooltipLine(this.Mod, "HealSpellSlot", string.Format(Language.GetTextValue("Mods.BG3MagicRework.TooltipModify.RecoverAXRingSlot"), canHealRing)));
                        break;
                    }
                }
            }
        }

        public override void Load()
        {
            On_Player.QuickMana += On_Player_QuickMana;
            On_Player.QuickMana_GetItemToUse += On_Player_QuickMana_GetItemToUse;
        }

        public override void Unload()
        {
            On_Player.QuickMana -= On_Player_QuickMana;
            On_Player.QuickMana_GetItemToUse -= On_Player_QuickMana_GetItemToUse;
        }

        private static int GetPotionRecoverSlot(int type)
        {
            switch (type)
            {
                case ItemID.LesserManaPotion:
                    return 1;
                case ItemID.ManaPotion:
                    return 2;
                case ItemID.GreaterManaPotion:
                    return 3;
                case ItemID.SuperManaPotion:
                    return 4;
            }
            return -1;
        }

        private static Item On_Player_QuickMana_GetItemToUse(On_Player.orig_QuickMana_GetItemToUse orig, Player self)
        {
            DNDMagicPlayer modplayer = self.GetModPlayer<DNDMagicPlayer>();
            int result = -1;           //寻找最大的已损失法术位
            foreach (int lvl in modplayer.ConsumedSpellSlot.Keys)
            {
                if (lvl <= 4)
                {
                    if (lvl > result) result = lvl;
                }
            }
            if (result == -1) return null;

            Item useitem = null;
            int DifferenceValue = 999;
            int SpellLevel = 0;
            for (int i = 0; i < 58; i++)
            {
                if (self.inventory[i].stack > 0 && self.inventory[i].type > ItemID.None && GetPotionRecoverSlot(self.inventory[i].type) > 0 && CombinedHooks.CanUseItem(self, self.inventory[i]))
                {
                    int potionLevel = GetPotionRecoverSlot(self.inventory[i].type);
                    int canHealLevel = GetCanRecoverSlot(self, potionLevel);
                    if (GetCanRecoverSlot(self, GetPotionRecoverSlot(self.inventory[i].type)) != -1)      //这个药水可以用来恢复缺失的法术位
                    {
                        if (useitem == null)
                        {
                            useitem = self.inventory[i];
                        }
                        else
                        {
                            if (potionLevel - canHealLevel < DifferenceValue)
                            {
                                useitem = self.inventory[i];
                                DifferenceValue = potionLevel - canHealLevel;
                                SpellLevel = canHealLevel;
                            }
                            else if (potionLevel - canHealLevel == DifferenceValue)
                            {
                                if (canHealLevel > SpellLevel)
                                {
                                    useitem = self.inventory[i];
                                    DifferenceValue = potionLevel - canHealLevel;
                                    SpellLevel = canHealLevel;
                                }
                            }
                        }
                    }
                }
            }
            if (self.useVoidBag())
            {
                for (int j = 0; j < 40; j++)
                {
                    if (self.bank4.item[j].stack > 0 && self.bank4.item[j].type > ItemID.None && GetPotionRecoverSlot(self.bank4.item[j].type) > 0 && CombinedHooks.CanUseItem(self, self.inventory[j]))
                    {
                        int potionLevel = GetPotionRecoverSlot(self.bank4.item[j].type);
                        int canHealLevel = GetCanRecoverSlot(self, potionLevel);
                        if (GetCanRecoverSlot(self, GetPotionRecoverSlot(self.bank4.item[j].type)) != -1)      //这个药水可以用来恢复缺失的法术位
                        {
                            if (useitem == null)
                            {
                                useitem = self.bank4.item[j];
                            }
                            else
                            {
                                if (potionLevel - canHealLevel < DifferenceValue)
                                {
                                    useitem = self.bank4.item[j];
                                    DifferenceValue = potionLevel - canHealLevel;
                                    SpellLevel = canHealLevel;
                                }
                                else if (potionLevel - canHealLevel == DifferenceValue)
                                {
                                    if (canHealLevel > SpellLevel)
                                    {
                                        useitem = self.bank4.item[j];
                                        DifferenceValue = potionLevel - canHealLevel;
                                        SpellLevel = canHealLevel;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return useitem;
        }

        private static int GetCanRecoverSlot(Player player, int ring)
        {
            DNDMagicPlayer modplayer = player.GetModPlayer<DNDMagicPlayer>();
            int result = -1;
            foreach (int lvl in modplayer.ConsumedSpellSlot.Keys)
            {
                if (lvl <= ring && modplayer.ConsumedSpellSlot[lvl] > 0)
                {
                    if (lvl > result) result = lvl;
                }
            }
            if (result > 0)
            {
                return result;
            }
            return -1;
        }

        private static void On_Player_QuickMana(On_Player.orig_QuickMana orig, Player self)
        {
            DNDMagicPlayer modplayer = self.GetModPlayer<DNDMagicPlayer>();
            if (self.cursed || self.CCed || self.dead || modplayer.ConsumedSpellSlot.Count == 0)
            {
                return;
            }
            Item item = self.QuickMana_GetItemToUse();

            if (item == null) return;

            MethodInfo methodInfo = typeof(Player).GetMethod("ItemCheck_CheckCanUse", BindingFlags.NonPublic | BindingFlags.Instance);
            bool returnValue = (bool)methodInfo.Invoke(self, new object[] { item });

            if (returnValue && !self.HasBuff(ModContent.BuffType<DNDManaSickness>()))
            {
                SoundEngine.PlaySound(item.UseSound, new Vector2?(self.position));
                ItemLoader.UseItem(item, self);
                if (item.consumable && ItemLoader.ConsumeItem(item, self))
                {
                    item.stack--;
                }
                if (item.stack <= 0)
                {
                    item.TurnToAir(false);
                }
                Recipe.FindRecipes(false);
            }
        }
    }
}
