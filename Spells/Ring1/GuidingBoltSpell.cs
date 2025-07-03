using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Ring1;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Spells.Ring1
{
    public class GuidingBoltSpell : BaseSpell
    {
        public override string Name => "GuidingBolt";
        public override Color NameColor => Color.LightGoldenrodYellow;
        public override SchoolOfMagic Shool => SchoolOfMagic.Evocation;
        public override DiceDamage BaseDamage => new(6, 4, DamageElement.Radiant, CombatStat.Ring1Damage);

        public override DiceDamage RisingDamageAddition => new(6, 1, DamageElement.Radiant, CombatStat.Ring1Damage);
        public override int InitialRing => 1;
        public override int SpellRange => 36;
        public override bool Concentration => false;
        public override int TimeSpan => 6;

        public override void Shoot(Player player, ModProjectile modproj, Vector2 tipPosition, Vector2 mousePosition, int Ring)
        {
            Vector2 Vel = Vector2.Normalize(mousePosition - tipPosition) * 30f;
            int protmp = player.NewMagicProj(tipPosition, Vel, ModContent.ProjectileType<GuidingBoltProj>(), player.GetDiceDamage(BaseDamage, InitialRing, Ring, RisingDamageAddition), 0, Ring);
            if (protmp >= 0 && protmp < 1000)
            {
                Main.projectile[protmp].rotation = -MathHelper.Pi / 6 * player.direction;
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).ActivateMetaMagic(player, true, true, true, false, true);
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
            Texture2D texBubble = TextureLibrary.LightBubble;
            Texture2D texExtra = TextureLibrary.Extra;
            Texture2D LightTex = TextureLibrary.BloomFlare;
            if (!HasShot)
            {
                float r = -MathHelper.Pi / 6 * owner.direction;
                Main.spriteBatch.Draw(texExtra, tipPos - Main.screenPosition, null, Color.White * 0.8f * light, r, texExtra.Size() / 2f, new Vector2(2.5f, 0.2f) * scale, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texExtra, tipPos - Main.screenPosition, null, Color.White * 0.8f * light, r + MathHelper.Pi / 2f, texExtra.Size() / 2f, new Vector2(1.25f, 0.1f) * scale, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texBubble, tipPos - Main.screenPosition, null, Color.White * 0.8f * light, r, texBubble.Size() / 2f, 0.06f * scale, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texBubble, tipPos - Main.screenPosition, null, Color.White * 0.8f * light, r + MathHelper.Pi / 2f, texBubble.Size() / 2f, 0.04f * scale, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, Color.LightYellow * light, miscTimer, LightTex.Size() / 2f, 0.08f * scale, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, Color.White * light, miscTimer, LightTex.Size() / 2f, 0.04f * scale, SpriteEffects.None, 0);
            }
            else
            {
                float r = -MathHelper.Pi / 6 * owner.direction;
                Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, Color.LightYellow * light, miscTimer, LightTex.Size() / 2f, 0.08f * scale, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, Color.White * light, miscTimer, LightTex.Size() / 2f, 0.04f * scale, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}
