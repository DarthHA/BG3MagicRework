using BG3MagicRework.Items.Icons;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BG3MagicRework.BaseType
{
    public abstract class BaseSpell
    {
        /// <summary>
        /// 法术内部名
        /// </summary>
        public virtual string Name => "";

        /// <summary>
        /// 法术名字颜色
        /// </summary>
        public virtual Color NameColor => Color.White;

        /// <summary>
        /// 所需环数，最低为0，也就是戏法
        /// </summary>
        public virtual int InitialRing => 0;

        /// <summary>
        /// 显示法术伤害
        /// </summary>
        public virtual DiceDamage BaseDamage => new();

        /// <summary>
        /// 满足某些条件时的显示伤害
        /// </summary>
        public virtual DiceDamage ExtraDamage => new();

        /// <summary>
        /// 升环施法所加的伤害
        /// </summary>
        public virtual DiceDamage RisingDamageAddition => new();

        /// <summary>
        /// 升环施法所加的额外伤害
        /// </summary>
        public virtual DiceDamage RisingDamageAdditionExtra => new();

        /// <summary>
        /// 是否为反应
        /// </summary>
        public virtual bool IsReaction => false;

        /// <summary>
        /// 反应冷却
        /// </summary>
        public virtual int ReactionCD => 0;

        /// <summary>
        /// 持续时间，-1为无限
        /// </summary>
        public virtual int TimeSpan => 0;

        /// <summary>
        /// 检定难度，-1不显示
        /// </summary>
        public virtual int DifficultyClass => -1;

        /// <summary>
        /// 是否需要专注
        /// </summary>
        public virtual bool Concentration => false;

        /// <summary>
        /// 施法距离，0不显示，-1为自身
        /// </summary>
        public virtual int SpellRange => 0;

        /// <summary>
        /// 影响范围，0不显示
        /// </summary>
        public virtual int AOERadius => 0;

        /// <summary>
        /// 法术学派
        /// </summary>
        public virtual SchoolOfMagic Shool => SchoolOfMagic.Unknown;

        public string GetName()
        {
            return Language.GetTextValue("Mods.BG3MagicRework.Spells.Names." + Name);
        }

        public string GetSchoolDesc()
        {
            string ringAndSchool = InitialRing == 0 ?
                (string.Format(Language.GetTextValue("Mods.BG3MagicRework.CantripAndXSchool"), Shool.GetLocalize())) :
                (string.Format(Language.GetTextValue("Mods.BG3MagicRework.XRingAndXSchool"), InitialRing, Shool.GetLocalize()));
            return ringAndSchool;
        }

        public string GetDamageDesc()
        {
            string damageDescs = "";
            if (!BaseDamage.NoDamage())
            {
                if (BaseDamage.NoDiceDamage())
                {
                    damageDescs += string.Format(Language.GetTextValue("Mods.BG3MagicRework.DamageDescsOne"), Main.LocalPlayer.GetDiceDamage(BaseDamage, InitialRing, InitialRing, RisingDamageAddition).MinDamage());
                }
                else
                {
                    damageDescs += string.Format(Language.GetTextValue("Mods.BG3MagicRework.DamageDescs"), Main.LocalPlayer.GetDiceDamage(BaseDamage, InitialRing, InitialRing, RisingDamageAddition).MinDamage(), Main.LocalPlayer.GetDiceDamage(BaseDamage, InitialRing, InitialRing, RisingDamageAddition).MaxDamage());
                }
            }
            if (!ExtraDamage.NoDamage())
            {
                if (ExtraDamage.NoDiceDamage())
                {
                    damageDescs += string.Format(Language.GetTextValue("Mods.BG3MagicRework.ExtraDamageDescs"), Main.LocalPlayer.GetDiceDamage(ExtraDamage, InitialRing, InitialRing, RisingDamageAdditionExtra).MinDamage());
                }
                else
                {
                    damageDescs += string.Format(Language.GetTextValue("Mods.BG3MagicRework.ExtraDamageDescs"), Main.LocalPlayer.GetDiceDamage(ExtraDamage, InitialRing, InitialRing, RisingDamageAdditionExtra).MinDamage(), Main.LocalPlayer.GetDiceDamage(ExtraDamage, InitialRing, InitialRing, RisingDamageAdditionExtra).MaxDamage());
                }
            }
            return damageDescs;
        }

        public string GetBaseDiceCalcDesc()
        {
            string damageDescs = "";
            if (!BaseDamage.NoDamage())
            {
                damageDescs += Main.LocalPlayer.GetDiceDamage(BaseDamage, InitialRing, InitialRing, RisingDamageAddition).ShowWithColor();
            }
            return damageDescs;
        }

        public string GetExtraDiceTitle()
        {
            string Descs = "";
            if (!ExtraDamage.NoDamage())
            {
                Descs += Language.GetTextValue("Mods.BG3MagicRework.ExtraDamageDescs2");
            }
            return Descs;
        }

        public string GetExtraDiceCalcDesc()
        {
            string damageDescs = "";
            if (!ExtraDamage.NoDamage())
            {
                damageDescs += Main.LocalPlayer.GetDiceDamage(ExtraDamage, InitialRing, InitialRing, RisingDamageAdditionExtra).ShowWithColor();
            }
            return damageDescs;
        }

        public string GetDesc()
        {
            return Language.GetTextValue("Mods.BG3MagicRework.Spells.Descs." + Name);
        }

        public string GetTimeSpanDesc()
        {
            string timeSpan = "";
            if (TimeSpan == -1)
            {
                timeSpan = $"[i:{ModContent.ItemType<TimeSpanIconItem>()}]" + Language.GetTextValue("Mods.BG3MagicRework.Infinite");
            }
            else if (TimeSpan > 0)
            {
                timeSpan = $"[i:{ModContent.ItemType<TimeSpanIconItem>()}]" + Main.LocalPlayer.GetTimeSpan(Name).ToString() + Language.GetTextValue("Mods.BG3MagicRework.Seconds");
            }
            return timeSpan;
        }

        public string GetRisingRingTitle()
        {
            string title = (InitialRing == 0 ? Language.GetTextValue("Mods.BG3MagicRework.LvlUpCantrip") : $"[i:{ModContent.ItemType<UpcastIconItem>()}]" + Language.GetTextValue("Mods.BG3MagicRework.RiseRing"));
            return title;
        }

        public string GetRisingRingDesc()
        {
            string desc = "";
            if (InitialRing == 0)
            {
                if (!BaseDamage.NoDamage())
                {
                    desc += Language.GetTextValue("Mods.BG3MagicRework.LvlUpCantripNormalText1");
                    if (!RisingDamageAddition.NoDamage())
                    {
                        desc += "\n" + string.Format(Language.GetTextValue("Mods.BG3MagicRework.LvlUpCantripNormalText2"), RisingDamageAddition.SimpleShowWithColor());
                    }
                    if (!RisingDamageAdditionExtra.NoDamage())
                    {
                        desc += "\n" + string.Format(Language.GetTextValue("Mods.BG3MagicRework.LvlUpCantripNormalText3"), RisingDamageAdditionExtra.SimpleShowWithColor());
                    }
                }
            }
            else
            {
                if (!BaseDamage.NoDamage())
                {
                    desc += Language.GetTextValue("Mods.BG3MagicRework.RiseRingNormalText1");
                    if (!RisingDamageAddition.NoDamage())
                    {
                        desc += "\n" + string.Format(Language.GetTextValue("Mods.BG3MagicRework.RiseRingNormalText2"), RisingDamageAddition.SimpleShowWithColor());
                    }
                    if (!RisingDamageAdditionExtra.NoDamage())
                    {
                        desc += "\n" + string.Format(Language.GetTextValue("Mods.BG3MagicRework.RiseRingNormalText3"), RisingDamageAdditionExtra.SimpleShowWithColor());
                    }
                }
            }
            string local = Language.GetTextValue("Mods.BG3MagicRework.Spells.RiseRingDescs." + Name);
            if (desc == "")
            {
                if (local == "")
                {
                    if (InitialRing == 0)
                    {
                        desc = Language.GetTextValue("Mods.BG3MagicRework.NoLvlUpEffect");
                    }
                    else
                    {
                        desc = Language.GetTextValue("Mods.BG3MagicRework.NoRiseRingEffect");
                    }
                }
                else
                {
                    desc = local;
                }
            }
            else
            {
                if (local != "")
                {
                    desc += "\n" + local;
                }
            }
            return desc;
        }
        public string GetOtherDescs()
        {
            string bLaBaLa = "";
            if (AOERadius != 0) bLaBaLa += $"[i:{ModContent.ItemType<AOEIconItem>()}]" + AOERadius.ToString() + LangLibrary.Tiles + "  ";
            if (SpellRange > 0) bLaBaLa += $"[i:{ModContent.ItemType<SpellRangeIconItem>()}]" + SpellRange.ToString() + LangLibrary.Tiles + "  ";
            if (SpellRange == -1) bLaBaLa += $"[i:{ModContent.ItemType<SpellRangeIconItem>()}]" + Language.GetTextValue("Mods.BG3MagicRework.Self") + "  ";
            if (DifficultyClass != -1) bLaBaLa += $"[i:{ModContent.ItemType<DCIconItem>()}]" + Language.GetTextValue("Mods.BG3MagicRework.DifficultyClass") + DifficultyClass.ToString() + "  ";
            if (Concentration) bLaBaLa += $"[i:{ModContent.ItemType<ConcentrationIconItem>()}]" + Language.GetTextValue("Mods.BG3MagicRework.ConRequired");
            if (bLaBaLa != "") bLaBaLa += "\n";
            string action = IsReaction ? ($"[i:{ModContent.ItemType<ReactionIconItem>()}]" + Language.GetTextValue("Mods.BG3MagicRework.IsReaction")) :
                ($"[i:{ModContent.ItemType<ActionIconItem>()}]" + Language.GetTextValue("Mods.BG3MagicRework.IsAction"));
            if (InitialRing != 0) action += "  " + $"[i:{ModContent.ItemType<SpellSlotIconItem>()}]" + string.Format(Language.GetTextValue("Mods.BG3MagicRework.XRingSlot"), InitialRing);

            return bLaBaLa + action;
        }


        public Texture2D GetIcon()
        {
            return ModContent.Request<Texture2D>("BG3MagicRework/Images/Icons/" + Name, AssetRequestMode.ImmediateLoad).Value;
        }

        public virtual void Shoot(Player player, ModProjectile modproj, Vector2 tipPosition, Vector2 mousePosition, int Ring)
        {

        }

        public virtual bool CanRelease(Player owner, ModProjectile modproj, Vector2 mousePosition, int Ring, ref string Warning)
        {
            return true;
        }

        public virtual bool DrawLight(Player owner, int ring, Vector2 tipPos, ref float light, ref Color color, ref float scale, float miscTimer, bool HasShot)
        {
            return true;
        }

        public virtual void DrawBehind(Player owner, int ring, float light, Color color, float scale, float miscTimer)
        {
        }

        public virtual void DrawFront(Player owner, int ring, float light, Color color, float scale, float miscTimer)
        {
        }

        public virtual bool ReactionEffect(Player player, int usedRing, float extraInfo1, float extraInfo2, float extraInfo3, float extraInfo4)
        {
            return true;
        }

        /// <summary>
        /// 未使用
        /// </summary>
        /// <param name="player"></param>
        /// <param name="useRing"></param>
        /// <returns></returns>
        public virtual bool ConsumeSlots(Player player, int willUseRing)
        {
            return true;
        }

        public virtual bool ModifyDrawRangeInfo(Player player, int Ring)
        {
            return true;
        }
    }

    public enum SchoolOfMagic
    {
        /// <summary>
        /// 防护
        /// </summary>
        Abjuration,
        /// <summary>
        /// 咒法
        /// </summary>
        Conjuration,
        /// <summary>
        /// 预言
        /// </summary>
        Divination,
        /// <summary>
        /// 惑控
        /// </summary>
        Enchantment,
        /// <summary>
        /// 塑能
        /// </summary>
        Evocation,
        /// <summary>
        /// 幻术
        /// </summary>
        Illusion,
        /// <summary>
        /// 死灵
        /// </summary>
        Necromancy,
        /// <summary>
        /// 变化
        /// </summary>
        Transmutation,
        /// <summary>
        /// 未知
        /// </summary>
        Unknown
    }
}
