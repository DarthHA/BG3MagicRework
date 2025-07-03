using BG3MagicRework.BaseType;
using BG3MagicRework.Concentrations;
using BG3MagicRework.Spells.Ring3;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring3
{
    public class HungerOfHadarShowProj : BaseMagicProj
    {
        public List<List<Vector2>> Tentacles = new();
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 99999;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
        }


        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.IsDead())
            {
                Projectile.Kill();
                return;
            }
            Projectile.ai[0]++;

            if (Projectile.ai[0] == 20)   //生成触手
            {
                int count = 10 + Main.rand.Next(10);
                for (int i = 0; i < count; i++)
                {
                    float iniRot = Main.rand.NextFloat() * MathHelper.TwoPi;
                    int rotDir = Main.rand.Next(2) * 2 - 1;
                    float rotDelta = (Main.rand.NextFloat() + 1) * MathHelper.TwoPi / 96f;
                    int length = Main.rand.Next(20) + 20;
                    List<Vector2> tentacle = new();
                    tentacle.Add(Projectile.Center);
                    Vector2 CurrentPos = Projectile.Center;
                    float CurrentRot = iniRot;
                    for (int j = 0; j < length; j++)
                    {
                        CurrentPos += CurrentRot.ToRotationVector2() * 15;
                        tentacle.Add(CurrentPos);
                        CurrentRot += rotDelta * MathHelper.Lerp(1, 3, j / (float)length) * rotDir;
                    }
                    Tentacles.Add(tentacle);
                }
            }

            if (Projectile.ai[0] == 30)
            {
                int protmp = owner.NewMagicProj(Projectile.Center, Vector2.Zero, ModContent.ProjectileType<HungerOfHadarDamageProj>(), diceDamage, 0, CurrentRing);
                if (protmp >= 0 && protmp < 1000)
                {
                    (Main.projectile[protmp].ModProjectile as BaseMagicProj).CopyMetaMagicFrom(this);
                    BaseConcentration con = owner.GenerateConcentration<ConHungerOfHadar>(CurrentRing, GetTimeSpan<HungerOfHadarSpell>() * 60, true);
                    if (con != null)
                    {
                        con.projIndex = protmp;
                        (Main.projectile[protmp].ModProjectile as BaseMagicProj).ConUUID = con.UUID;
                    }
                }

            }

            if (Projectile.ai[0] > 60) Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] < 50)
            {
                float light = 1;
                if (Projectile.ai[0] < 20)
                {
                    light = MathHelper.Lerp(0, 1, Projectile.ai[0] / 20f);
                }
                else if (Projectile.ai[0] > 40)
                {
                    light = MathHelper.Lerp(1, 0, (Projectile.ai[0] - 40) / 20f);
                }
                Texture2D texLight = TextureLibrary.BloomFlare;
                EasyDraw.AnotherDraw(DrawUtils.ReverseSubtract);
                Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.Red * light, 0, texLight.Size() / 2f, 0.2f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.White * light, 0, texLight.Size() / 2f, 0.15f, SpriteEffects.None, 0);
                EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            }
            if (Projectile.ai[0] >= 20)
            {
                float k = MathHelper.Lerp(0, 1, MathHelper.Clamp((Projectile.ai[0] - 20) / 20f, 0, 1));
                float light = MathHelper.Lerp(1, 0, MathHelper.Clamp((Projectile.ai[0] - 40) / 20f, 0, 1));
                float width = 5;
                foreach (List<Vector2> tentacle in Tentacles)
                {
                    List<CustomVertexInfo> bars = new();
                    int len = (int)(k * (tentacle.Count - 1)) + 1;
                    if (len > 1)
                    {
                        Vector2 UnitY = Vector2.Normalize(tentacle[1] - tentacle[0]).RotatedBy(MathHelper.Pi / 2f);
                        bars.Add(new CustomVertexInfo(tentacle[0] - UnitY * width - Main.screenPosition, Color.White, new Vector3(0, 0, 1f)));
                        bars.Add(new CustomVertexInfo(tentacle[0] + UnitY * width - Main.screenPosition, Color.White, new Vector3(0, 1, 1f)));
                        for (int i = 1; i < len; i++)
                        {
                            UnitY = Vector2.Normalize(tentacle[i] - tentacle[i - 1]).RotatedBy(MathHelper.Pi / 2f);
                            bars.Add(new CustomVertexInfo(tentacle[i] - UnitY * width * (1 - i / (float)(len - 1)) - Main.screenPosition, Color.White, new Vector3(i / (float)(len - 1), 0, 1f)));
                            bars.Add(new CustomVertexInfo(tentacle[i] + UnitY * width * (1 - i / (float)(len - 1)) - Main.screenPosition, Color.White, new Vector3(i / (float)(len - 1), 1, 1f)));
                        }
                        DrawUtils.DrawTrail(Terraria.GameContent.TextureAssets.MagicPixel.Value, bars, Color.White * light, DrawUtils.ReverseSubtract);
                    }
                }
            }
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }


    }
}
