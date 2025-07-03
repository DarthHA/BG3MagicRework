using BG3MagicRework.BaseType;
using BG3MagicRework.Buffs;
using BG3MagicRework.Buffs.Enemy;
using BG3MagicRework.Projectiles.VirtualEffects;
using BG3MagicRework.Spells.Reaction;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace BG3MagicRework.Systems
{
    public partial class DNDMagicPlayer : ModPlayer
    {
        /// <summary>
        /// 当前法术位总量
        /// </summary>
        public Dictionary<int, int> MaxSpellSlot = new();
        /// <summary>
        /// 已消耗的法术位总量
        /// </summary>
        public Dictionary<int, int> ConsumedSpellSlot = new();

        /// <summary>
        /// 剩余法术位
        /// </summary>
        public Dictionary<int, int> RemainingSpellSlot = new();


        /// <summary>
        /// 戏法等级
        /// </summary>
        public int CantripLevel = 0;

        /// <summary>
        /// 最大超魔点
        /// </summary>
        public int MaxSorceryPoint = 0;

        /// <summary>
        /// 已消耗的超魔点
        /// </summary>
        public int ConsumedSorceryPoint = 0;

        /// <summary>
        /// 剩余超魔点
        /// </summary>
        public int RemainingSorceryPoint = 0;

        /// <summary>
        /// 最大专注位数
        /// </summary>
        public int MaxConcentrationCount = 1;

        /// <summary>
        /// 专注位
        /// </summary>
        public List<BaseConcentration> ConcentrationSlot = new();

        /// <summary>
        /// A类附伤，视为额外攻击，可触发B类附伤
        /// </summary>
        public DiceDamage DamageAdditionA = new();

        /// <summary>
        /// B类附伤，可由A类附伤触发
        /// </summary>
        public DiceDamage DamageAdditionB = new();

        /// <summary>
        /// C类附伤，只能触发一次
        /// </summary>
        public DiceDamage DamageAdditionC = new();

        /// <summary>
        /// 远距法术（距离翻倍）
        /// </summary>
        public bool DistantSpellMM = false;
        /// <summary>
        /// 延效法术（持续时间翻倍）
        /// </summary>
        public bool ExtendedSpellMM = false;
        /// <summary>
        /// 谨慎法术（法术穿墙）
        /// </summary>
        public bool CarefulSpellMM = false;
        /// <summary>
        /// 成双法术（伤害优势）
        /// </summary>
        public bool TwinnedSpellMM = false;
        /// <summary>
        /// 强效法术（豁免劣势）
        /// </summary>
        public bool HeightenedSpell = false;

        /// <summary>
        /// 法术位开始恢复的计时器
        /// </summary>
        public float SpellSlotRecoveryStartTimer = 0;

        /// <summary>
        /// 开始恢复法术位后的计时器
        /// </summary>
        public float SpellSlotRecoveryTimer = 0;

        /// <summary>
        /// 开始恢复法术位后的速度
        /// </summary>
        public float SpellSlotRecoveryRate = 1;

        /// <summary>
        /// 法术位开始恢复后的速度
        /// </summary>
        public float SpellSlotRecoveryStartRate = 1;


        /// <summary>
        /// 术法点恢复计时器
        /// </summary>
        public float SorceryPointRecoveryTimer = 0;

        /// <summary>
        /// 术法点恢复速度
        /// </summary>
        public float SorceryPointRecoveryRate = 1;

        /// <summary>
        /// 守卫刻纹效果
        /// </summary>
        public int GoWType = 0;

        /// <summary>
        /// 额外生命值，目前仅用于冰铠
        /// </summary>
        public int ExtraLife = 0;
        /// <summary>
        /// 冰铠效果等级
        /// </summary>
        public int AoALevel = 0;

        /// <summary>
        /// 法师护甲效果等级
        /// </summary>
        public int MageArmorLevel = 0;

        /// <summary>
        /// 镜像术幻象数量
        /// </summary>
        public int MirrorImageCount = 0;
        public override void PostUpdateMiscEffects()
        {
            if (Player.HasBuff(BuffID.ManaRegeneration))             //魔力回复药
            {
                SpellSlotRecoveryStartRate += CombatStat.ManaRegenPotionValue;
            }

            if (Player.HasBuff(BuffID.StarInBottle))             //星星瓶
            {
                SpellSlotRecoveryRate += CombatStat.StarInBottleValue;
            }

            if (Player.HasBuff(BuffID.MagicPower))         //魔能药
            {
                MaxSorceryPoint += CombatStat.MagicPowerValue;
            }

            //在这里写法术位增减效果
            if (Player.ConsumedManaCrystals >= 3)
            {
                Player.AddSpellSlot(1, 1);
            }
            if (Player.ConsumedManaCrystals >= 6)
            {
                Player.AddSpellSlot(1, 1);
            }
            if (Player.ConsumedManaCrystals >= 9)
            {
                Player.AddSpellSlot(2, 1);
            }
            if (Player.ConsumedManaCrystals >= 9 && Player.usedArcaneCrystal)
            {
                Player.AddSpellSlot(2, 1);
            }

            UpdateRecoverResources();

            //更新剩余法术位
            for (int i = 1; i <= 6; i++)
            {
                if (MaxSpellSlot.TryGetValue(i, out int count))
                {
                    if (ConsumedSpellSlot.TryGetValue(i, out int value))
                    {
                        count -= value;
                    }
                    if (count >= 0) RemainingSpellSlot.Add(i, count);
                }
            }

            //奥数饥渴惩罚
            bool Punishment = false;
            foreach (int lvl in ConsumedSpellSlot.Keys)
            {
                if (lvl == 0) continue;
                if (MaxSpellSlot.TryGetValue(lvl, out int value))
                {
                    if (ConsumedSpellSlot[lvl] > value)
                    {
                        Punishment = true;
                        break;
                    }
                }
                else
                {
                    Punishment = true;
                    break;
                }
            }
            if (ConsumedSorceryPoint > MaxSorceryPoint) Punishment = true;
            if (Punishment)
            {
                Player.AddBuff(ModContent.BuffType<ArcaneHungerBuff>(), 2);
            }

            UpdateMM();

            //在这里写专注修饰效果
            UpdateCon();

            UpdateReaction();

            if (Player.HasBuff(ModContent.BuffType<LongstriderBuff>()))
            {
                Player.accRunSpeed = Math.Max(6f, Player.accRunSpeed);
                Player.maxRunSpeed *= 1.75f;
                Player.accRunSpeed *= 1.75f;
                Player.runAcceleration *= 1.75f;
                Player.runSlowdown *= 1.75f;
            }

            if (Player.HasBuff(ModContent.BuffType<DisadvantageTerrainBuff>()))
            {
                Player.moveSpeed *= 0.5f;
                Player.jump /= 2;
            }

            if (ShieldCooldown > 0 && ShieldRingActive > 0)
            {
                Player.statDefense += 10;
            }

            if (Player.HasBuff(ModContent.BuffType<ArcaneHungerBuff>()))
            {
                Player.GetDamage(DamageClass.Generic) *= 0.1f;
                Player.GetDamage(DamageClass.Default) *= 0.1f;
                Player.accRunSpeed *= 0.5f;
                Player.maxRunSpeed *= 0.5f;
                Player.wingTimeMax = (int)(Player.wingTimeMax * 0.5f);
                Player.jumpSpeed *= 0.5f;
            }

            //法师护甲
            if (MageArmorLevel > 0)
            {
                Player.AddBuff(ModContent.BuffType<MageArmorBuff>(), 2);
                if (Player.statDefense < 20 * MageArmorLevel)
                {
                    Player.statDefense -= Player.statDefense;
                    Player.statDefense += 20 * MageArmorLevel;
                }
            }

            if (Player.HasBuff(ModContent.BuffType<MirrorImageBuff>()))
            {
                if (MirrorImageCount <= 0)
                {
                    Player.ClearBuff(ModContent.BuffType<MirrorImageBuff>());
                }
            }
            else
            {
                if (MirrorImageCount > 0)
                {
                    AdvancedCombatText.NewText(Main.LocalPlayer.getRect(), Color.White, Lang.GetBuffName(ModContent.BuffType<MirrorImageBuff>()), true);
                }
                MirrorImageCount = 0;
            }


            //判断是否给buff的
            if (ExtraLife > 0)
            {
                Player.statLifeMax2 += ExtraLife;
                if (AoALevel > 0)
                {
                    Player.AddBuff(ModContent.BuffType<ArmorOfAgathysBuff>(), 2);
                }
            }
            else
            {
                AoALevel = 0;
            }
        }

        public void UpdateRecoverResources()
        {
            //Boss存在时术法点和法术位恢复速度均变为原来的40%
            if (SomeUtils.AnyBossesExceptMiniboss())
            {
                SpellSlotRecoveryStartRate /= 2.5f;
                SpellSlotRecoveryRate /= 2.5f;
                SorceryPointRecoveryRate /= 2.5f;
            }

            //法术位经过12秒预备时间后，基础每3秒恢复一个，每高一级法术位多25%时间
            if (ConsumedSpellSlot.Count == 0 || Player.HoldingMagic())
            {
                SpellSlotRecoveryStartTimer = 0;
                SpellSlotRecoveryTimer = 0;
            }
            else
            {
                if (SpellSlotRecoveryStartTimer < CombatStat.StartRecoverSpellSlotTime)
                {
                    SpellSlotRecoveryStartTimer += SpellSlotRecoveryStartRate;
                }
                else
                {
                    float slotRecoverModifier = 1f;
                    //按照低到高恢复一个法术位
                    int minSlot = 10;
                    foreach (int lvl in ConsumedSpellSlot.Keys)
                    {
                        if (ConsumedSpellSlot[lvl] > 0)
                        {
                            if (lvl < minSlot)
                            {
                                minSlot = lvl;
                            }
                        }
                    }
                    if (minSlot != 10)
                    {
                        slotRecoverModifier = (float)Math.Pow(CombatStat.SlotRecoverModifier, minSlot - 1);
                    }
                    if (SpellSlotRecoveryTimer >= CombatStat.RecoverSpellSlotTime * slotRecoverModifier)
                    {
                        SpellSlotRecoveryTimer -= CombatStat.RecoverSpellSlotTime * slotRecoverModifier;
                        if (minSlot != 10)
                        {
                            ConsumedSpellSlot[minSlot]--;
                            if (ConsumedSpellSlot[minSlot] <= 0) ConsumedSpellSlot.Remove(minSlot);
                        }
                    }
                    else
                    {
                        SpellSlotRecoveryTimer += SpellSlotRecoveryRate;
                    }
                }
            }
            //术法点每6秒恢复一个，无其他限制

            if (ConsumedSorceryPoint <= 0)
            {
                SorceryPointRecoveryTimer = 0;
            }
            else
            {
                SorceryPointRecoveryTimer += SorceryPointRecoveryRate;
                if (SorceryPointRecoveryTimer >= CombatStat.RecoverSorceryPointTime)
                {
                    SorceryPointRecoveryTimer = 0;
                    ConsumedSorceryPoint--;
                }
            }
        }

        public void UpdateMM()
        {
            RemainingSorceryPoint = MaxSorceryPoint - ConsumedSorceryPoint;
            if (MaxSorceryPoint > 0)
            {
                Player.AddBuff(ModContent.BuffType<CarefulSpellMMBuff>(), 2);
                Player.AddBuff(ModContent.BuffType<DistantSpellMMBuff>(), 2);
                Player.AddBuff(ModContent.BuffType<ExtendedSpellMMBuff>(), 2);
                Player.AddBuff(ModContent.BuffType<HeightenedSpellMMBuff>(), 2);
                Player.AddBuff(ModContent.BuffType<TwinnedSpellMMBuff>(), 2);
            }
        }

        public void UpdateCon()
        {
            for (int i = ConcentrationSlot.Count - 1; i >= 0; i--)
            {
                ConcentrationSlot[i].TimeLeft--;
                if (ConcentrationSlot[i].TimeLeft <= 0)
                {
                    AdvancedCombatText.NewText(Player.getRect(), Color.White, ConcentrationSlot[i].GetName(), true);
                    if (ModContent.GetInstance<BG3Config>().ShowCombatInfo)
                    {
                        Main.NewText(string.Format(LangLibrary.XEndConX, Player.name, ConcentrationSlot[i].GetName(false, false)));
                    }
                    ConcentrationSlot.RemoveAt(i);
                }
            }
            //被控住时失去所有专注
            if (Player.CCed)
            {
                while (ConcentrationSlot.Count > 0)
                {
                    AdvancedCombatText.NewText(Player.getRect(), Color.White, ConcentrationSlot[0].GetName(), true);
                    if (ModContent.GetInstance<BG3Config>().ShowCombatInfo)
                    {
                        Main.NewText(string.Format(LangLibrary.XEndConX, Player.name, ConcentrationSlot[0].GetName(false, false)));
                    }
                    ConcentrationSlot.RemoveAt(0);
                }
            }
            //专注位不足时按照从早到晚删除多余专注
            if (ConcentrationSlot.Count > MaxConcentrationCount)
            {
                int diff = ConcentrationSlot.Count - MaxConcentrationCount;
                for (int i = 0; i < diff; i++)
                {
                    AdvancedCombatText.NewText(Player.getRect(), Color.White, ConcentrationSlot[0].GetName(), true);
                    if (ModContent.GetInstance<BG3Config>().ShowCombatInfo)
                    {
                        Main.NewText(string.Format(LangLibrary.XEndConX, Player.name, ConcentrationSlot[0].GetName(false, false)));
                    }
                    ConcentrationSlot.RemoveAt(0);
                }
            }
            //更新专注效果与时间
            for (int i = ConcentrationSlot.Count - 1; i >= 0; i--)
            {
                if (!ConcentrationSlot[i].UpdateAndDecide(Player))
                {
                    AdvancedCombatText.NewText(Player.getRect(), Color.White, ConcentrationSlot[i].GetName(), true);
                    if (ModContent.GetInstance<BG3Config>().ShowCombatInfo)
                    {
                        Main.NewText(string.Format(LangLibrary.XEndConX, Player.name, ConcentrationSlot[i].GetName(false, false)));
                    }
                    ConcentrationSlot.RemoveAt(i);
                }
            }

            for (int i = ConcentrationSlot.Count - 1; i >= 0; i--)
            {
                ConcentrationSlot[i].TimeLeft--;
                if (ConcentrationSlot[i].TimeLeft <= 0)
                {
                    AdvancedCombatText.NewText(Player.getRect(), Color.White, ConcentrationSlot[i].GetName(), true);
                    if (ModContent.GetInstance<BG3Config>().ShowCombatInfo)
                    {
                        Main.NewText(string.Format(LangLibrary.XEndConX, Player.name, ConcentrationSlot[i].GetName(false, false)));
                    }
                    ConcentrationSlot.RemoveAt(i);
                }
            }
            //神秘小代码
            if (ConcentrationSlot.Count >= 1)
            {
                Player.AddBuff(ModContent.BuffType<ConSlotBuff0>(), ConcentrationSlot[0].TimeLeft);
            }
            else
            {
                Player.ClearBuff(ModContent.BuffType<ConSlotBuff0>());
            }
            if (ConcentrationSlot.Count >= 2)
            {
                Player.AddBuff(ModContent.BuffType<ConSlotBuff1>(), ConcentrationSlot[1].TimeLeft);
            }
            else
            {
                Player.ClearBuff(ModContent.BuffType<ConSlotBuff1>());
            }
            if (ConcentrationSlot.Count >= 3)
            {
                Player.AddBuff(ModContent.BuffType<ConSlotBuff2>(), ConcentrationSlot[2].TimeLeft);
            }
            else
            {
                Player.ClearBuff(ModContent.BuffType<ConSlotBuff2>());
            }
            if (ConcentrationSlot.Count >= 4)
            {
                Player.AddBuff(ModContent.BuffType<ConSlotBuff3>(), ConcentrationSlot[3].TimeLeft);
            }
            else
            {
                Player.ClearBuff(ModContent.BuffType<ConSlotBuff3>());
            }
            if (ConcentrationSlot.Count >= 5)
            {
                Player.AddBuff(ModContent.BuffType<ConSlotBuff4>(), ConcentrationSlot[4].TimeLeft);
            }
            else
            {
                Player.ClearBuff(ModContent.BuffType<ConSlotBuff4>());
            }
            if (ConcentrationSlot.Count >= 6)
            {
                Player.AddBuff(ModContent.BuffType<ConSlotBuff5>(), ConcentrationSlot[5].TimeLeft);
            }
            else
            {
                Player.ClearBuff(ModContent.BuffType<ConSlotBuff5>());
            }
            if (ConcentrationSlot.Count >= 7)
            {
                Player.AddBuff(ModContent.BuffType<ConSlotBuff6>(), ConcentrationSlot[6].TimeLeft);
            }
            else
            {
                Player.ClearBuff(ModContent.BuffType<ConSlotBuff6>());
            }
            if (ConcentrationSlot.Count >= 8)
            {
                Player.AddBuff(ModContent.BuffType<ConSlotBuff7>(), ConcentrationSlot[7].TimeLeft);
            }
            else
            {
                Player.ClearBuff(ModContent.BuffType<ConSlotBuff7>());
            }
            if (ConcentrationSlot.Count >= 9)
            {
                Player.AddBuff(ModContent.BuffType<ConSlotBuff8>(), ConcentrationSlot[8].TimeLeft);
            }
            else
            {
                Player.ClearBuff(ModContent.BuffType<ConSlotBuff8>());
            }
            if (ConcentrationSlot.Count >= 10)
            {
                Player.AddBuff(ModContent.BuffType<ConSlotBuff9>(), ConcentrationSlot[9].TimeLeft);
            }
            else
            {
                Player.ClearBuff(ModContent.BuffType<ConSlotBuff9>());
            }
        }

        public void UpdateReaction()
        {
            if (Player.HasReaction<HellishRebukeSpell>())
            {
                Player.AddBuff(ModContent.BuffType<HRTrigger>(), 2);
            }
            if (HellishRebukeCooldown > 0) HellishRebukeCooldown--;
            if (Player.HasReaction<ShieldSpell>())
            {
                Player.AddBuff(ModContent.BuffType<ShieldTrigger>(), 2);
            }
            if (ShieldCooldown > 0) ShieldCooldown--; else ShieldRingActive = 0;
            if (Player.HasReaction<LegionOfBeesSpell>())
            {
                Player.AddBuff(ModContent.BuffType<BeeTrigger>(), 2);
            }
            if (BeeTimer > 0) BeeTimer--;
        }

        public override void ResetEffects()
        {
            DamageAdditionA = new();
            DamageAdditionB = new();
            DamageAdditionC = new();
            MaxSpellSlot.Clear();
            RemainingSpellSlot.Clear();
            CantripLevel = 0;
            MaxSorceryPoint = 0;
            MaxConcentrationCount = 1;
            SpellSlotRecoveryRate = 1;
            SpellSlotRecoveryStartRate = 1;
            SorceryPointRecoveryRate = 1;
        }

        public override void UpdateDead()
        {
            DamageAdditionA = new();
            DamageAdditionB = new();
            DamageAdditionC = new();
            MaxSpellSlot.Clear();
            RemainingSpellSlot.Clear();
            ConsumedSpellSlot.Clear();
            CantripLevel = 0;
            MaxSorceryPoint = 0;
            ConsumedSorceryPoint = 0;
            RemainingSorceryPoint = 0;
            MaxConcentrationCount = 1;
            HellishRebukeCooldown = 0;
            ShieldCooldown = 0;
            BeeTimer = 0;
            ConcentrationSlot.Clear();
            SpellSlotRecoveryStartTimer = 0;
            SpellSlotRecoveryTimer = 0;
            SorceryPointRecoveryTimer = 0;
            SpellSlotRecoveryRate = 1;
            SpellSlotRecoveryStartRate = 1;
            SorceryPointRecoveryRate = 1;

            ExtraLife = 0;
            AoALevel = 0;
            MageArmorLevel = 0;
            MirrorImageCount = 0;
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add("ShieldTrigger", ShieldTrigger);
            tag.Add("HellishRebukeTrigger", HellishRebukeTrigger);
            tag.Add("BeeTrigger", BeeTrigger);
            tag.Add("ConsumedSpellSlotKeys", ConsumedSpellSlot.Keys.ToList());
            tag.Add("ConsumedSpellSlotValues", ConsumedSpellSlot.Values.ToList());
            tag.Add("ConsumedSorceryPoint", ConsumedSorceryPoint);
            tag.Add("ExtraLife", ExtraLife);
            tag.Add("AoALevel", AoALevel);
            tag.Add("MageArmorLevel", MageArmorLevel);
        }

        public override void LoadData(TagCompound tag)
        {
            ShieldTrigger = tag.GetBool("ShieldTrigger");
            HellishRebukeTrigger = tag.GetBool("HellishRebukeTrigger");
            BeeTrigger = tag.GetBool("BeeTrigger");
            List<int> consumedSpellSlotKeys = tag.Get<List<int>>("ConsumedSpellSlotKeys");
            List<int> consumedSpellSlotValues = tag.Get<List<int>>("ConsumedSpellSlotValues");
            ConsumedSpellSlot = consumedSpellSlotKeys.Zip(consumedSpellSlotValues, (k, v) => new { Key = k, Value = v }).ToDictionary(x => x.Key, x => x.Value);
            ConsumedSorceryPoint = tag.GetInt("ConsumedSorceryPoint");
            ExtraLife = tag.GetInt("ExtraLife");
            AoALevel = tag.GetInt("AoALevel");
            MageArmorLevel = tag.GetInt("MageArmorLevel");
        }
    }

}