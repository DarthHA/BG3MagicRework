using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.VirtualEffects
{
    public abstract class BaseDrawOrbit : ModProjectile
    {
        public override string Texture => "BG3MagicRework/Images/PlaceHolder";
        private bool Summoned = false;
        public virtual bool BehindNPCs => false;
        public sealed override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 99999;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.hide = true;
            Projectile.netImportant = true;
            SafeSetdefaults();
        }

        public virtual void SafeSetdefaults()
        {

        }

        public sealed override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public sealed override void AI()
        {
            if (!Summoned)
            {
                Summoned = true;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DrawBehindOrbit>(), 0, 0, Projectile.owner, Projectile.whoAmI, BehindNPCs ? 1 : 0);
            }
            SafeAI();
        }

        public virtual void SafeAI()
        {

        }

        public sealed override bool PreDraw(ref Color lightColor)
        {
            DrawFront(lightColor);
            return false;
        }

        public virtual void DrawFront(Color lightColor)
        {

        }

        public virtual void DrawBehind(Color lightColor)
        {

        }

        public bool IsFront(float r)
        {
            Vector2 vec = r.ToRotationVector2();
            return vec.Y >= 0;
        }
    }
}
