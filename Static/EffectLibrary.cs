using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Reflection;
using Terraria.ModLoader;

namespace BG3MagicRework.Static
{
    public static class EffectLibrary
    {
        public static Effect NormalColorEffect;
        public static Effect GradientColorEffect;
        public static Effect SimpleLoopEffect;
        public static Effect RoSLoopEffect;
        public static Effect RoSLoopVertEffect;
        public static Effect RollingSphere;
        public static Effect CrackEffect;
        public static Effect WhiteEffect;
        public static Effect LoopMaskEffect;
        public static Effect GradientCircleEffect;
        public static Effect FrozenEffect;

        public static void Load()
        {
            FieldInfo[] f = typeof(EffectLibrary).GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (FieldInfo info in f)
            {
                if (info.FieldType == typeof(Effect))
                {
                    info.SetValue(null, ModContent.Request<Effect>("BG3MagicRework/Effects/" + info.Name, AssetRequestMode.ImmediateLoad).Value);
                }
            }
        }

        public static void Unload()
        {
            FieldInfo[] f = typeof(EffectLibrary).GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (FieldInfo info in f)
            {
                if (info.FieldType == typeof(Effect))
                {
                    info.SetValue(null, null);
                }
            }
        }
    }
}
