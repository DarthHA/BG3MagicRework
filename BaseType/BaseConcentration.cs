using BG3MagicRework.Static;
using BG3MagicRework.Systems;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BG3MagicRework.BaseType
{
    public abstract class BaseConcentration
    {
        public long UUID = 0;
        public virtual string Name => "";
        public int TimeLeft = 0;
        public int projIndex = -1;
        public int Ring = 0;
        public virtual bool UpdateAndDecide(Player player)
        {
            return true;
        }

        public virtual void ModifyName(ref string name)
        {

        }

        public virtual void ModifyDesc(ref string desc)
        {

        }

        public Texture2D GetIcon()
        {
            return ModContent.Request<Texture2D>("BG3MagicRework/Concentrations/" + Name, AssetRequestMode.ImmediateLoad).Value;
        }

        public string GetName(bool withColor = false, bool withPrefix = true)
        {
            string result = (withPrefix ? Language.GetTextValue("Mods.BG3MagicRework.ConPrefix") : "") + Language.GetTextValue("Mods.BG3MagicRework.Concentrations.Name." + Name);
            ModifyName(ref result);
            if (withColor)
            {
                if (EverythingLibrary.spells.TryGetValue(Name, out BaseSpell spell))
                {
                    SomeUtils.AddColorString(ref result, spell.NameColor);
                }
            }
            return result;
        }

        public string GetDesc()
        {
            string result = Language.GetTextValue("Mods.BG3MagicRework.Concentrations.Desc." + Name);
            ModifyDesc(ref result);
            return result;
        }

        public static BaseConcentration NewConcentration<T>() where T : BaseConcentration
        {
            BaseConcentration template = EverythingLibrary.GetConcentration<T>();
            if (template != null)
            {
                BaseConcentration instance = (BaseConcentration)Activator.CreateInstance(template.GetType());
                return instance;
            }
            return null;
        }
    }
}
