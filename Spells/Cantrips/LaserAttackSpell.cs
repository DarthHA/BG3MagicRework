using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Cantrips;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Spells.Cantrips
{
    public class LaserAttackSpell : BaseSpell
    {
        public override string Name => "LaserAttack";
        public override Color NameColor => Color.Green;
        public override SchoolOfMagic Shool => SchoolOfMagic.Evocation;
        public override DiceDamage BaseDamage => new(8, 1, DamageElement.Force, CombatStat.GunCantripDamage);
        public override DiceDamage RisingDamageAddition => new(8, 1, DamageElement.Force, CombatStat.GunCantripDamage);
        public override int InitialRing => 0;
        public override int SpellRange => 36;
        public override bool Concentration => false;
        public override int TimeSpan => 0;

        public override void Shoot(Player player, ModProjectile modproj, Vector2 tipPosition, Vector2 mousePosition, int Ring)
        {
            Vector2 Vel = Vector2.Normalize(mousePosition - tipPosition) * 20f;
            int protmp = player.NewMagicProj(tipPosition, Vel, ModContent.ProjectileType<LaserAttackProj>(), player.GetDiceDamage(BaseDamage, InitialRing, InitialRing, RisingDamageAddition));
            if (protmp >= 0 && protmp < 1000)
            {
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).ActivateMetaMagic(player, true, true, false, false, true);
            }
        }

        public override bool DrawLight(Player owner, int ring, Vector2 tipPos, ref float light, ref Color color, ref float scale, float miscTimer, bool HasShot)
        {
            return true;
        }

        public override bool CanRelease(Player owner, ModProjectile modproj, Vector2 mousePosition, int Ring, ref string Warning)
        {
            bool success = true;
            if (mousePosition.Distance(owner.Center) > owner.GetSpellRange(Name) * 16)
            {
                Warning += LangLibrary.OutOfRange + "\n";
            }
            return success;
        }
    }
}
