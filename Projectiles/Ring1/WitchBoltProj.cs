using BG3MagicRework.BaseType;
using BG3MagicRework.Concentrations;
using BG3MagicRework.Spells.Ring1;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring1
{
    public class WitchBoltProj : BaseMagicProj
    {
        public List<ArcSegments> Arcs = new();
        public List<ArcSegments> ArcSelf = new();
        public List<ArcSegments> ArcEnemy = new();
        public Vector2 RelaPos = Vector2.Zero;
        public int TargetNPC = -1;
        public Vector2 TargetRelaPos = Vector2.Zero;
        public float? FirstRot = null;

        public override int MaxHits => -1;
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.noEnchantmentVisuals = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.IsDead())
            {
                Projectile.Kill();
                return;
            }

            if (FirstRot == null) FirstRot = Projectile.velocity.ToRotation();
            Arcs.Clear();
            for (int i = 0; i < 2; i++)
            {
                ArcSegments arc = new();
                arc.GenerateSegs(owner.Center, Projectile.Center, new Vector2(40, 20), 30);
                Arcs.Add(arc);
            }
            if (++Projectile.localAI[0] <= 30)
            {
                ArcSelf.Clear();
                for (int i = 0; i < 3; i++)
                {
                    ArcSegments arc = new();
                    Vector2 EndPos = owner.Center + (FirstRot.Value + (Main.rand.NextFloat() * 2 - 1) * MathHelper.Pi / 5f).ToRotationVector2() * Main.rand.Next(10, 150);
                    arc.GenerateSegs(owner.Center, EndPos, new Vector2(40, 20), 20);
                    ArcSelf.Add(arc);
                }
            }
            else
            {
                ArcSelf.Clear();
            }

            if (Projectile.ai[0] == 0) //飞行状态
            {
                Projectile.ai[1]++;
                if ((Collision.SolidTiles(Projectile.position, Projectile.width, Projectile.height) && !CarefulSpellMM) || TravelDistance > GetSpellRange<WitchBoltSpell>() * 16f)
                {
                    Projectile.ai[0] = 1;
                    Projectile.ai[1] = 0;
                    Projectile.velocity = Vector2.Zero;
                }
            }
            else if (Projectile.ai[0] == 1)  //飞行撞到物块后滞留一段时间然后消失
            {
                Projectile.ai[1]++;
                Projectile.velocity = Vector2.Zero;
                if (Projectile.ai[1] > 20) Projectile.Kill();
            }
            else if (Projectile.ai[0] == 2)     //和目标channel了
            {
                //目标死亡会消失
                if (TargetNPC == -1 || (!Main.npc[TargetNPC].CanBeChasedBy() && !Main.npc[TargetNPC].immortal))
                {
                    Projectile.Kill();
                    return;
                }
                //断专注也会消失
                if (owner.GetConcentration(ConUUID) == -1)
                {
                    Projectile.Kill();
                    return;
                }
                //距离过远也会消失
                if (Main.npc[TargetNPC].Distance(owner.Center) > GetSpellRange<WitchBoltSpell>() * 16f * 1.5f)
                {
                    Projectile.Kill();
                    return;
                }
                ArcEnemy.Clear();

                for (int i = 0; i < 3; i++)
                {
                    if (Main.rand.NextBool(10))
                    {
                        ArcSegments arc = new();
                        Vector2 EndPos = Projectile.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(10, 150);
                        arc.GenerateSegs(Projectile.Center, EndPos, new Vector2(40, 20), 20);
                        ArcEnemy.Add(arc);
                    }
                }
                Projectile.Center = Main.npc[TargetNPC].Center + TargetRelaPos;
            }
        }


        public override void SafeOnHit(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] == 0)
            {
                TargetNPC = target.whoAmI;
                TargetRelaPos = Projectile.Center - target.Center;
                Projectile.ai[0] = 2;
                Projectile.ai[1] = 0;
                Projectile.velocity = Vector2.Zero;

                //专注链接
                Player owner = Main.player[Projectile.owner];
                BaseConcentration con = owner.GenerateConcentration<ConWitchBolt>(CurrentRing, GetTimeSpan<WitchBoltSpell>() * 60, true);
                if (con != null)
                {
                    con.projIndex = Projectile.whoAmI;
                    ConUUID = con.UUID;
                }
            }
        }

        public override bool? SafeCanHitNPC(NPC target)
        {
            if (Projectile.ai[0] == 1) return false;
            if (TargetNPC != -1 && TargetNPC != target.whoAmI) return false;
            return null;
        }

        public override void SafeModifyHit(NPC target, ref NPC.HitModifiers modifiers, ref DiceDamage diceUsed, ref float damageModifier, ref Dictionary<DamageElement, float> resistance)
        {
            if (Projectile.ai[0] == 2)
            {
                diceUsed = extraDiceDamage;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];
            foreach (ArcSegments arc in Arcs)
            {
                arc.DrawSegs(Color.Blue, 10);
            }
            foreach (ArcSegments arc in ArcSelf)
            {
                arc.DrawSegs(Color.Blue, 10);
            }
            foreach (ArcSegments arc in ArcEnemy)
            {
                arc.DrawSegs(Color.Blue, 10);
            }
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }
    }
}
