using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Ring3;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Spells.Ring3
{
    public class FearSpell : BaseSpell
    {
        public override string Name => "Fear";
        public override Color NameColor => Color.Purple;
        public override SchoolOfMagic Shool => SchoolOfMagic.Illusion;
        public override int InitialRing => 3;
        public override int AOERadius => 24;
        public override bool Concentration => true;
        public override int TimeSpan => 20;
        public override int DifficultyClass => 15;

        public override void Shoot(Player player, ModProjectile modproj, Vector2 tipPosition, Vector2 mousePosition, int Ring)
        {
            Vector2 ShootVel = Vector2.Normalize(mousePosition - tipPosition);
            int protmp = player.New1DmgMagicProj(tipPosition, ShootVel, ModContent.ProjectileType<FearProj>(), Ring);
            if (protmp >= 0 && protmp < 1000)
            {
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).ActivateMetaMagic(player, true, false, true, true, false);
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).DifficultyClass = player.GetDifficultyClass(Name) + Ring - 3;
            }

        }

        public override bool CanRelease(Player owner, ModProjectile modproj, Vector2 mousePosition, int Ring, ref string Warning)
        {
            bool success = true;
            if (mousePosition.Distance(owner.Center) > owner.GetSpellRange(Name) * 16)
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
            return false;
        }

        public override void DrawBehind(Player owner, int ring, float light, Color color, float scale, float miscTimer)
        {
            float rot = miscTimer * MathHelper.TwoPi;
            int dir = owner.direction;
        }


        public override bool ModifyDrawRangeInfo(Player player, int Ring)
        {
            float r = (Main.MouseWorld - player.Center).ToRotation();
            DrawUtils.DrawIndicatorFan(player.Center, player.GetAOERadius(Name) * 16, r - MathHelper.Pi / 3, r + MathHelper.Pi / 3);
            DrawUtils.DrawIndicatorLine(player.Center, player.Center + (r - MathHelper.Pi / 3f).ToRotationVector2() * player.GetAOERadius(Name) * 16);
            DrawUtils.DrawIndicatorLine(player.Center, player.Center + (r + MathHelper.Pi / 3f).ToRotationVector2() * player.GetAOERadius(Name) * 16);
            return false;
        }
    }
}
