using BG3MagicRework.BaseType;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria.ModLoader;

namespace BG3MagicRework.Systems
{
    public class EverythingLibrary : ModSystem
    {
        public static List<BaseArmorSetModifier> armorSetModifiers = new();
        public static Dictionary<int, BaseItemModifier> itemModifiers = new();
        public static Dictionary<int, BaseWeaponModifier> weaponModifiers = new();
        public static Dictionary<string, BaseConcentration> concentrations = new();
        public static Dictionary<string, BaseSpell> spells = new();

        public static BaseSpell GetSpell<T>() where T : BaseSpell
        {
            foreach (var v in spells.Values)
            {
                if (v.GetType() == typeof(T))
                {
                    return v;
                }
            }
            return null;
        }

        public static BaseConcentration GetConcentration<T>() where T : BaseConcentration
        {
            foreach (var v in concentrations.Values)
            {
                if (v.GetType() == typeof(T))
                {
                    return v;
                }
            }
            return null;
        }

        public override void Load()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsClass && !type.IsAbstract)
                {
                    if (typeof(BaseArmorSetModifier).IsAssignableFrom(type))
                    {
                        BaseArmorSetModifier instance = (BaseArmorSetModifier)Activator.CreateInstance(type);
                        armorSetModifiers.Add(instance);
                    }
                    if (typeof(BaseItemModifier).IsAssignableFrom(type))
                    {
                        BaseItemModifier instance = (BaseItemModifier)Activator.CreateInstance(type);
                        itemModifiers.Add(instance.Type, instance);
                    }
                    if (typeof(BaseWeaponModifier).IsAssignableFrom(type))
                    {
                        BaseWeaponModifier instance = (BaseWeaponModifier)Activator.CreateInstance(type);
                        weaponModifiers.Add(instance.Type, instance);
                    }
                    if (typeof(BaseSpell).IsAssignableFrom(type))
                    {
                        BaseSpell instance = (BaseSpell)Activator.CreateInstance(type);
                        spells.Add(instance.Name, instance);
                    }
                    if (typeof(BaseConcentration).IsAssignableFrom(type))
                    {
                        BaseConcentration instance = (BaseConcentration)Activator.CreateInstance(type);
                        concentrations.Add(instance.Name, instance);
                    }
                }
            }
        }

        public override void Unload()
        {
            itemModifiers.Clear();
            armorSetModifiers.Clear();
            weaponModifiers.Clear();
            spells.Clear();
            concentrations.Clear();
        }
    }
}
