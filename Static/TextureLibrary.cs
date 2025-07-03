using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Static
{
    public static class TextureLibrary
    {
        public static Texture2D BlobGlow;
        public static Texture2D BlobGlow2;
        public static Texture2D BloomFlare;
        public static Texture2D BloomLine;
        public static Texture2D CrispCircle;
        public static Texture2D EnergyShield;
        public static Texture2D Extra;
        public static Texture2D HollowCircleSoftEdge;
        public static Texture2D IceShard;
        public static Texture2D LargeTrail;
        public static Texture2D LightBubble;
        public static Texture2D LightField;
        public static Texture2D PlaceHolder;
        public static Texture2D Ribbon;
        public static Texture2D Ring1;
        public static Texture2D Ring2;
        public static Texture2D SmallTrail;
        public static Texture2D SpinnyNoise;
        public static Texture2D Ritual;
        public static Texture2D Ritual2;
        public static Texture2D Circle;
        public static Texture2D CircleBlack;
        public static Texture2D RandomSmoke;
        public static Texture2D Perlin;
        public static Texture2D Book;
        public static Texture2D LightFieldVert;
        public static Texture2D ReactionIcon;
        public static Texture2D Bee;
        public static Texture2D SmallBee;
        public static Texture2D TileCracks;
        public static Texture2D GoWRitual;
        public static Texture2D Lava;
        public static Texture2D SnowFlake;
        public static Texture2D Meteor3;
        public static Texture2D Crystal;
        public static Texture2D DaggerVert;
        public static Texture2D SlashWave;
        public static void Load()
        {
            FieldInfo[] f = typeof(TextureLibrary).GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (FieldInfo info in f)
            {
                if (info.FieldType == typeof(Texture2D))
                {
                    info.SetValue(null, ModContent.Request<Texture2D>("BG3MagicRework/Images/" + info.Name, AssetRequestMode.ImmediateLoad).Value);
                }
            }

            Main.instance.LoadProjectile(ProjectileID.VilethornBase);
            Main.instance.LoadProjectile(ProjectileID.VilethornTip);
        }

        public static void Unload()
        {
            FieldInfo[] f = typeof(TextureLibrary).GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (FieldInfo info in f)
            {
                if (info.FieldType == typeof(Texture2D))
                {
                    info.SetValue(null, null);
                }
            }
        }

    }
}
