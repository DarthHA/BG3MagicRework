using BG3MagicRework.Buffs;
using BG3MagicRework.Spells.Reaction;
using BG3MagicRework.Static;
using BG3MagicRework.Systems;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Channel
{
    public abstract class BaseChannel : ModProjectile
    {
        /// <summary>
        /// 状态，0为举起,1为待机,2为复位.3为释放，4为后摇
        /// </summary>
        public int Phase = 0;
        /// <summary>
        /// 计时器
        /// </summary>
        public int Timer = 0;

        /// <summary>
        /// 法术名
        /// </summary>
        public string Spell = "";
        /// <summary>
        /// 当前法术环数
        /// </summary>
        public int currentRing = 0;

        /// <summary>
        /// 吟唱物种类
        /// </summary>
        public int ItemType = 0;

        /// <summary>
        /// 光圈颜色
        /// </summary>
        public Color LightColor = Color.White;

        /// <summary>
        /// 光圈大小
        /// </summary>
        public float Scale = 1f;

        /// <summary>
        /// 光圈亮度
        /// </summary>
        public float Light = 0;

        /// <summary>
        /// 绘制特效用计时器
        /// </summary>
        public float miscTimer = 0;

        /// <summary>
        /// 立刻免费使用，不能改环
        /// </summary>
        public bool InstantAndFree = false;

        public override string Texture => "BG3MagicRework/Images/PlaceHolder";


        public bool CanRelease(Player owner, ref string warning)
        {
            bool success = true;
            string tmp = "";
            if (owner.HasBuff(ModContent.BuffType<ArcaneHungerBuff>()))
            {
                tmp += LangLibrary.HungerReason + "\n";
                success = false;
            }
            if (SilenceBlockUseMagic(owner))
            {
                tmp += LangLibrary.DebuffReason + "\n";
                success = false;
            }
            if (!EverythingLibrary.spells[Spell].CanRelease(owner, this, Main.MouseWorld, currentRing, ref tmp))
            {
                success = false;
            }
            if (!success)
            {
                warning += LangLibrary.CannotRelease + "\n" + tmp;
            }
            else
            {
                warning += tmp;
            }
            return success;
        }

        public static bool SilenceBlockUseMagic(Player player)
        {
            if (player.CCed || player.silence || player.cursed)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 滚动改环
        /// </summary>
        /// <param name="player"></param>
        public void ScrollChangeRing(Player player)
        {
            if (PlayerInput.ScrollWheelDeltaForUI != 0)
            {
                int factor = PlayerInput.ScrollWheelDeltaForUI / 120;

                List<int> availableRings = player.GetAvailableRings(EverythingLibrary.spells[Spell].InitialRing);
                if (factor < 0)  //增大
                {
                    int result = -1;
                    foreach (int ar in availableRings)
                    {
                        if (ar > currentRing)
                        {
                            if (result == -1 || ar < result)
                            {
                                result = ar;
                            }
                        }
                    }
                    if (result != -1) currentRing = result;
                }
                else
                {
                    int result = -1;
                    foreach (int ar in availableRings)
                    {
                        if (ar < currentRing)
                        {
                            if (result == -1 || ar > result)
                            {
                                result = ar;
                            }
                        }
                    }
                    if (result != -1) currentRing = result;
                }
            }
        }

        /// <summary>
        /// 检查是否有能用的环位，假如说当前环位不可用就寻找可用的最小环位，如果没找到就返回false
        /// </summary>
        /// <returns></returns>
        public bool CheckAnyRingAvailable(Player player)
        {
            if (player.GetAvailableRings(currentRing).Contains(currentRing))
            {
                return true;
            }
            //特殊情况法术位被反应用光了,用可以用的最小环
            List<int> availableRings = player.GetAvailableRings(EverythingLibrary.spells[Spell].InitialRing);
            int result = -1;
            foreach (int ar in availableRings)
            {
                if (result == -1 || ar < result)
                {
                    result = ar;
                }
            }
            if (result != -1)
            {
                currentRing = result;
                return true;
            }
            else
            {
                return false;
            }

        }

        public void ReleaseMagic(Vector2 TipPos)
        {
            Player owner = Main.player[Projectile.owner];
            EverythingLibrary.spells[Spell].Shoot(owner, this, TipPos, Main.MouseWorld, currentRing);
            if (owner.HasReaction<LegionOfBeesSpell>() && owner.GetModPlayer<DNDMagicPlayer>().BeeTrigger)
            {
                owner.GetModPlayer<DNDMagicPlayer>().BeeTimer = EverythingLibrary.GetSpell<LegionOfBeesSpell>().ReactionCD;
            }
            if (currentRing > 0 && !InstantAndFree)
            {
                if (!owner.GetModPlayer<DNDMagicPlayer>().ConsumedSpellSlot.TryAdd(currentRing, 1))
                {
                    owner.GetModPlayer<DNDMagicPlayer>().ConsumedSpellSlot[currentRing]++;
                }
                if (owner.manaFlower && owner.GetModPlayer<DNDMagicPlayer>().ConsumedSpellSlot.Count > 0)        //魔力花效果
                {
                    owner.QuickMana();
                }
            }
            if (ModContent.GetInstance<BG3Config>().ShowCombatInfo)
            {
                string spellname = EverythingLibrary.spells[Spell].GetName();
                SomeUtils.AddColorString(ref spellname, EverythingLibrary.spells[Spell].NameColor);
                Main.NewText(string.Format(LangLibrary.XUsedSpellX, owner.name, spellname));
            }
        }

        public void SetCursorInfo(string mouseText)
        {
            if (mouseText != "")
            {
                if (!Main.mouseText)
                {
                    Main.mouseText = true;
                    PlayerInput.SetZoom_UI();
                    Main.instance.MouseText(mouseText, 0, 0, Main.mouseX + 50, Main.mouseY - 10, Main.screenWidth + 1000, Main.screenHeight + 1000);
                    PlayerInput.SetZoom_MouseInWorld();
                    ModifyDrawUI.DrawSpellName = Spell;
                }
            }
        }

        public void DrawSpellRangeInfo(Player player)
        {
            if (EverythingLibrary.spells[Spell].ModifyDrawRangeInfo(player, currentRing))
            {
                Vector2 mouseWorld = Main.MouseWorld;
                if (player.GetSpellRange(Spell) > 0)
                {
                    DrawUtils.DrawIndicatorRing(player.Center, player.GetSpellRange(Spell) * 16);
                    if (mouseWorld.Distance(player.Center) > (player.GetSpellRange(Spell) * 16))
                    {
                        mouseWorld = player.Center + Vector2.Normalize(mouseWorld - player.Center) * (player.GetSpellRange(Spell) * 16);
                    }
                }
                if (player.GetAOERadius(Spell) > 0)
                {
                    DrawUtils.DrawIndicatorRing(mouseWorld, player.GetAOERadius(Spell) * 16);
                }
            }
        }
    }
}
