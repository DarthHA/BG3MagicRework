using BG3MagicRework.BaseType;
using BG3MagicRework.Buffs.Enemy;
using BG3MagicRework.Projectiles.Ring1;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Spells.Ring1
{
    public class SleepSpell : BaseSpell
    {
        public override string Name => "Sleep";
        public override Color NameColor => Color.Pink;
        public override SchoolOfMagic Shool => SchoolOfMagic.Enchantment;
        public override int InitialRing => 1;
        public override int SpellRange => 36;
        public override int DifficultyClass => 15;
        public override bool Concentration => false;
        public override int TimeSpan => 12;

        public override void Shoot(Player player, ModProjectile modproj, Vector2 tipPosition, Vector2 mousePosition, int Ring)
        {
            (bool careful, bool distant, bool extended, bool heightened, _) = player.ActivateMetaMagic(true, true, true, true, false);
            List<int> targets = CCEffectUtils.FindMultipleEnemyCC(player.Center, SpellRange * 16 * (distant ? 2 : 1), Ring, careful);
            for (int i = 0; i < targets.Count; i++)
            {
                int protmp = player.NewMagicProj(Main.npc[targets[i]].Bottom, Vector2.Zero, ModContent.ProjectileType<SleepProj>(), Ring);
                if (protmp >= 0 && protmp < 1000)
                {
                    (Main.projectile[protmp].ModProjectile as BaseMagicProj).CarefulSpellMM = careful;
                    (Main.projectile[protmp].ModProjectile as BaseMagicProj).DistantSpellMM = distant;
                    (Main.projectile[protmp].ModProjectile as BaseMagicProj).ExtendedSpellMM = extended;
                    (Main.projectile[protmp].ModProjectile as BaseMagicProj).HeightenedSpellMM = heightened;
                    (Main.projectile[protmp].ModProjectile as BaseMagicProj).DifficultyClass = player.GetDifficultyClass(Name) + (Ring - 1);
                    (Main.projectile[protmp].ModProjectile as BaseMagicProj).DeepAddCCBuffByDC(Main.npc[targets[i]], ModContent.BuffType<SleepDNDBuff>(),
                        (Main.projectile[protmp].ModProjectile as BaseMagicProj).GetTimeSpan<SleepSpell>() * 60);
                }
            }
            /*
            foreach(NPC npc in Main.ActiveNPCs)
            {
                if (npc.GetInstantSource() != -1)
                {
                    Main.NewText($"{Lang.GetNPCNameValue(npc.type)} 的同帧起源是 {Lang.GetNPCNameValue(Main.npc[npc.GetInstantSource()].type)}");
                }
            }
            */
        }

        public override bool CanRelease(Player owner, ModProjectile modproj, Vector2 mousePosition, int Ring, ref string Warning)
        {
            bool success = true;
            if (CCEffectUtils.FindMultipleEnemyCC(owner.Center, owner.GetSpellRange(Name) * 16, 1, owner.CarefulSpellMM()).Count == 0)
            {
                success = false;
                Warning += LangLibrary.NoTarget + "\n";
            }
            return success;
        }

        public override bool DrawLight(Player owner, int ring, Vector2 tipPos, ref float light, ref Color color, ref float scale, float miscTimer, bool HasShot)
        {
            tipPos = owner.Center + (tipPos - owner.Center) * 0.85f;

            Texture2D texRibbon = TextureLibrary.Ribbon;
            Texture2D LightTex = TextureLibrary.BloomFlare;

            List<CustomVertexInfo> bars1 = new()
                {
                new CustomVertexInfo(tipPos + new Vector2(-30,-30).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(0, 0f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(-30,30).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(0, 1f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(30,-30).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(1, 0f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(30,30).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(1, 1f, 1))
                };
            DrawUtils.DrawRoSLaser(texRibbon, bars1, Color.Purple * light, 0.4f, 1f, miscTimer, BlendState.Additive);

            for (int i = 0; i < 3; i++)
            {
                List<CustomVertexInfo> bars2 = new();
                for (int k = 0; k < 10; k++)
                {
                    float x = MathHelper.Lerp(-30, 0, k / 9f);
                    float deltaY = 5 * (float)Math.Sin(miscTimer * 8 + k / 10f * MathHelper.TwoPi);
                    bars2.Add(new CustomVertexInfo(tipPos + new Vector2(x, -5 + deltaY).RotatedBy(MathHelper.TwoPi / 3f * i + miscTimer) - Main.screenPosition, Color.White, new Vector3(k / 9f, 0f, 1)));
                    bars2.Add(new CustomVertexInfo(tipPos + new Vector2(x, 5 + deltaY).RotatedBy(MathHelper.TwoPi / 3f * i + miscTimer) - Main.screenPosition, Color.White, new Vector3(k / 9f, 1f, 1)));
                }
                DrawUtils.DrawRoSLaser(texRibbon, bars2, Color.Purple * light * 1.3f, 0.4f, 1f, miscTimer + 0.4f * i, BlendState.Additive);
            }

            for (int i = 0; i < 3; i++)
            {
                List<CustomVertexInfo> bars3 = new();
                for (int k = 0; k < 10; k++)
                {
                    float x = MathHelper.Lerp(-25, 0, k / 9f);
                    float deltaY = 5 * (float)Math.Sin(miscTimer * 8 + k / 10f * MathHelper.TwoPi);
                    bars3.Add(new CustomVertexInfo(tipPos + new Vector2(x, -3 + deltaY).RotatedBy(MathHelper.TwoPi / 3f * i + miscTimer) - Main.screenPosition, Color.White, new Vector3(k / 9f, 0f, 1)));
                    bars3.Add(new CustomVertexInfo(tipPos + new Vector2(x, 3 + deltaY).RotatedBy(MathHelper.TwoPi / 3f * i + miscTimer) - Main.screenPosition, Color.White, new Vector3(k / 9f, 1f, 1)));
                }
                DrawUtils.DrawRoSLaser(texRibbon, bars3, Color.White * light * 1.3f, 0.4f, 1f, miscTimer + 0.4f * i, BlendState.Additive);
            }

            EasyDraw.AnotherDraw(BlendState.Additive);
            Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, Color.Purple * light * 1.3f, miscTimer, LightTex.Size() / 2f, 0.04f * scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, Color.White * light, miscTimer, LightTex.Size() / 2f, 0.02f * scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
