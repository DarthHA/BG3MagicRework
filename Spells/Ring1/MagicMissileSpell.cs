using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Channel;
using BG3MagicRework.Projectiles.Ring1;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Spells.Ring1
{
    public class MagicMissileSpell : BaseSpell
    {
        public override string Name => "MagicMissile";
        public override Color NameColor => Color.IndianRed;
        public override SchoolOfMagic Shool => SchoolOfMagic.Evocation;
        public override int InitialRing => 1;
        public override DiceDamage BaseDamage => new DiceDamage(4, 1, DamageElement.Force, CombatStat.Ring1Damage) + new DiceDamage(1, CombatStat.Ring1Damage, DamageElement.Force);

        public override DiceDamage RisingDamageAddition => new();

        public override int SpellRange => 36;
        public override bool Concentration => false;
        public override int TimeSpan => 0;

        public override void Shoot(Player player, ModProjectile modproj, Vector2 tipPosition, Vector2 mousePosition, int Ring)
        {
            //间隔5帧发一发，初始3发，每升一环多一发
            int numProj = Ring + 2;
            (modproj as BaseStaffChannel).P4TimeLeft += 5 * (numProj - 1);

            int protmp = player.NewMagicProj(tipPosition, Vector2.Zero, ModContent.ProjectileType<MagicMissileController>(), player.GetDiceDamage(BaseDamage, InitialRing, Ring, RisingDamageAddition), 0, Ring);
            if (protmp >= 0 && protmp < 1000)
            {
                (Main.projectile[protmp].ModProjectile as MagicMissileController).RelaPos = tipPosition - player.Center;
                (Main.projectile[protmp].ModProjectile as MagicMissileController).numOfShoots = numProj;
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
            if (!Collision.CanHit(owner.position, owner.width, owner.height, mousePosition, 1, 1) && !owner.CarefulSpellMM())
            {
                Warning += LangLibrary.CannotSee + "\n";
            }
            return success;
        }


        public override bool DrawLight(Player owner, int ring, Vector2 tipPos, ref float light, ref Color color, ref float scale, float miscTimer, bool HasShot)
        {
            Texture2D texLight = TextureLibrary.BloomFlare;
            Texture2D texExtra = TextureLibrary.Extra;
            List<float> lines = new() { 0.52f, 1.04f, 2.54f, 2.04f };
            int timer = (int)(miscTimer * 100) % 160;
            int index = timer / 40;
            int timer2 = timer % 40;
            float scaleX;
            if (timer2 <= 20)
            {
                scaleX = MathHelper.Lerp(0, 1, timer2 / 20f);
            }
            else
            {
                scaleX = MathHelper.Lerp(1, 0, (timer2 - 20) / 20f);
            }
            Main.spriteBatch.Draw(texExtra, tipPos - Main.screenPosition, null, Color.Red * light, 0, texExtra.Size() / 2f, new Vector2(3, 0.5f) * scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texExtra, tipPos - Main.screenPosition, null, Color.Red * light, lines[index], texExtra.Size() / 2f, new Vector2(2.8f * scaleX, 0.5f) * scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texLight, tipPos - Main.screenPosition, null, Color.Red * light, miscTimer, texLight.Size() / 2f, 0.09f * scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texLight, tipPos - Main.screenPosition, null, Color.White * light * 1.3f, miscTimer, texLight.Size() / 2f, 0.06f * scale, SpriteEffects.None, 0);
            return false;
        }

    }
}
