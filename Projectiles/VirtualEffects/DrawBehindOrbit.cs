using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.VirtualEffects
{
    public class DrawBehindOrbit : ModProjectile
    {
        public override string Texture => "BG3MagicRework/Images/PlaceHolder";

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 99999;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (Projectile.ai[1] == 1)
            {
                behindNPCs.Add(index);
            }
        }

        public override void AI()
        {
            Projectile.hide = Projectile.ai[1] == 1;
            Projectile source = Main.projectile[(int)Projectile.ai[0]];
            if (!source.active)
            {
                Projectile.Kill();
                return;
            }
            if (source.ModProjectile == null || source.ModProjectile is not BaseDrawOrbit)
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = source.Center;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile source = Main.projectile[(int)Projectile.ai[0]];
            if (source.ModProjectile != null && source.ModProjectile is BaseDrawOrbit)
            {
                (source.ModProjectile as BaseDrawOrbit).DrawBehind(lightColor);
            }
            return false;
        }
    }
}