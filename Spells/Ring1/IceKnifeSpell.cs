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
    public class IceKnifeSpell : BaseSpell
    {
        public override string Name => "IceKnife";
        public override Color NameColor => Color.Cyan;
        public override SchoolOfMagic Shool => SchoolOfMagic.Conjuration;
        public override int InitialRing => 1;
        public override DiceDamage BaseDamage => new(10, 1, DamageElement.None, CombatStat.Ring1Damage);
        public override DiceDamage RisingDamageAddition => new();
        public override DiceDamage ExtraDamage => new(6, 2, DamageElement.Cold, CombatStat.Ring1Damage);
        public override DiceDamage RisingDamageAdditionExtra => new(6, 1, DamageElement.Cold, CombatStat.Ring1Damage);
        public override int SpellRange => 36;
        public override int AOERadius => 5;
        public override bool Concentration => false;
        public override int TimeSpan => 12;
        public override int DifficultyClass => 12;

        public override void Shoot(Player player, ModProjectile modproj, Vector2 tipPosition, Vector2 mousePosition, int Ring)
        {
            Vector2 Vel = Vector2.Normalize(mousePosition - tipPosition) * 25f;
            int protmp = player.NewMagicProj(tipPosition, Vel, ModContent.ProjectileType<IceKnifeProj>(), player.GetDiceDamage(BaseDamage, InitialRing, Ring, RisingDamageAddition), 0, Ring);
            if (protmp >= 0 && protmp < 1000)
            {
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).extraDiceDamage = player.GetDiceDamage(ExtraDamage, InitialRing, Ring, RisingDamageAdditionExtra);
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).ActivateMetaMagic(player, true, true, false, false, true);
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).DifficultyClass = player.GetDifficultyClass(Name);
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
            Texture2D texRibbon = TextureLibrary.Ribbon;
            Texture2D LightTex = TextureLibrary.BloomFlare;
            List<CustomVertexInfo> bars1 = new()
                {
                new CustomVertexInfo(tipPos + new Vector2(-30,-15).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(0, 0f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(-30,15).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(0, 1f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(30,-15).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(1, 0f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(30,15).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(1, 1f, 1))
                };
            List<CustomVertexInfo> bars2 = new()
                {
                new CustomVertexInfo(tipPos + new Vector2(-15,-30).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(0, 0f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(-15,30).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(0, 1f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(15,-30).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(1, 0f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(15,30).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(1, 1f, 1))
                };
            DrawUtils.DrawRoSLaser(texRibbon, bars1, Color.White * light, 0.4f, 1f, miscTimer, BlendState.Additive);
            DrawUtils.DrawRoSLaser(texRibbon, bars2, Color.White * light, 0.4f, 1f, miscTimer, BlendState.Additive);
            EasyDraw.AnotherDraw(BlendState.Additive);
            Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, color * light, miscTimer, LightTex.Size() / 2f, 0.04f * scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, Color.White * light, miscTimer, LightTex.Size() / 2f, 0.02f * scale, SpriteEffects.None, 0);
            return false;
        }

    }

}
