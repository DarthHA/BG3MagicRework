using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Ring1;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Spells.Ring1
{
    public class FaerieFireSpell : BaseSpell
    {
        public override string Name => "FaerieFire";
        public override Color NameColor => Color.Purple;
        public override SchoolOfMagic Shool => SchoolOfMagic.Evocation;
        public override int InitialRing => 1;
        public override int SpellRange => 36;
        public override int AOERadius => 24;
        public override bool Concentration => true;
        public override int TimeSpan => 60;
        public override int DifficultyClass => 15;

        public override void Shoot(Player player, ModProjectile modproj, Vector2 tipPosition, Vector2 mousePosition, int Ring)
        {
            int protmp = player.New1DmgMagicProj(mousePosition, Vector2.Zero, ModContent.ProjectileType<FaerieFireProj>(), Ring);
            if (protmp >= 0 && protmp < 1000)
            {
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).ActivateMetaMagic(player, true, true, true, true, false);
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).DifficultyClass = player.GetDifficultyClass(Name) + Ring - 1;
                Vector2 TargetPosition;
                if ((Main.projectile[protmp].ModProjectile as BaseMagicProj).CarefulSpellMM)
                {
                    TargetPosition = SomeUtils.GetNoBlockEndPos(player.Center, mousePosition, (Main.projectile[protmp].ModProjectile as BaseMagicProj).GetSpellRange(Name) * 16);
                }
                else
                {
                    TargetPosition = SomeUtils.GetTileBlockedEndPos(player.Center, mousePosition, (Main.projectile[protmp].ModProjectile as BaseMagicProj).GetSpellRange(Name) * 16);
                }
                Main.projectile[protmp].Center = TargetPosition;
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


    }
}
