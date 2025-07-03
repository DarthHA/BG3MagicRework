using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Cantrips;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Spells.Cantrips
{
    public class FireBoltSpell : BaseSpell
    {
        public override string Name => "FireBolt";
        public override Color NameColor => Color.OrangeRed;
        public override SchoolOfMagic Shool => SchoolOfMagic.Evocation;
        public override DiceDamage BaseDamage => new(10, 1, DamageElement.Fire, CombatStat.CantripDamage);
        public override DiceDamage RisingDamageAddition => new(10, 1, DamageElement.Fire, CombatStat.CantripDamage);
        public override int InitialRing => 0;
        public override int SpellRange => 36;
        public override bool Concentration => false;
        public override int TimeSpan => 0;

        public override void Shoot(Player player, ModProjectile modproj, Vector2 tipPosition, Vector2 mousePosition, int Ring)
        {
            Vector2 Vel = Vector2.Normalize(mousePosition - tipPosition) * 15f;
            int protmp = player.NewMagicProj(tipPosition, Vel, ModContent.ProjectileType<FireBoltProj>(), player.GetDiceDamage(BaseDamage, InitialRing, InitialRing, RisingDamageAddition));
            if (protmp >= 0 && protmp < 1000)
            {
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).ActivateMetaMagic(player, true, true, false, false, true);
            }
        }

        public override bool CanRelease(Player owner, ModProjectile modproj, Vector2 mousePosition, int Ring, ref string Warning)
        {
            bool success = true;
            if (mousePosition.Distance(owner.Center) > owner.GetSpellRange(Name) * 16)
            {
                Warning += LangLibrary.OutOfRange + "\n";
            }
            if (!owner.CarefulSpellMM() && !Collision.CanHit(owner.position, owner.width, owner.height, mousePosition, 1, 1))
            {
                Warning += LangLibrary.CannotSee + "\n";
            }
            return success;
        }
    }
}
