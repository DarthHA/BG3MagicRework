using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Channel;
using BG3MagicRework.Projectiles.VirtualEffects;
using BG3MagicRework.Systems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Static
{
    public static class PlayerUtils
    {
        public static bool IsDead(this Player player)
        {
            return !player.active || player.dead || player.ghost;
        }

        public static bool HasRing(this Player player, int ring)
        {
            foreach (KeyValuePair<int, int> item in player.GetModPlayer<DNDMagicPlayer>().RemainingSpellSlot)
            {
                if (item.Key >= ring && item.Value > 0) return true;
            }
            return false;
        }

        public static bool LeftClick(this Player player)
        {
            if (Main.mouseLeft && Main.mouseLeftRelease && !player.mouseInterface && !Main.blockMouse)
            {
                return true;
            }
            return false;
        }

        public static bool RightClick(this Player player)
        {
            if (Main.mouseRight && Main.mouseRightRelease && !player.mouseInterface && !Main.blockMouse)
            {
                return true;
            }
            return false;
        }

        public static bool LeftPress(this Player player)
        {
            if (Main.mouseLeft && !player.mouseInterface && !Main.blockMouse)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检查对应环位的法术有无法术位可用
        /// </summary>
        /// <param name="player"></param>
        /// <param name="ring"></param>
        /// <returns></returns>
        public static List<int> GetAvailableRings(this Player player, int ring)
        {
            if (ring == 0) return new List<int> { 0 };
            List<int> result = new();
            foreach (KeyValuePair<int, int> item in player.GetModPlayer<DNDMagicPlayer>().RemainingSpellSlot)
            {
                if (item.Key >= ring && item.Value > 0)
                {
                    result.Add(item.Key);
                }
            }
            return result;
        }

        public static int GetSmallestAvailableRings(this Player player, int ring)
        {
            List<int> availableRings = player.GetAvailableRings(ring);
            if (availableRings.Count == 0) return 0;
            availableRings.Sort((x, y) => x.CompareTo(y));
            return availableRings[0];
        }

        public static int GetConcentration(this Player player, long uuid)
        {
            DNDMagicPlayer modplayer = player.GetModPlayer<DNDMagicPlayer>();
            for (int i = 0; i < modplayer.ConcentrationSlot.Count; i++)
            {
                if (modplayer.ConcentrationSlot[i].UUID == uuid)
                {
                    return i;
                }
            }
            return -1;
        }

        public static int GetConcentration<T>(this Player player) where T : BaseConcentration
        {
            DNDMagicPlayer modplayer = player.GetModPlayer<DNDMagicPlayer>();
            for (int i = 0; i < modplayer.ConcentrationSlot.Count; i++)
            {
                if (modplayer.ConcentrationSlot[i].GetType() == typeof(T))
                {
                    return i;
                }
            }
            return -1;
        }

        public static BaseConcentration GenerateConcentration<T>(this Player player, int ring, int time, bool OneOnly = true) where T : BaseConcentration
        {
            DNDMagicPlayer modplayer = player.GetModPlayer<DNDMagicPlayer>();
            if (OneOnly)
            {
                for (int i = modplayer.ConcentrationSlot.Count - 1; i >= 0; i--)
                {
                    if (modplayer.ConcentrationSlot[i].GetType() == typeof(T))
                    {
                        modplayer.ConcentrationSlot[i].TimeLeft = 0;
                    }
                }
            }
            BaseConcentration instance = BaseConcentration.NewConcentration<T>();
            instance.UUID = SomeUtils.GenerateUUID();
            instance.TimeLeft = time;
            instance.Ring = ring;
            modplayer.ConcentrationSlot.Add(instance);
            AdvancedCombatText.NewText(player.getRect(), Color.White, instance.GetName());
            if (ModContent.GetInstance<BG3Config>().ShowCombatInfo)
            {
                Main.NewText(string.Format(LangLibrary.XBeginConX, player.name, instance.GetName(false, false)));
            }
            return instance;
        }

        public static void DeleteConcentration(this Player player, long uuid)
        {
            DNDMagicPlayer modplayer = player.GetModPlayer<DNDMagicPlayer>();
            for (int i = modplayer.ConcentrationSlot.Count - 1; i >= 0; i--)
            {
                if (modplayer.ConcentrationSlot[i].UUID == uuid)
                {
                    modplayer.ConcentrationSlot[i].TimeLeft = 0;
                    break;
                }
            }
        }

        public static bool HasReaction<T>(this Player player) where T : BaseSpell
        {
            string spellName = "";
            foreach (var v in EverythingLibrary.spells.Values)
            {
                if (v.GetType() == typeof(T))
                {
                    spellName = v.Name;
                    break;
                }
            }

            if (spellName != "")
            {
                for (int i = 0; i < 58; i++)
                {
                    if (!player.inventory[i].IsAir)
                    {
                        if (EverythingLibrary.weaponModifiers.TryGetValue(player.inventory[i].type, out BaseWeaponModifier result))
                        {
                            if (result.SpellName == spellName) return true;
                        }
                    }
                }
            }
            return false;
        }

        public static int NewMagicProj(this Player player, Vector2 position, Vector2 velocity, int type, DiceDamage diceDamage, float knockBack = 0f, int ring = 0, float ai0 = 0, float ai1 = 0, float ai2 = 0)
        {
            int dmg = diceDamage.NoDamage() ? 0 : 10;
            int protmp = Projectile.NewProjectile(player.GetSource_FromThis("BG3Magic"), position, velocity, type, dmg, knockBack, player.whoAmI, ai0, ai1, ai2);
            if (protmp >= 0 && protmp < 1000)
            {
                if (Main.projectile[protmp].ModProjectile != null)
                {
                    if (Main.projectile[protmp].ModProjectile is BaseMagicProj)
                    {
                        (Main.projectile[protmp].ModProjectile as BaseMagicProj).CurrentRing = ring;
                        (Main.projectile[protmp].ModProjectile as BaseMagicProj).diceDamage = diceDamage;
                    }
                }
            }
            return protmp;
        }

        public static int New1DmgMagicProj(this Player player, Vector2 position, Vector2 velocity, int type, int ring = 0, float ai0 = 0, float ai1 = 0, float ai2 = 0)
        {
            int protmp = Projectile.NewProjectile(player.GetSource_FromThis("BG3Magic"), position, velocity, type, 1, 0, player.whoAmI, ai0, ai1, ai2);
            if (protmp >= 0 && protmp < 1000)
            {
                if (Main.projectile[protmp].ModProjectile != null)
                {
                    if (Main.projectile[protmp].ModProjectile is BaseMagicProj)
                    {
                        (Main.projectile[protmp].ModProjectile as BaseMagicProj).CurrentRing = ring;
                    }
                }
            }
            return protmp;
        }

        public static int NewMagicProj(this Player player, Vector2 position, Vector2 velocity, int type, int ring = 0, float ai0 = 0, float ai1 = 0, float ai2 = 0)
        {
            int protmp = Projectile.NewProjectile(player.GetSource_FromThis("BG3Magic"), position, velocity, type, 0, 0, player.whoAmI, ai0, ai1, ai2);
            if (protmp >= 0 && protmp < 1000)
            {
                if (Main.projectile[protmp].ModProjectile != null)
                {
                    if (Main.projectile[protmp].ModProjectile is BaseMagicProj)
                    {
                        (Main.projectile[protmp].ModProjectile as BaseMagicProj).CurrentRing = ring;
                    }
                }
            }
            return protmp;
        }



        public static int GetRingDamage(int ring) => new List<int>() { CombatStat.Ring1Damage, CombatStat.Ring2Damage, CombatStat.Ring3Damage, CombatStat.Ring4Damage, CombatStat.Ring5Damage, CombatStat.Ring6Damage }[ring - 1];

        public static int GetCantripDamage(int lvl)
        {
            int dNum = 1;
            if (lvl >= 3) dNum = 2;
            if (lvl >= 6) dNum = 3;
            return (int)(Math.Pow(2, lvl) / dNum);
        }


        public static DiceDamage GetDiceDamage(this Player player, DiceDamage diceDamage, int originalRing, int currentRing, DiceDamage risingDiceAddition)
        {
            //TODO: 魔能爆/灼热射线升环不加伤
            if (player.TryGetModPlayer(out DNDMagicPlayer modPlayer))
            {
                DiceDamage result;
                if (originalRing == 0)          //戏法用戏法等级算
                {
                    result = diceDamage * GetCantripDamage(modPlayer.CantripLevel);
                    if (risingDiceAddition.GetDiceCount() == 0)
                    {
                        if (modPlayer.CantripLevel >= 6)
                        {
                            result *= 3;
                        }
                        else if (modPlayer.CantripLevel >= 3)
                        {
                            result *= 2;
                        }
                    }
                    else
                    {
                        if (modPlayer.CantripLevel >= 3)
                        {
                            result = result.GetRisingRingAddition(risingDiceAddition);
                        }
                        if (modPlayer.CantripLevel >= 6)
                        {
                            result = result.GetRisingRingAddition(risingDiceAddition);
                        }
                    }
                }
                else            //非戏法用升环差算
                {
                    result = diceDamage / GetRingDamage(originalRing) * GetRingDamage(currentRing);
                    for (int i = 0; i < currentRing - originalRing; i++)
                    {
                        result = result.GetRisingRingAddition(risingDiceAddition);
                    }
                }
                return result.GetDamageAddition(modPlayer.DamageAdditionA, modPlayer.DamageAdditionB, modPlayer.DamageAdditionC);
            }
            return diceDamage;
        }




        public static int GetDifficultyClass(this Player player, string spellName)
        {
            return EverythingLibrary.spells[spellName].DifficultyClass;
        }

        /// <summary>
        /// 仅用于展示，不能套到实际效果里
        /// </summary>
        /// <param name="player"></param>
        /// <param name="spellName"></param>
        /// <returns></returns>
        public static int GetSpellRange(this Player player, string spellName)
        {
            bool distant = player.GetModPlayer<DNDMagicPlayer>().RemainingSorceryPoint > 0 && player.GetModPlayer<DNDMagicPlayer>().DistantSpellMM;
            return EverythingLibrary.spells[spellName].SpellRange * (distant ? 2 : 1);
        }
        /// <summary>
        /// 仅用于展示，不能套到实际效果里
        /// </summary>
        /// <param name="player"></param>
        /// <param name="spellName"></param>
        /// <returns></returns>
        public static int GetAOERadius(this Player player, string spellName)
        {
            return EverythingLibrary.spells[spellName].AOERadius;
        }

        /// <summary>
        /// 仅用于展示，不能套到实际效果里
        /// </summary>
        /// <param name="player"></param>
        /// <param name="spellName"></param>
        /// <returns></returns>
        public static int GetTimeSpan(this Player player, string spellName)
        {
            bool extended = player.GetModPlayer<DNDMagicPlayer>().RemainingSorceryPoint > 0 && player.GetModPlayer<DNDMagicPlayer>().ExtendedSpellMM;
            return EverythingLibrary.spells[spellName].TimeSpan * (extended ? 2 : 1);
        }
        /// <summary>
        /// 仅用于展示，不能套到实际效果里
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static bool CarefulSpellMM(this Player player)
        {
            return player.GetModPlayer<DNDMagicPlayer>().CarefulSpellMM;
        }

        public static void CopyMetaMagicFrom(this BaseMagicProj proj1, BaseMagicProj proj2)
        {
            proj1.CarefulSpellMM = proj2.CarefulSpellMM;
            proj1.DistantSpellMM = proj2.DistantSpellMM;
            proj1.HeightenedSpellMM = proj2.HeightenedSpellMM;
            proj1.ExtendedSpellMM = proj2.ExtendedSpellMM;
            proj1.TwinnedSpellMM = proj2.TwinnedSpellMM;
        }

        public static void ActivateMetaMagic(this BaseMagicProj proj, Player player, bool careful, bool distant, bool extended, bool heightened, bool twinned)
        {
            DNDMagicPlayer modplayer = player.GetModPlayer<DNDMagicPlayer>();
            if (modplayer.MaxSorceryPoint - modplayer.ConsumedSorceryPoint > 0 && careful && modplayer.CarefulSpellMM)
            {
                proj.CarefulSpellMM = true;
                modplayer.ConsumedSorceryPoint++;
            }
            if (modplayer.MaxSorceryPoint - modplayer.ConsumedSorceryPoint > 0 && distant && modplayer.DistantSpellMM)
            {
                proj.DistantSpellMM = true;
                modplayer.ConsumedSorceryPoint++;
            }
            if (modplayer.MaxSorceryPoint - modplayer.ConsumedSorceryPoint > 0 && extended && modplayer.ExtendedSpellMM)
            {
                proj.ExtendedSpellMM = true;
                modplayer.ConsumedSorceryPoint++;
            }
            if (modplayer.MaxSorceryPoint - modplayer.ConsumedSorceryPoint > 0 && heightened && modplayer.HeightenedSpell)
            {
                proj.HeightenedSpellMM = true;
                modplayer.ConsumedSorceryPoint++;
            }
            if (modplayer.MaxSorceryPoint - modplayer.ConsumedSorceryPoint > 0 && twinned && modplayer.TwinnedSpellMM)
            {
                proj.TwinnedSpellMM = true;
                modplayer.ConsumedSorceryPoint++;
            }
        }

        public static (bool Careful, bool Distant, bool Extended, bool Heightened, bool Twinned) ActivateMetaMagic(this Player player, bool careful, bool distant, bool extended, bool heightened, bool twinned)
        {
            bool _careful = false;
            bool _distant = false;
            bool _extended = false;
            bool _heightened = false;
            bool _twinned = false;
            DNDMagicPlayer modplayer = player.GetModPlayer<DNDMagicPlayer>();
            if (modplayer.MaxSorceryPoint - modplayer.ConsumedSorceryPoint > 0 && careful && modplayer.CarefulSpellMM)
            {
                modplayer.ConsumedSorceryPoint++;
                _careful = true;
            }
            if (modplayer.MaxSorceryPoint - modplayer.ConsumedSorceryPoint > 0 && distant && modplayer.DistantSpellMM)
            {
                modplayer.ConsumedSorceryPoint++;
                _distant = true;
            }
            if (modplayer.MaxSorceryPoint - modplayer.ConsumedSorceryPoint > 0 && extended && modplayer.ExtendedSpellMM)
            {
                modplayer.ConsumedSorceryPoint++;
                _extended = true;
            }
            if (modplayer.MaxSorceryPoint - modplayer.ConsumedSorceryPoint > 0 && heightened && modplayer.HeightenedSpell)
            {
                modplayer.ConsumedSorceryPoint++;
                _heightened = true;
            }
            if (modplayer.MaxSorceryPoint - modplayer.ConsumedSorceryPoint > 0 && twinned && modplayer.TwinnedSpellMM)
            {
                modplayer.ConsumedSorceryPoint++;
                _twinned = true;
            }
            return (_careful, _distant, _extended, _heightened, _twinned);
        }

        public static bool HoldingMagic(this Player player)
        {
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.ModProjectile != null && proj.ModProjectile is BaseChannel)
                {
                    if (proj.owner == player.whoAmI) return true;
                }
            }
            return false;
        }

        public static void AddSpellSlot(this Player player, int slot, int count)
        {
            DNDMagicPlayer modplayer = player.GetModPlayer<DNDMagicPlayer>();
            if (!modplayer.MaxSpellSlot.TryAdd(slot, count))
            {
                modplayer.MaxSpellSlot[slot] += count;
            }
        }

        /// <summary>
        /// 获得某玩家的某种第一个弹幕
        /// </summary>
        /// <param name="player"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int GetProj(this Player player, int type)
        {
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.owner == player.whoAmI && proj.type == type)
                {
                    return proj.whoAmI;
                }
            }
            return -1;
        }

    }
}
