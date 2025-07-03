using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Ring1;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Spells.Ring1
{
    public class ThunderWaveSpell : BaseSpell
    {
        public override string Name => "ThunderWave";
        public override Color NameColor => Color.Purple;
        public override SchoolOfMagic Shool => SchoolOfMagic.Evocation;
        public override int InitialRing => 1;
        public override DiceDamage BaseDamage => new(8, 2, DamageElement.Thunder, CombatStat.Ring1Damage);
        public override DiceDamage RisingDamageAddition => new(8, 1, DamageElement.Thunder, CombatStat.Ring1Damage);
        public override int AOERadius => 18;
        public override bool Concentration => false;
        public override int TimeSpan => 0;

        public override void Shoot(Player player, ModProjectile modproj, Vector2 tipPosition, Vector2 mousePosition, int Ring)
        {
            Vector2 vel = Vector2.Normalize(mousePosition - tipPosition);
            int protmp = player.NewMagicProj(tipPosition, vel, ModContent.ProjectileType<ThunderWaveProj>(), player.GetDiceDamage(BaseDamage, InitialRing, Ring, RisingDamageAddition), 0, Ring);
            if (protmp >= 0 && protmp < 1000)
            {
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).CurrentRing = Ring;
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).ActivateMetaMagic(player, true, false, false, false, true);
            }
        }

        public override bool CanRelease(Player owner, ModProjectile modproj, Vector2 mousePosition, int Ring, ref string Warning)
        {
            bool success = true;
            if (mousePosition.Distance(owner.Center) > owner.GetAOERadius(Name) * 16)
            {
                Warning += LangLibrary.OutOfRange + "\n";
            }
            if (!Collision.CanHit(owner.position, owner.width, owner.height, mousePosition, 1, 1) && !owner.CarefulSpellMM())
            {
                Warning += LangLibrary.CannotSee + "\n";
            }
            return success;
        }

        public override bool DrawLight(Player owner, int ring, Vector2 tipPos, ref float light, ref Color color, ref float scale, float miscTimer, bool HasShot)
        {
            Draw710(tipPos, 40, -miscTimer * 4, Color.White * light * 0.8f, miscTimer);
            return false;
        }

        public void Draw710(Vector2 Center, float radius, float progress, Color color, float rot = 0)
        {
            List<CustomVertexInfo> bars = new();
            for (int i = 0; i <= 240; i++)
            {
                float r = i * MathHelper.TwoPi / 240f + rot;
                Vector2 Pos1 = r.ToRotationVector2() * 1;
                Vector2 Pos2 = r.ToRotationVector2() * radius;
                bars.Add(new CustomVertexInfo(Center + Pos1 - Main.screenPosition, Color.White, new Vector3(0, i / 240f, 1f)));
                bars.Add(new CustomVertexInfo(Center + Pos2 - Main.screenPosition, Color.White, new Vector3(1, i / 240f, 1f)));
            }
            DrawUtils.DrawRoSLaser(TextureLibrary.Perlin, bars, color, 0.4f, 2f, progress, BlendState.Additive);
        }

        public override bool ModifyDrawRangeInfo(Player player, int Ring)
        {
            float r = (Main.MouseWorld - player.Center).ToRotation();
            DrawUtils.DrawIndicatorFan(player.Center, player.GetAOERadius(Name) * 16, r - MathHelper.Pi / 2, r + MathHelper.Pi / 2);
            DrawUtils.DrawIndicatorLine(player.Center, player.Center + (r - MathHelper.Pi / 2f).ToRotationVector2() * player.GetAOERadius(Name) * 16);
            DrawUtils.DrawIndicatorLine(player.Center, player.Center + (r + MathHelper.Pi / 2f).ToRotationVector2() * player.GetAOERadius(Name) * 16);
            return false;
        }
    }

}