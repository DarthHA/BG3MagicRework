using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Dusts
{
    public class ZZZDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.velocity *= 0.95f;
            dust.scale *= 0.97f;
            if (dust.scale < 0.05f) dust.active = false;
            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Texture2D tex = ModContent.Request<Texture2D>("BG3MagicRework/Dusts/ZZZDust").Value;
            Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, Color.White, 0, tex.Size() / 2f, dust.scale, SpriteEffects.None, 0);
            return false;
        }

    }
}