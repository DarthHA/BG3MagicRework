using BG3MagicRework.BaseType;
using BG3MagicRework.Buffs.Enemy;
using BG3MagicRework.Systems;
using Microsoft.Xna.Framework;
using MonoMod.Utils;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Static
{
    public static class SomeUtils
    {
        /// <summary>
        /// 把123456改为罗马数字
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string RomanNumber(int num)
        {
            string s = "";
            switch (num)
            {
                case 1:
                    s = "I";
                    break;
                case 2:
                    s = "II";
                    break;
                case 3:
                    s = "III";
                    break;
                case 4:
                    s = "IV";
                    break;
                case 5:
                    s = "V";
                    break;
                case 6:
                    s = "VI";
                    break;
            }
            return s;
        }

        public static bool IsBoss(this NPC npc)
        {
            if (npc.realLife != -1)
            {
                if (Main.npc[npc.realLife].active)
                {
                    return Main.npc[npc.realLife].boss || NPCID.Sets.ShouldBeCountedAsBoss[Main.npc[npc.realLife].type];
                }
            }
            else
            {
                return npc.boss || NPCID.Sets.ShouldBeCountedAsBoss[npc.type];
            }
            return false;
        }

        public static bool AnyBossesExceptMiniboss()
        {
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.IsBoss() &&
                    npc.type != NPCID.LunarTowerSolar &&
                    npc.type != NPCID.LunarTowerVortex &&
                    npc.type != NPCID.LunarTowerNebula &&
                    npc.type != NPCID.LunarTowerStardust &&
                    npc.type != NPCID.MartianSaucer &&
                    npc.type != NPCID.MartianSaucerCore)
                {
                    return true;
                }
            }
            return false;
        }

        public static void AddLightLine(Vector2 Begin, Vector2 End, Color color, float intensity = 1)
        {
            if (End == Begin)
            {
                AddLight(Begin, color, intensity);
                return;
            }
            float Length = End.Distance(Begin);
            Vector2 UnitX = Vector2.Normalize(End - Begin);
            int l = 0;
            do
            {
                Vector2 Pos = Begin + UnitX * l;
                AddLight(Pos, color, intensity);
                l += 16;
            } while (l < Length);
            AddLight(End, color, intensity);
        }

        public static void AddLight(Vector2 Pos, Color color, float intensity = 1)
        {
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;
            Lighting.AddLight(Pos, r * intensity, g * intensity, b * intensity);
        }

        public static void AddLightCircle(Vector2 Center, float radius, Color color, float intensity = 1)
        {
            for (float i = Center.X - radius; i < Center.X + radius; i += 16)
            {
                for (float j = Center.Y - radius; j < Center.Y + radius; j += 16)
                {
                    if (new Vector2(i, j).Distance(Center) <= radius + 8)
                    {
                        AddLight(new Vector2(i, j), color, intensity);
                    }
                }
            }
        }

        public static void LazyRemakeMagicItem(this Item item, bool Cantrips = false)
        {
            item.damage = 0;
            item.DamageType = DamageClass.Magic;
            item.knockBack = 0;
            item.crit = 0;
            item.mana = 0;
            item.useTime = 20;
            item.useAnimation = 20;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useStyle = ItemUseStyleID.Rapier;
            item.useTurn = false;
            item.useTurnOnAnimationStart = true;
            item.autoReuse = Cantrips;
            item.channel = false;
        }

        public static void LazyRemakeReactionItem(this Item item)
        {
            item.damage = 0;
            item.knockBack = 0;
            item.crit = 0;
            item.mana = 0;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.None;
            item.useTime = 0;
            item.useAnimation = 0;
            item.channel = false;
        }

        public static int FindEnemyByOwner(Vector2 AttackerCenter, Vector2 OwnerCenter, float radius, bool SeeThroughTiles = true)
        {
            int result = -1;
            float distance = -1;
            foreach (NPC npc in Main.npc)
            {
                if (npc.Distance(OwnerCenter) <= radius)
                {
                    if (npc.CanBeChasedBy() || npc.immortal)
                    {
                        if (SeeThroughTiles || Collision.CanHit(OwnerCenter, 1, 1, npc.position, npc.width, npc.height))
                        {
                            if (result == -1 || npc.Distance(AttackerCenter) < distance)
                            {
                                result = npc.whoAmI;
                                distance = npc.Distance(AttackerCenter);
                            }
                        }
                    }
                }
            }
            return result;
        }

        public static int FindEnemyBySelf(Vector2 Center, int radius, bool SeeThroughTiles = true)
        {
            int result = -1;
            float distance = -1;
            foreach (NPC npc in Main.npc)
            {
                if (npc.Distance(Center) <= radius)
                {
                    if (npc.CanBeChasedBy() || npc.immortal)
                    {
                        if (SeeThroughTiles || Collision.CanHit(Center, 1, 1, npc.position, npc.width, npc.height))
                        {
                            if (result == -1 || npc.Distance(Center) < distance)
                            {
                                result = npc.whoAmI;
                                distance = npc.Distance(Center);
                            }
                        }
                    }
                }
            }
            return result;
        }

        public static long GenerateUUID()
        {
            return Main.rand.Next(0, 1000000);
        }

        public static void AddColorString(ref string text, Color color)
        {
            string r = color.R.ToString("x");
            string g = color.G.ToString("x");
            string b = color.B.ToString("x");
            if (r.Length == 1) r = "0" + r;
            if (g.Length == 1) g = "0" + g;
            if (b.Length == 1) b = "0" + b;
            text = "[c/" + r + g + b + ":" + text + "]";
        }

        /// <summary>
        /// 不受物块阻挡的有最远距离的坐标
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static Vector2 GetNoBlockEndPos(Vector2 start, Vector2 end, float maxLength)
        {
            if (start.Distance(end) < maxLength) return end;
            return start + Vector2.Normalize(end - start) * maxLength;
        }

        /// <summary>
        /// 受物块阻挡的坐标（可以有最远距离）
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static Vector2 GetTileBlockedEndPos(Vector2 start, Vector2 end, float maxLength = -1)
        {
            if (maxLength != -1)
            {
                if (start.Distance(end) > maxLength)
                {
                    end = start + Vector2.Normalize(end - start) * maxLength;
                }
            }

            float maxLen = end.Distance(start);
            if (maxLen <= 16) return end;
            float currentLen;
            float result = maxLen;
            for (currentLen = 16; currentLen <= maxLen; currentLen += 16)
            {
                Vector2 pos = start + Vector2.Normalize(end - start) * currentLen;
                if (!Collision.CanHit(start, 1, 1, pos, 1, 1))
                {
                    result = currentLen - 16;
                    break;
                }
            }
            return start + Vector2.Normalize(end - start) * result;
        }

        public static Color GetColor(this DamageElement element)
        {
            switch (element)
            {
                case DamageElement.None:
                    return Color.Gray;
                case DamageElement.Acid:
                    return Color.GreenYellow;
                case DamageElement.Cold:
                    return Color.Cyan;
                case DamageElement.Fire:
                    return Color.Orange;
                case DamageElement.Force:
                    return Color.DarkRed;
                case DamageElement.Lightning:
                    return Color.DeepSkyBlue;
                case DamageElement.Necrotic:
                    return Color.DarkGreen;
                case DamageElement.Poison:
                    return Color.Green;
                case DamageElement.Psychic:
                    return Color.DeepPink;
                case DamageElement.Radiant:
                    return Color.Yellow;
                case DamageElement.Thunder:
                    return Color.MediumPurple;
                case DamageElement.Unknown:
                    return Main.DiscoColor;
                default:
                    return Color.White;
            }
        }


        /// <summary>
        /// 获得怪的抗性
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        public static Dictionary<DamageElement, float> GetResistance(this NPC npc)
        {
            Dictionary<DamageElement, float> resistance = new();
            resistance.AddRange(npc.GetGlobalNPC<EnemyStatusSystem>().InitialResistance);
            EnemyStatusSystem gnpc = npc.GetGlobalNPC<EnemyStatusSystem>();
            if (gnpc.FireAndColdStatus == FireAndColdStatusID.Wet)
            {
                if (!resistance.TryAdd(DamageElement.Fire, 0.5f))
                {
                    resistance[DamageElement.Fire] = 0.5f;
                }
                if (!resistance.TryAdd(DamageElement.Cold, 2f))
                {
                    resistance[DamageElement.Cold] = 2f;
                }
                if (!resistance.TryAdd(DamageElement.Lightning, 2f))
                {
                    resistance[DamageElement.Lightning] = 2f;
                }
            }
            else if (gnpc.FireAndColdStatus == FireAndColdStatusID.Chilled)
            {
                if (!resistance.TryAdd(DamageElement.Fire, 0.5f))
                {
                    resistance[DamageElement.Fire] = 0.5f;
                }
                if (!resistance.TryAdd(DamageElement.Cold, 2f))
                {
                    resistance[DamageElement.Cold] = 2f;
                }
            }
            else if (gnpc.FireAndColdStatus == FireAndColdStatusID.Frozen)
            {
                if (!resistance.TryAdd(DamageElement.Fire, 0.5f))
                {
                    resistance[DamageElement.Fire] = 0.5f;
                }
                if (!resistance.TryAdd(DamageElement.Force, 4f))
                {
                    resistance[DamageElement.Force] = 4f;
                }
                if (!resistance.TryAdd(DamageElement.Thunder, 4f))
                {
                    resistance[DamageElement.Thunder] = 4f;
                }
                if (!resistance.TryAdd(DamageElement.None, 4f))
                {
                    resistance[DamageElement.None] = 4f;
                }
            }
            if (npc.HasBuff(ModContent.BuffType<GreaseBuff>()))
            {
                if (!resistance.TryAdd(DamageElement.Fire, 1f))
                {
                    if (resistance[DamageElement.Fire] < 1)
                    {
                        resistance[DamageElement.Fire] = 1f;
                    }
                }
            }
            if (npc.HasBuff(ModContent.BuffType<SilencedDNDBuff>()))
            {
                if (!resistance.TryAdd(DamageElement.Thunder, 0))
                {
                    resistance[DamageElement.Thunder] = 0;
                }
            }
            return resistance;
        }

        public static bool LavaCollision(Vector2 Position, int Width, int Height)
        {
            int value = (int)(Position.X / 16f) - 1;
            int value2 = (int)((Position.X + (float)Width) / 16f) + 2;
            int value3 = (int)(Position.Y / 16f) - 1;
            int value4 = (int)((Position.Y + (float)Height) / 16f) + 2;
            int num = Utils.Clamp(value, 0, Main.maxTilesX - 1);
            value2 = Utils.Clamp(value2, 0, Main.maxTilesX - 1);
            value3 = Utils.Clamp(value3, 0, Main.maxTilesY - 1);
            value4 = Utils.Clamp(value4, 0, Main.maxTilesY - 1);
            Vector2 vector = default;
            for (int i = num; i < value2; i++)
            {
                for (int j = value3; j < value4; j++)
                {
                    if (Main.tile[i, j] != null && Main.tile[i, j].LiquidAmount > 0 && Main.tile[i, j].LiquidType == LiquidID.Lava)
                    {
                        vector.X = i * 16;
                        vector.Y = j * 16;
                        int num2 = 16;
                        float num3 = 256 - Main.tile[i, j].LiquidAmount;
                        num3 /= 32f;
                        vector.Y += num3 * 2f;
                        num2 -= (int)(num3 * 2f);
                        if (Position.X + Width > vector.X && Position.X < vector.X + 16f && Position.Y + Height > vector.Y && Position.Y < vector.Y + num2)
                            return true;
                    }
                }
            }

            return false;
        }

        public static bool LavaCollision(NPC npc) => LavaCollision(npc.position, npc.width, npc.height);

        public static bool WaterCollision(Vector2 Position, int Width, int Height)
        {
            int value = (int)(Position.X / 16f) - 1;
            int value2 = (int)((Position.X + (float)Width) / 16f) + 2;
            int value3 = (int)(Position.Y / 16f) - 1;
            int value4 = (int)((Position.Y + (float)Height) / 16f) + 2;
            int num = Utils.Clamp(value, 0, Main.maxTilesX - 1);
            value2 = Utils.Clamp(value2, 0, Main.maxTilesX - 1);
            value3 = Utils.Clamp(value3, 0, Main.maxTilesY - 1);
            value4 = Utils.Clamp(value4, 0, Main.maxTilesY - 1);
            Vector2 vector = default;
            for (int i = num; i < value2; i++)
            {
                for (int j = value3; j < value4; j++)
                {
                    if (Main.tile[i, j] != null && Main.tile[i, j].LiquidAmount > 0 && Main.tile[i, j].LiquidType == LiquidID.Water)
                    {
                        vector.X = i * 16;
                        vector.Y = j * 16;
                        int num2 = 16;
                        float num3 = 256 - Main.tile[i, j].LiquidAmount;
                        num3 /= 32f;
                        vector.Y += num3 * 2f;
                        num2 -= (int)(num3 * 2f);
                        if (Position.X + Width > vector.X && Position.X < vector.X + 16f && Position.Y + Height > vector.Y && Position.Y < vector.Y + num2)
                            return true;
                    }
                }
            }

            return false;
        }
        public static bool WaterCollision(NPC npc) => WaterCollision(npc.position, npc.width, npc.height);
    }

}
