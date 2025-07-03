using BG3MagicRework.Static;
using Terraria.ModLoader;

namespace BG3MagicRework
{
    public class BG3MagicRework : Mod
    {
        public override void Load()
        {
            TextureLibrary.Load();
            EffectLibrary.Load();
        }

        public override void Unload()
        {
            TextureLibrary.Unload();
            EffectLibrary.Unload();
        }
    }

}
