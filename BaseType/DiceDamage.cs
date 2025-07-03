using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.BaseType
{
    public readonly struct DiceDamage
    {
        /// <summary>
        /// 骰子面最值，其为1时视为固定值
        /// </summary>
        private readonly List<int> DiceValue;
        /// <summary>
        /// 掷骰数量，当固定值时用于确定固定值数量
        /// </summary>
        private readonly List<int> DiceCount;
        /// <summary>
        /// 骰子的元素种类
        /// </summary>
        private readonly List<DamageElement> DiceElements;
        /// <summary>
        /// 骰子的全局修正值
        /// </summary>
        public readonly float DmgPerDice = 1;

        public DiceDamage()
        {
            DiceValue = new();
            DiceCount = new();
            DmgPerDice = 1;
            DiceElements = new();
        }

        public DiceDamage(DiceDamage source)
        {
            DiceValue = new(source.DiceValue);
            DiceCount = new(source.DiceCount);
            DmgPerDice = source.DmgPerDice;
            DiceElements = new(source.DiceElements);
        }

        public DiceDamage(int dice, int count, DamageElement diceElement = DamageElement.None, float dmgPerDice = 1)
        {
            DiceValue = new List<int> { dice };
            DiceCount = new List<int> { count };
            DiceElements = new List<DamageElement> { diceElement };
            DmgPerDice = dmgPerDice;
        }
        public DiceDamage(int flat, DamageElement flatElement = DamageElement.None, float dmgPerDice = 1)
        {
            DiceValue = new() { 1 };
            DiceCount = new() { flat };
            DiceElements = new() { flatElement };
            DmgPerDice = dmgPerDice;
        }
        private DiceDamage(List<int> dice, List<int> count, List<DamageElement> diceElements, float dmgPerDice = 1)
        {
            DiceValue = new(dice);
            DiceCount = new(count);
            DiceElements = new(diceElements);
            DmgPerDice = dmgPerDice;
        }
        public static DiceDamage operator +(DiceDamage a, DiceDamage b)
        {
            var newDice = new List<int>(a.DiceValue);
            newDice.AddRange(b.DiceValue);

            var newCount = new List<int>(a.DiceCount);
            newCount.AddRange(b.DiceCount);

            var newDiceElements = new List<DamageElement>(a.DiceElements);
            newDiceElements.AddRange(b.DiceElements);


            return new DiceDamage(newDice, newCount, newDiceElements, Math.Max(a.DmgPerDice, b.DmgPerDice));
        }

        public static DiceDamage operator *(DiceDamage a, float b)
        {
            var newDice = new List<int>(a.DiceValue);
            var newCount = new List<int>(a.DiceCount);
            var newDiceElements = new List<DamageElement>(a.DiceElements);
            return new DiceDamage(newDice, newCount, newDiceElements, a.DmgPerDice * b);
        }

        public static DiceDamage operator /(DiceDamage a, float b)
        {
            if (b == 0) throw new Exception("不能除以0");
            var newDice = new List<int>(a.DiceValue);
            var newCount = new List<int>(a.DiceCount);
            var newDiceElements = new List<DamageElement>(a.DiceElements);
            return new DiceDamage(newDice, newCount, newDiceElements, a.DmgPerDice / b);
        }

        public readonly int Damage(out bool crit, Dictionary<DamageElement, float> Resistance, bool ForceCrit = false)
        {
            bool GreatSuccess = true;
            bool FlatOnly = true;
            List<int> diceRoll = new();
            //第一步，投点，存点，判断暴击
            for (int i = 0; i < DiceValue.Count; i++)
            {
                if (DiceValue[i] == 1)      //为固定值
                {
                    //不会掷骰
                }
                else
                {
                    FlatOnly = false;
                    for (int j = 0; j < DiceCount[i]; j++)
                    {
                        int roll = ForceCrit ? DiceValue[i] : Main.rand.Next(DiceValue[i]) + 1;
                        if (roll < DiceValue[i]) GreatSuccess = false;
                        diceRoll.Add(roll);
                    }
                }
            }
            crit = GreatSuccess && !FlatOnly;

            //第二步，计算伤害
            int dmg = 0;
            for (int i = 0; i < DiceValue.Count; i++)
            {
                if (DiceValue[i] == 1)      //为固定值
                {
                    dmg += DiceCount[i];
                }
                else
                {
                    for (int j = 0; j < DiceCount[i]; j++)
                    {
                        int roll = diceRoll[0];
                        float resist = 1f;
                        if (Resistance.TryGetValue(DiceElements[i], out float value)) resist = value;
                        dmg += (int)(roll * resist);
                        diceRoll.RemoveAt(0);
                    }
                }
            }
            return (int)(dmg * DmgPerDice);
        }

        public readonly int MaxDamage()
        {
            int dmg = 0;
            for (int i = 0; i < DiceValue.Count; i++)
            {
                if (DiceValue[i] == 1)
                {
                    dmg += DiceCount[i];
                }
                else
                {
                    for (int j = 0; j < DiceCount[i]; j++)
                    {
                        dmg += DiceValue[i];
                    }
                }
            }
            return Math.Max((int)(dmg * DmgPerDice), 0);
        }

        public readonly int MinDamage()
        {
            int dmg = 0;
            for (int i = 0; i < DiceValue.Count; i++)
            {
                for (int j = 0; j < DiceCount[i]; j++)
                {
                    dmg += 1;
                }
            }
            return Math.Max((int)(dmg * DmgPerDice), 0);
        }

        public bool NoDamage()
        {
            return MaxDamage() <= 0;
        }

        public override readonly string ToString()
        {
            return ShowCalc();
        }

        public readonly string ShowCalc()
        {
            string result = "";
            for (int i = 0; i < DiceValue.Count; i++)
            {
                if (result != "") result += "+";
                string icon = $"[i:BG3MagicRework/{DiceElements[i]}DamageIconItem]";
                if (DiceValue[i] > 1)
                {
                    if (DmgPerDice == 1)
                    {
                        result += $"{DiceCount[i]}d{DiceValue[i]}({icon}{LangLibrary.GetLocalize(DiceElements[i])})";
                    }
                    else
                    {
                        result += $"{DiceCount[i]}d{DiceValue[i]}x{DmgPerDice}({icon}{LangLibrary.GetLocalize(DiceElements[i])})";
                    }
                }
                else
                {
                    if (DmgPerDice == 1)
                    {
                        result += $"{DiceCount[i]}({icon}{LangLibrary.GetLocalize(DiceElements[i])})";
                    }
                    else
                    {
                        result += $"{DiceCount[i]}x{DmgPerDice}({icon}{LangLibrary.GetLocalize(DiceElements[i])})";
                    }
                }
            }
            return result;
        }

        public readonly string ShowWithColor()
        {
            string result = "";
            for (int i = 0; i < DiceCount.Count; i++)
            {
                if (result != "") result += " +\n";
                string line1, line2;
                string icon = $"[i:BG3MagicRework/{DiceElements[i]}DamageIconItem]";
                if (DiceValue[i] > 1)
                {
                    if (DmgPerDice == 1)
                    {
                        line1 = $"{DiceCount[i]}d{DiceValue[i]} ";
                        line2 = LangLibrary.GetLocalize(DiceElements[i]);
                    }
                    else
                    {
                        line1 = $"{DiceCount[i]}d{DiceValue[i]} x{DmgPerDice} ";
                        line2 = LangLibrary.GetLocalize(DiceElements[i]);
                    }
                }
                else
                {
                    if (DmgPerDice == 1)
                    {
                        line1 = $"{DiceCount[i]} ";
                        line2 = LangLibrary.GetLocalize(DiceElements[i]);
                    }
                    else
                    {
                        line1 = $"{DiceCount[i]} x{DmgPerDice} ";
                        line2 = LangLibrary.GetLocalize(DiceElements[i]);
                    }
                }
                SomeUtils.AddColorString(ref line1, SomeUtils.GetColor(DiceElements[i]));
                SomeUtils.AddColorString(ref line2, SomeUtils.GetColor(DiceElements[i]));
                result += line1 + icon + line2;
            }

            return result;
        }

        public readonly string SimpleShowWithColor()
        {
            string result = "";
            for (int i = 0; i < DiceCount.Count; i++)
            {
                if (result != "") result += " +\n";
                string line1, line2;
                string icon = $"[i:BG3MagicRework/{DiceElements[i]}DamageIconItem]";
                if (DiceValue[i] > 1)
                {
                    line1 = $"{DiceCount[i]}d{DiceValue[i]} ";
                    line2 = LangLibrary.GetLocalize(DiceElements[i]);
                }
                else
                {
                    line1 = $"{DiceCount[i]} ";
                    line2 = LangLibrary.GetLocalize(DiceElements[i]);
                }
                SomeUtils.AddColorString(ref line1, SomeUtils.GetColor(DiceElements[i]));
                SomeUtils.AddColorString(ref line2, SomeUtils.GetColor(DiceElements[i]));
                result += line1 + icon + line2;
            }

            return result;
        }

        public readonly void ChangeUnknownElement(DamageElement element)
        {
            for (int i = 0; i < DiceCount.Count; i++)
            {
                if (DiceElements[i] == DamageElement.Unknown)
                {
                    DiceElements[i] = element;
                }
            }
        }

        /// <summary>
        /// 判断是否有骰子伤害
        /// </summary>
        /// <returns></returns>
        public readonly bool NoDiceDamage()
        {
            if (DiceCount.Count == 0) return true;
            for (int i = 0; i < DiceCount.Count; i++)
            {
                if (DiceValue[i] > 1)
                    return false;
            }
            return true;
        }

        public bool HasElement(DamageElement element)
        {
            for (int i = 0; i < DiceElements.Count; i++)
            {
                if (DiceElements[i] == element) return true;
            }
            return false;
        }

        public DamageElement GetLastElement()
        {
            if (DiceElements.Count > 0) return DiceElements[DiceElements.Count - 1];
            return DamageElement.None;
        }

        public readonly float GetDamagePerDice()
        {
            return DmgPerDice;
        }

        public readonly int GetDiceCount()
        {
            int count = 0;
            for (int i = 0; i < DiceValue.Count; i++)
            {
                if (DiceValue[i] > 1) count++;
            }
            return count;
        }

        public readonly DiceDamage FireAdd1D4Fire()
        {
            DiceDamage result = new(DiceValue, DiceCount, DiceElements, DmgPerDice);
            return result + new DiceDamage(4, 1, DamageElement.Fire);
        }

        public readonly DiceDamage GetDamageAddition(DiceDamage add1, DiceDamage add2, DiceDamage add3)
        {
            DiceDamage result = new(DiceValue, DiceCount, DiceElements, DmgPerDice);
            for (int i = 0; i < add1.DiceValue.Count; i++)
            {
                result += new DiceDamage(add1.DiceValue[i], add1.DiceValue[i], add1.DiceElements[i], DmgPerDice);
                result += add2;
            }
            result += add3;
            return result;
        }

        public readonly DiceDamage GetRisingRingAddition(DiceDamage risingRing)
        {
            var newDiceValue = new List<int>(DiceValue);
            var newDiceCount = new List<int>(DiceCount);
            var newDiceElements = new List<DamageElement>(DiceElements);
            if (risingRing.DiceValue.Count > 0)
            {
                for (int i = 0; i < newDiceValue.Count; i++)
                {
                    if (newDiceValue[i] == risingRing.DiceValue[0] && newDiceElements[i] == risingRing.DiceElements[0])
                    {
                        newDiceCount[i] += risingRing.DiceCount[0];
                    }
                }
            }
            return new(newDiceValue, newDiceCount, newDiceElements, DmgPerDice);
        }
        public readonly void DrawDice(Vector2 DrawPos)
        {
            Texture2D GetDiceTex(int dice)
            {
                string path = "BG3MagicRework/Images/Dice/D";
                switch (dice)
                {
                    case 4 or 6 or 8 or 10 or 12 or 20:
                        return ModContent.Request<Texture2D>(path + dice.ToString(), AssetRequestMode.ImmediateLoad).Value;
                    default:
                        return ModContent.Request<Texture2D>(path + "6", AssetRequestMode.ImmediateLoad).Value;
                }
            }
            Texture2D GetDiceLightTex(int dice)
            {
                string path = "BG3MagicRework/Images/Dice/D";
                switch (dice)
                {
                    case 4 or 6 or 8 or 10 or 12 or 20:
                        return ModContent.Request<Texture2D>(path + dice.ToString() + "_Light", AssetRequestMode.ImmediateLoad).Value;
                    default:
                        return ModContent.Request<Texture2D>(path + "6_Light", AssetRequestMode.ImmediateLoad).Value;
                }
            }
            Vector2 GetDiceDrawPos(int index)
            {
                if (index == 0) return Vector2.Zero;
                if (index % 2 == 1)
                {
                    return new Vector2(-10, -10 * (int)((index + 1) / 2));
                }
                else
                {
                    return new Vector2(10, -10 * (int)(index / 2));
                }
            }
            float GetDiceRot(int index)
            {
                if (index == 0) return 0f;
                float value;
                if (index % 2 == 1)
                {
                    value = -MathHelper.Pi / 6f;
                }
                else
                {
                    value = MathHelper.Pi / 6f;
                }
                if (index % 4 < 2) value = -value;
                return value;
            }
            int realDiceCount = GetDiceCount();
            int realDiceIndex = 0;
            Vector2 OffSet = new(realDiceCount == 1 ? 10 : 30, (int)(realDiceCount / 2) * 15 + 10);
            for (int i = DiceValue.Count - 1; i >= 0; i--)
            {
                if (DiceValue[i] > 1)
                {
                    Color DiceColor = SomeUtils.GetColor(DiceElements[i]);
                    Texture2D tex = GetDiceTex(DiceValue[i]);
                    Main.spriteBatch.Draw(tex, GetDiceDrawPos(realDiceCount - 1 - realDiceIndex) + OffSet + DrawPos, null, DiceColor, GetDiceRot(realDiceCount - 1 - realDiceIndex), tex.Size() / 2f, 0.8f, SpriteEffects.None, 0);
                    tex = GetDiceLightTex(DiceValue[i]);
                    Main.spriteBatch.Draw(tex, GetDiceDrawPos(realDiceCount - 1 - realDiceIndex) + OffSet + DrawPos, null, Color.LightGray, GetDiceRot(realDiceCount - 1 - realDiceIndex), tex.Size() / 2f, 0.8f, SpriteEffects.None, 0);
                    realDiceIndex++;
                }
            }
        }

    }
}
