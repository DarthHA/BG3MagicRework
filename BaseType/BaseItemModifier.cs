using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;

namespace BG3MagicRework.BaseType
{
    public abstract class BaseArmorSetModifier
    {
        /// <summary>
        /// 本地化用的
        /// </summary>
        public virtual string Name => "";
        public virtual List<int> Head => new();
        public virtual List<int> Body => new();
        public virtual List<int> Legs => new();
        public virtual void UpdateArmorSet(Player player)
        {

        }
    }

    public abstract class BaseItemModifier
    {
        public virtual int Type => 0;
        public virtual void SetDefaults(Item item)
        {
        }
        public virtual bool? UseItem(Item item, Player player)
        {
            return null;
        }
        public virtual bool CanUseItem(Item item, Player player)
        {
            return true;
        }

        public virtual void UpdateEquip(Item item, Player player)
        {
        }

        public virtual string GetTooltip()
        {
            return "";
        }
        public string GetLocalization(string key) => Language.GetTextValue("Mods.BG3MagicRework.TooltipModify." + key);
    }

    public abstract class BaseWeaponModifier
    {
        public virtual int Type => 0;
        public virtual string SpellName => "";
        public virtual bool AlterFunctionUse => false;
        public virtual void SetDefaults(Item item)
        {

        }
        public virtual bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return true;
        }

        public virtual bool CanUse(Item item, Player player, bool AlterFunctionUse = false)
        {
            return true;
        }
    }
}