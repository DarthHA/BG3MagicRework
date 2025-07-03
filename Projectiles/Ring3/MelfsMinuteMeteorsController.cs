using BG3MagicRework.BaseType;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring3
{
    public class MelfsMinuteMeteorsController : BaseMagicProj
    {
        public int numOfShots = 6;
        public List<MMMeteor> mmMeteors = new();
        /// <summary>
        /// 这是一个动态量，用于决定发射多少流星
        /// </summary>
        public int ReleaseCount = 0;
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
            Projectile.Center = owner.Center;
            Projectile.localAI[0]++;
            if (Projectile.ai[0] == 0)       //装填状态
            {
                Projectile.ai[0] = 1;
                List<int> ABCCount = new() { 0, 0, 0 };
                for (int i = 0; i < numOfShots; i++)
                {
                    ABCCount[i % 3]++;
                }
                for (int i = 0; i < ABCCount.Count; i++)
                {
                    float rotX = (i - 1) * -MathHelper.Pi / 3f;
                    float rotYOffset = Main.rand.NextFloat() * MathHelper.TwoPi;
                    for (int j = 0; j < ABCCount[i]; j++)
                    {
                        float rotY = j / (float)ABCCount[i] * MathHelper.TwoPi + rotYOffset;
                        mmMeteors.Add(new(rotX, rotY, (i % 3 == 1 ? 1 : -1) * owner.direction, Main.rand.Next(3)));
                    }
                }
            }
            else if (Projectile.ai[0] == 1)    //准备发射
            {
                if (Projectile.ai[1] < 20) Projectile.ai[1]++;

                if (owner.GetConcentration(ConUUID) == -1)                //断专注
                {
                    Projectile.ai[0] = 2;
                    return;
                }

                foreach (MMMeteor mmMeteor in mmMeteors)
                {
                    mmMeteor.RotY += MathHelper.Pi / 100f * mmMeteor.RotDir;

                    Vector2 RotVec = mmMeteor.RotY.ToRotationVector2() * 100;
                    RotVec.Y *= 0.5f;
                    mmMeteor.Pos = RotVec.RotatedBy(mmMeteor.RotX);

                    Vector2 vel = mmMeteor.RotY.ToRotationVector2().RotatedBy(MathHelper.Pi / 2 * mmMeteor.RotDir);
                    vel.Y *= 0.5f;
                    mmMeteor.UnitX = Vector2.Normalize(vel.RotatedBy(mmMeteor.RotX));

                    mmMeteor.Trails.Add(mmMeteor.Pos);
                    if (mmMeteor.Trails.Count > 12)
                    {
                        mmMeteor.Trails.RemoveAt(0);
                    }
                }

                if (ReleaseCount > 0 && mmMeteors.Count > 0)
                {
                    int index = Main.rand.Next(mmMeteors.Count);
                    Vector2 Vel = Vector2.Normalize(Main.MouseWorld - (Projectile.Center + mmMeteors[index].Pos)) * 2;
                    int protmp = owner.NewMagicProj(Projectile.Center + mmMeteors[index].Pos, Vel, ModContent.ProjectileType<MelfsMinuteMeteorsShot>(), diceDamage, Projectile.knockBack, CurrentRing);
                    if (protmp >= 0 && protmp < 1000)
                    {
                        (Main.projectile[protmp].ModProjectile as BaseMagicProj).CopyMetaMagicFrom(this);
                        (Main.projectile[protmp].ModProjectile as MelfsMinuteMeteorsShot).shotFrame = mmMeteors[index].Frame;
                    }
                    mmMeteors.RemoveAt(index);
                    ReleaseCount--;
                }
                else
                {
                    CarefulSpellMM = false;
                    DistantSpellMM = false;
                    TwinnedSpellMM = false;
                }
                if (mmMeteors.Count <= 0)
                {
                    Projectile.Kill();
                }
            }
            else if (Projectile.ai[0] == 2)       //非发射完的消失
            {
                if (Projectile.ai[1] > 0)
                {
                    Projectile.ai[1]--;
                }
                else
                {
                    Projectile.Kill();
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureLibrary.Meteor3;
            Texture2D texLight = TextureLibrary.BloomFlare;

            float light0 = 1;
            if (Projectile.ai[0] == 1)
            {
                light0 = MathHelper.Lerp(0, 1, Projectile.ai[1] / 20f);
            }
            else if (Projectile.ai[0] == 2)
            {
                light0 = MathHelper.Lerp(1, 0, MathHelper.Clamp((20 - Projectile.ai[1]) / 10f, 0, 1));
            }

            foreach (MMMeteor mmMeteor in mmMeteors)
            {
                int width = 13;
                if (mmMeteor.Trails.Count > 1)
                {
                    List<CustomVertexInfo> bars1 = new();
                    Vector2 UnitX = mmMeteor.UnitX;
                    Vector2 UnitY = UnitX.RotatedBy(MathHelper.Pi / 2f);
                    bars1.Add(new CustomVertexInfo(Projectile.Center + mmMeteor.Pos + UnitX * width - UnitY * width - Main.screenPosition, Color.White, new Vector3(0, 0, 1f)));
                    bars1.Add(new CustomVertexInfo(Projectile.Center + mmMeteor.Pos + UnitX * width + UnitY * width - Main.screenPosition, Color.White, new Vector3(0, 1, 1f)));
                    for (int i = mmMeteor.Trails.Count - 1; i >= 0; i--)
                    {
                        UnitX = -mmMeteor.UnitX;
                        if (i != mmMeteor.Trails.Count - 1 && mmMeteor.Trails[i] != mmMeteor.Trails[i + 1])
                        {
                            UnitX = Vector2.Normalize(mmMeteor.Trails[i] - mmMeteor.Trails[i + 1]);
                        }
                        UnitY = UnitX.RotatedBy(MathHelper.Pi / 2f);
                        bars1.Add(new CustomVertexInfo(Projectile.Center + mmMeteor.Trails[i] + UnitY * width - Main.screenPosition, Color.White, new Vector3(1f - (float)i / mmMeteor.Trails.Count, 0, 1f)));
                        bars1.Add(new CustomVertexInfo(Projectile.Center + mmMeteor.Trails[i] - UnitY * width - Main.screenPosition, Color.White, new Vector3(1f - (float)i / mmMeteor.Trails.Count, 1, 1f)));
                    }
                    DrawUtils.DrawTrail(TextureLibrary.BlobGlow2, bars1, Color.Orange * light0, BlendState.Additive);
                }
            }

            foreach (MMMeteor mmMeteor in mmMeteors)
            {
                Rectangle rect = new(0, tex.Height / 3 * mmMeteor.Frame, tex.Width, tex.Height / 3);
                Main.spriteBatch.Draw(tex, Projectile.Center + mmMeteor.Pos - Main.screenPosition
                    , rect, Color.White * light0,
                    -Projectile.localAI[0] / 200f * MathHelper.TwoPi * mmMeteor.RotDir, rect.Size() / 2f, 0.5f, SpriteEffects.None, 0);
            }

            if (Projectile.ai[0] == 1)
            {
                if (Projectile.ai[1] < 20)
                {
                    float scale = 1;
                    float light = 1;
                    if (Projectile.ai[1] <= 5)
                    {
                        scale = MathHelper.Lerp(0, 1, Projectile.ai[1] / 5f);
                    }
                    else
                    {
                        light = MathHelper.Lerp(1, 0, (Projectile.ai[1] - 5) / 15f);
                    }
                    EasyDraw.AnotherDraw(BlendState.Additive);
                    foreach (MMMeteor mmMeteor in mmMeteors)
                    {
                        Main.spriteBatch.Draw(texLight, Projectile.Center + mmMeteor.Pos - Main.screenPosition, null, Color.Orange * light, 0, texLight.Size() / 2f, 0.05f * scale, SpriteEffects.None, 0);
                        Main.spriteBatch.Draw(texLight, Projectile.Center + mmMeteor.Pos - Main.screenPosition, null, Color.White * light, 0, texLight.Size() / 2f, 0.04f * scale, SpriteEffects.None, 0);
                    }
                    EasyDraw.AnotherDraw(BlendState.AlphaBlend);
                }
            }
            else if (Projectile.ai[0] == 2)
            {
                if (Projectile.ai[1] < 20)
                {
                    float scale = 1;
                    float light = 1;
                    if (Projectile.ai[1] >= 15)
                    {
                        scale = MathHelper.Lerp(0, 1, (20 - Projectile.ai[1]) / 5f);
                    }
                    else
                    {
                        light = MathHelper.Lerp(1, 0, (15 - Projectile.ai[1]) / 15f);
                    }
                    EasyDraw.AnotherDraw(BlendState.Additive);
                    foreach (MMMeteor mmMeteor in mmMeteors)
                    {
                        Main.spriteBatch.Draw(texLight, Projectile.Center + mmMeteor.Pos - Main.screenPosition, null, Color.Orange * light, 0, texLight.Size() / 2f, 0.05f * scale, SpriteEffects.None, 0);
                        Main.spriteBatch.Draw(texLight, Projectile.Center + mmMeteor.Pos - Main.screenPosition, null, Color.White * light, 0, texLight.Size() / 2f, 0.04f * scale, SpriteEffects.None, 0);
                    }
                    EasyDraw.AnotherDraw(BlendState.AlphaBlend);
                }
            }

            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public class MMMeteor(float rotX, float rotY, int dir, int frame)
        {
            public float RotX = rotX;
            public float RotY = rotY;
            public float RotDir = dir;
            public int Frame = frame;
            public List<Vector2> Trails = new();
            public Vector2 Pos;
            public Vector2 UnitX;
        }
    }
}
