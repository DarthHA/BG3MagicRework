using BG3MagicRework.BaseType;
using BG3MagicRework.Buffs.Enemy;
using BG3MagicRework.Projectiles.VirtualEffects;
using BG3MagicRework.Systems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Static
{
    public static class CCEffectUtils
    {
        public static int GetEnemySavingRoll(this NPC npc)        //敌方豁免加成，血量10对数底
        {
            if (npc.realLife == -1)
            {
                return (int)Math.Log10(npc.lifeMax);
            }
            else
            {
                if (Main.npc[npc.realLife].active)
                {
                    return (int)Math.Log10(Main.npc[npc.realLife].lifeMax);
                }
                else
                {
                    return (int)Math.Log10(npc.lifeMax);
                }
            }
        }

        public static bool CheckEnemyFail(NPC target, int DC, bool Disadvantage = false)       //我方DC基础12
        {
            if (target.immortal) return true;
            int sr = target.GetEnemySavingRoll();
            float roll = Main.rand.Next(20) + 1;
            if (Disadvantage)
            {
                roll = Math.Min(roll, Main.rand.Next(20) + 1);
            }
            if (roll == 20)  //大成功
            {
                roll += 99;
            }
            else if (roll > 1)  //非大失败
            {
                roll += sr;
            }
            return DC >= roll;
        }

        public static bool DeepAddCCBuffByDC(this BaseMagicProj modproj, NPC target, int buffType, int buffTime)
        {
            bool success = CheckEnemyFail(target, modproj.DifficultyClass, modproj.HeightenedSpellMM || target.HasBuff(ModContent.BuffType<PoisonedDNDBuff>()) || target.HasBuff(ModContent.BuffType<FearDNDBuff>()));
            if (success)
            {
                target.DeepAddCCBuff(buffType, buffTime);
                if (ModContent.GetInstance<BG3Config>().ShowCombatInfo)
                {
                    Main.NewText(string.Format(LangLibrary.XNotPassedXsSavingThrowDCX,
                       Lang.GetNPCNameValue(target.type),
                       Lang.GetProjectileName(modproj.Projectile.type).Value,
                       modproj.DifficultyClass
                       ));
                }
            }
            else
            {
                if (ModContent.GetInstance<BG3Config>().ShowCombatInfo)
                {
                    Main.NewText(string.Format(LangLibrary.XPassedXsSavingThrowDCX,
                       Lang.GetNPCNameValue(target.type),
                       Lang.GetProjectileName(modproj.Projectile.type).Value,
                       modproj.DifficultyClass
                       ));
                }
            }
            return success;
        }

        public static void AddNormalBuff(this NPC target, int buffType, int buffTime)
        {
            target.buffImmune[buffType] = false;
            if ((!target.HasBuff(buffType) || buffTime > 2) && buffType != ModContent.BuffType<DisadvantageTerrainBuff>())
            {
                AdvancedCombatText.NewText(target.getRect(), Color.White, Lang.GetBuffName(buffType));
            }
            target.AddBuff(buffType, buffTime);
            if (ModContent.GetInstance<BG3Config>().ShowCombatInfo)
            {
                if (buffTime >= 60)
                {
                    Main.NewText(string.Format(LangLibrary.XAddedXSecXBuff,
                        Lang.GetNPCNameValue(target.type),
                        buffTime / 60,
                        Lang.GetBuffName(buffType)
                        ));
                }
            }
        }

        public static void DeepAddCCBuff(this NPC target, int buffType, int buffTime)
        {
            target.buffImmune[buffType] = false;
            if ((!target.HasBuff(buffType) || buffTime > 2) && buffType != ModContent.BuffType<DisadvantageTerrainBuff>())
            {
                AdvancedCombatText.NewText(target.getRect(), Color.White, Lang.GetBuffName(buffType));
            }
            target.AddBuff(buffType, buffTime);
            if (target.GetInstantSource() == -1)          //不存在源头
            {
                if (ModContent.GetInstance<BG3Config>().ShowCombatInfo)
                {
                    if (buffTime >= 60)
                    {
                        Main.NewText(string.Format(LangLibrary.XAddedXSecXBuff,
                            Lang.GetNPCNameValue(target.type),
                            buffTime / 60,
                            Lang.GetBuffName(buffType)
                            ));
                    }
                }
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.whoAmI != target.whoAmI)
                    {
                        if (npc.GetInstantSource() == target.whoAmI)
                        {
                            npc.buffImmune[buffType] = false;
                            npc.AddBuff(buffType, buffTime);
                        }
                    }
                }
            }
            else       //存在源头
            {
                if (ModContent.GetInstance<BG3Config>().ShowCombatInfo)
                {
                    if (buffTime >= 60)
                    {
                        Main.NewText(string.Format(LangLibrary.XAddedXSecXBuff,
                            Lang.GetNPCNameValue(Main.npc[target.GetInstantSource()].type),
                            buffTime / 60,
                            Lang.GetBuffName(buffType)
                            ));
                    }
                }
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.whoAmI != target.whoAmI)
                    {
                        if (npc.whoAmI == target.GetInstantSource() || npc.GetInstantSource() == target.whoAmI || npc.GetInstantSource() == target.GetInstantSource())
                        {
                            npc.buffImmune[buffType] = false;
                            npc.AddBuff(buffType, buffTime);
                        }
                    }
                }
            }
        }


        public static void ClearBuff(this NPC target, int buffType)
        {
            if (target.FindBuffIndex(buffType) != -1)
            {
                target.DelBuff(target.FindBuffIndex(buffType));
            }
        }


        public static void DeepClearCCBuff(this NPC target, int buffType)
        {
            if (target.HasBuff(buffType))
            {
                AdvancedCombatText.NewText(target.getRect(), Color.White, Lang.GetBuffName(buffType), true);
            }
            target.ClearBuff(buffType);
            if (target.GetInstantSource() == -1)          //不存在源头
            {
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.whoAmI != target.whoAmI)
                    {
                        if (npc.GetInstantSource() == target.whoAmI)
                        {
                            npc.ClearBuff(buffType);
                        }
                    }
                }
            }
            else       //存在源头
            {
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.whoAmI != target.whoAmI)
                    {
                        if (npc.whoAmI == target.GetInstantSource() || npc.GetInstantSource() == target.whoAmI || npc.GetInstantSource() == target.GetInstantSource())
                        {
                            npc.ClearBuff(buffType);
                        }
                    }
                }
            }
        }

        public static List<int> FindMultipleEnemyCC(Vector2 Center, int radius, int count = 1, bool SeeThroughTiles = true)
        {
            List<int> selectedNPC = new();
            void GetNPC(NPC mainNpc)
            {
                if (!selectedNPC.Contains(mainNpc.whoAmI)) selectedNPC.Add(mainNpc.whoAmI);
                if (mainNpc.GetInstantSource() == -1)          //不存在源头
                {
                    foreach (NPC npc in Main.ActiveNPCs)
                    {
                        if (npc.whoAmI != mainNpc.whoAmI)
                        {
                            if (npc.GetInstantSource() == mainNpc.whoAmI)
                            {
                                if (!selectedNPC.Contains(npc.whoAmI)) selectedNPC.Add(npc.whoAmI);
                            }
                        }
                    }
                }
                else       //存在源头
                {
                    foreach (NPC npc in Main.ActiveNPCs)
                    {
                        if (npc.whoAmI != mainNpc.whoAmI)
                        {
                            if (npc.whoAmI == mainNpc.GetInstantSource() || npc.GetInstantSource() == mainNpc.whoAmI || npc.GetInstantSource() == mainNpc.GetInstantSource())
                            {
                                if (!selectedNPC.Contains(npc.whoAmI)) selectedNPC.Add(npc.whoAmI);
                            }
                        }
                    }
                }
            }

            List<int> resultAll = new();
            while (true)
            {
                int resultOne = -1;
                float distance = -1;
                foreach (NPC npc in Main.npc)
                {
                    if (!selectedNPC.Contains(npc.whoAmI))
                    {
                        if (npc.Distance(Center) <= radius)
                        {
                            if (npc.CanBeChasedBy(null, true) || npc.immortal)
                            {
                                if (SeeThroughTiles || Collision.CanHit(Center, 1, 1, npc.position, npc.width, npc.height))
                                {
                                    if (resultOne == -1 || npc.Distance(Center) < distance)
                                    {
                                        resultOne = npc.whoAmI;
                                        distance = npc.Distance(Center);
                                    }
                                }
                            }
                        }
                    }
                }
                if (resultOne == -1)
                {
                    break;
                }
                else
                {
                    resultAll.Add(resultOne);
                    GetNPC(Main.npc[resultOne]);
                }
                if (resultAll.Count >= count) break;
            }
            return resultAll;
        }



        public delegate void AddStatusBuff(NPC target, int time);

        public static void DeepAddStatusBuff(NPC MainTarget, int time, AddStatusBuff addmethod)
        {
            addmethod(MainTarget, time);
            //根据共享血条原则定位
            if (MainTarget.realLife == -1)      //独立单位
            {
                foreach (NPC npc in Main.ActiveNPCs)       //找身体
                {
                    if (npc.realLife == MainTarget.whoAmI && npc.whoAmI != MainTarget.whoAmI)
                    {
                        addmethod(npc, time);
                    }
                }
            }
            else             //身体
            {
                if (Main.npc[MainTarget.realLife].active && MainTarget.realLife != MainTarget.whoAmI)
                {
                    addmethod(Main.npc[MainTarget.realLife], time);  //给头
                }
                foreach (NPC npc in Main.ActiveNPCs)   //找别的身体
                {
                    if (npc.whoAmI != MainTarget.whoAmI && MainTarget.realLife == npc.realLife)
                    {
                        addmethod(npc, time);
                    }
                }
            }
            //根据同帧生成原则定位
            if (MainTarget.GetInstantSource() == -1)          //不存在源头
            {
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.whoAmI != MainTarget.whoAmI)
                    {
                        if (npc.GetInstantSource() == MainTarget.whoAmI)
                        {
                            addmethod(npc, time);
                        }
                    }
                }
            }
            else       //存在源头
            {
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.whoAmI != MainTarget.whoAmI)
                    {
                        if (npc.whoAmI == MainTarget.GetInstantSource() || npc.GetInstantSource() == MainTarget.whoAmI || npc.GetInstantSource() == MainTarget.GetInstantSource())
                        {
                            addmethod(npc, time);
                        }
                    }
                }
            }

        }

        public static void addBurning(NPC target, int time)
        {
            EnemyStatusSystem gnpc = target.GetGlobalNPC<EnemyStatusSystem>();
            switch (gnpc.FireAndColdStatus)
            {
                case FireAndColdStatusID.None:          //施加燃烧
                    gnpc.FireAndColdStatus = FireAndColdStatusID.Burning;
                    gnpc.FireAndColdStatusTime = time;
                    break;
                case FireAndColdStatusID.Burning:       //刷新燃烧时长
                    if (gnpc.FireAndColdStatusTime < time)
                    {
                        gnpc.FireAndColdStatusTime = time;
                    }
                    break;
                case FireAndColdStatusID.Wet:          //无效
                    break;
                case FireAndColdStatusID.Chilled:       //解除冻僵
                    gnpc.FireAndColdStatusTime = 0;
                    break;
                case FireAndColdStatusID.Frozen:        //无效
                    break;
            }
        }

        public static void addWet(NPC target, int time)
        {
            EnemyStatusSystem gnpc = target.GetGlobalNPC<EnemyStatusSystem>();
            switch (gnpc.FireAndColdStatus)
            {
                case FireAndColdStatusID.None:          //施加潮湿
                    gnpc.FireAndColdStatus = FireAndColdStatusID.Wet;
                    gnpc.FireAndColdStatusTime = time;
                    break;
                case FireAndColdStatusID.Burning:       //解除燃烧并施加潮湿
                    gnpc.FireAndColdStatus = FireAndColdStatusID.Wet;
                    gnpc.FireAndColdStatusTime = time;
                    break;
                case FireAndColdStatusID.Wet:          //刷新时长
                    if (gnpc.FireAndColdStatusTime < time)
                    {
                        gnpc.FireAndColdStatusTime = time;
                    }
                    break;
                case FireAndColdStatusID.Chilled:       //施加冰冻
                    gnpc.FireAndColdStatus = FireAndColdStatusID.Frozen;
                    gnpc.FireAndColdStatusTime = CombatStat.FrozenTime;         //时间还得再改
                    break;
                case FireAndColdStatusID.Frozen:        //无效
                    break;
            }
        }
        public static void addChilled(NPC target, int time)
        {
            EnemyStatusSystem gnpc = target.GetGlobalNPC<EnemyStatusSystem>();
            switch (gnpc.FireAndColdStatus)
            {
                case FireAndColdStatusID.None:          //施加冻僵
                    gnpc.FireAndColdStatus = FireAndColdStatusID.Chilled;
                    gnpc.FireAndColdStatusTime = time;
                    break;
                case FireAndColdStatusID.Burning:       //解除燃烧并施加冻僵
                    gnpc.FireAndColdStatus = FireAndColdStatusID.Chilled;
                    gnpc.FireAndColdStatusTime = time;
                    break;
                case FireAndColdStatusID.Wet:          //解除潮湿并施加冻僵
                    gnpc.FireAndColdStatus = FireAndColdStatusID.Chilled;
                    gnpc.FireAndColdStatusTime = time;
                    break;
                case FireAndColdStatusID.Chilled:          //刷新时长
                    if (gnpc.FireAndColdStatusTime < time)
                    {
                        gnpc.FireAndColdStatusTime = time;
                    }
                    break;
                case FireAndColdStatusID.Frozen:        //无效
                    break;
            }
        }
        public static void clearWetAndChilled(NPC target, int time)
        {
            EnemyStatusSystem gnpc = target.GetGlobalNPC<EnemyStatusSystem>();
            switch (gnpc.FireAndColdStatus)
            {
                case FireAndColdStatusID.Wet:
                case FireAndColdStatusID.Chilled:
                    gnpc.FireAndColdStatusTime = 0;
                    break;
                default:
                    break;
            }
        }
        public static void clearFrozen(NPC target, int time)
        {
            EnemyStatusSystem gnpc = target.GetGlobalNPC<EnemyStatusSystem>();
            switch (gnpc.FireAndColdStatus)
            {
                case FireAndColdStatusID.Frozen:
                    gnpc.FireAndColdStatusTime = 0;
                    break;
                default:
                    break;
            }
        }

        public static void DeepAddBurning(this NPC target, int time, bool broadcast = true)
        {
            EnemyStatusSystem gnpc = target.GetGlobalNPC<EnemyStatusSystem>();
            if (gnpc.FireAndColdStatus == FireAndColdStatusID.None)
            {
                AdvancedCombatText.NewText(target.getRect(), Color.White, Lang.GetBuffName(ModContent.BuffType<BurningDNDBuff>()));
                if (ModContent.GetInstance<BG3Config>().ShowCombatInfo && broadcast)
                {
                    Main.NewText(string.Format(LangLibrary.XAddedXSecXBuff,
                        Lang.GetNPCNameValue(target.type),
                        time / 60,
                        Lang.GetBuffName(ModContent.BuffType<BurningDNDBuff>())
                        ));
                }
            }
            DeepAddStatusBuff(target, time, new AddStatusBuff(addBurning));
        }

        public static void DeepAddWet(this NPC target, int time, bool broadcast = true)
        {
            EnemyStatusSystem gnpc = target.GetGlobalNPC<EnemyStatusSystem>();
            if (gnpc.FireAndColdStatus == FireAndColdStatusID.None ||
                gnpc.FireAndColdStatus == FireAndColdStatusID.Burning)
            {
                AdvancedCombatText.NewText(target.getRect(), Color.White, Lang.GetBuffName(ModContent.BuffType<WetDNDBuff>()));
                if (ModContent.GetInstance<BG3Config>().ShowCombatInfo && broadcast)
                {
                    Main.NewText(string.Format(LangLibrary.XAddedXSecXBuff,
                        Lang.GetNPCNameValue(target.type),
                        time / 60,
                        Lang.GetBuffName(ModContent.BuffType<WetDNDBuff>())
                        ));
                }
            }
            else if (gnpc.FireAndColdStatus == FireAndColdStatusID.Chilled)
            {
                AdvancedCombatText.NewText(target.getRect(), Color.White, Lang.GetBuffName(ModContent.BuffType<FrozenDNDBuff>()));
                if (ModContent.GetInstance<BG3Config>().ShowCombatInfo && broadcast)
                {
                    Main.NewText(string.Format(LangLibrary.XAddedXSecXBuff,
                        Lang.GetNPCNameValue(target.type),
                        time / 60,
                        Lang.GetBuffName(ModContent.BuffType<FrozenDNDBuff>())
                        ));
                }
            }
            DeepAddStatusBuff(target, time, new AddStatusBuff(addWet));
        }

        public static void DeepAddChilled(this NPC target, int time, bool broadcast = true)
        {
            EnemyStatusSystem gnpc = target.GetGlobalNPC<EnemyStatusSystem>();
            if (gnpc.FireAndColdStatus == FireAndColdStatusID.None ||
                gnpc.FireAndColdStatus == FireAndColdStatusID.Burning ||
                gnpc.FireAndColdStatus == FireAndColdStatusID.Wet)
            {
                AdvancedCombatText.NewText(target.getRect(), Color.White, Lang.GetBuffName(ModContent.BuffType<ChilledDNDBuff>()));
                if (ModContent.GetInstance<BG3Config>().ShowCombatInfo && broadcast)
                {
                    Main.NewText(string.Format(LangLibrary.XAddedXSecXBuff,
                        Lang.GetNPCNameValue(target.type),
                        time / 60,
                        Lang.GetBuffName(ModContent.BuffType<ChilledDNDBuff>())
                        ));
                }
            }
            DeepAddStatusBuff(target, time, new AddStatusBuff(addChilled));
        }

        public static void DeepClearWetAndChilled(this NPC target, int time)
        {
            DeepAddStatusBuff(target, time, new AddStatusBuff(clearWetAndChilled));
        }

        public static void DeepClearFrozen(this NPC target, int time)
        {
            DeepAddStatusBuff(target, time, new AddStatusBuff(clearFrozen));
        }
    }
}
