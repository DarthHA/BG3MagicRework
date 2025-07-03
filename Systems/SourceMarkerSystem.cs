using BG3MagicRework.BaseType;
using BG3MagicRework.Buffs.Enemy;
using BG3MagicRework.Static;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Systems
{
    public class SourceMarkerSystem : ModSystem
    {
        public static int CurrentLastSource = -1;
        public static int CurrentRootSource = -1;
        public static int CurrentInstantSource = -1;
        public static int SavedRootSource = -1;
        public static int SavedLastSource = -1;

        public override void Load()
        {
            On_NPC.UpdateNPC += UpdateNPCHook;
            On_Projectile.Update += UpdateProjHook;
            On_NPC.checkDead += CheckDeadHook;
            On_Projectile.Kill += ProjKillHook;
            On_NPC.HitEffect_HitInfo += HitEffectHook;

            On_NPC.NewNPC += NewNPCHook;
            On_Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float += NewProjectileHook;

        }



        public override void Unload()
        {
            On_NPC.UpdateNPC -= UpdateNPCHook;
            On_Projectile.Update -= UpdateProjHook;
            On_NPC.checkDead -= CheckDeadHook;
            On_Projectile.Kill -= ProjKillHook;
            On_NPC.HitEffect_HitInfo -= HitEffectHook;

            On_NPC.NewNPC -= NewNPCHook;
            On_Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float -= NewProjectileHook;

        }

        internal static void CheckDeadHook(On_NPC.orig_checkDead orig, NPC self)
        {
            SavedRootSource = CurrentRootSource;
            SavedLastSource = CurrentLastSource;

            int sourceRoot = self.GetRootSource();
            if (sourceRoot == -1)
            {
                CurrentRootSource = self.whoAmI;
            }
            else
            {
                if (!Main.npc[sourceRoot].active)
                {
                    if (self.TryGetGlobalNPC(out SourceMarkNPC markNPC))
                    {
                        markNPC.RootSource = -1;
                    }
                    CurrentRootSource = -1;
                }
                else
                {
                    CurrentRootSource = sourceRoot;
                }
            }

            CurrentLastSource = self.whoAmI;

            orig.Invoke(self);
            CurrentRootSource = SavedRootSource;
            CurrentLastSource = SavedLastSource;
            SavedRootSource = -1;
            SavedLastSource -= 1;
        }

        private void HitEffectHook(On_NPC.orig_HitEffect_HitInfo orig, NPC self, NPC.HitInfo hit)
        {
            SavedRootSource = CurrentRootSource;
            SavedLastSource = CurrentLastSource;

            int source = self.GetRootSource();
            if (source == -1)
            {
                CurrentRootSource = self.whoAmI;
            }
            else
            {
                if (!Main.npc[source].active)
                {
                    if (self.TryGetGlobalNPC(out SourceMarkNPC markNPC))
                    {
                        markNPC.RootSource = -1;
                    }
                    CurrentRootSource = -1;
                }
                else
                {
                    CurrentRootSource = source;
                }
            }
            CurrentLastSource = self.whoAmI;
            orig.Invoke(self, hit);
            CurrentRootSource = SavedRootSource;
            SavedRootSource = -1;
            CurrentLastSource = SavedLastSource;
            SavedLastSource = -1;
        }


        internal static void ProjKillHook(On_Projectile.orig_Kill orig, Projectile self)
        {
            SavedRootSource = CurrentRootSource;
            SavedLastSource = CurrentLastSource;

            int sourceRoot = self.GetRootSource();
            if (sourceRoot != -1)
            {
                if (!Main.npc[sourceRoot].active)
                {
                    if (self.TryGetGlobalProjectile(out SourceMarkProj markProj))
                    {
                        markProj.RootSource = -1;
                    }
                    CurrentRootSource = -1;
                }
                else
                {
                    CurrentRootSource = sourceRoot;
                }
            }
            else
            {
                CurrentRootSource = -1;
            }

            int sourceLast = self.GetLastSource();
            if (sourceLast != -1)
            {
                if (!Main.npc[sourceLast].active)
                {
                    if (self.TryGetGlobalProjectile(out SourceMarkProj markProj))
                    {
                        markProj.LastSource = -1;
                    }
                    CurrentLastSource = -1;
                }
                else
                {
                    CurrentLastSource = sourceLast;
                }
            }
            else
            {
                CurrentLastSource = -1;
            }


            orig.Invoke(self);
            CurrentRootSource = SavedRootSource;
            SavedRootSource = -1;
            CurrentLastSource = SavedLastSource;
            SavedLastSource = -1;
        }

        internal static void UpdateNPCHook(On_NPC.orig_UpdateNPC orig, NPC npc, int i)
        {
            if (!npc.active)
            {
                CurrentRootSource = -1;
                CurrentLastSource = -1;
                CurrentInstantSource = -1;
                orig.Invoke(npc, i);
                return;
            }


            int rootSource = npc.GetRootSource();
            if (rootSource == -1)
            {
                CurrentRootSource = npc.whoAmI;
            }
            else
            {
                if (!Main.npc[rootSource].active)
                {
                    if (npc.TryGetGlobalNPC(out SourceMarkNPC markNPC))
                    {
                        markNPC.RootSource = -1;
                    }
                    CurrentRootSource = -1;
                }
                else
                {
                    CurrentRootSource = rootSource;
                }
            }
            CurrentLastSource = npc.whoAmI;
            if (npc.TryGetGlobalNPC(out SourceMarkNPC result))
            {
                if (result.InstantTimer > 0)
                {
                    int instantSource = npc.GetInstantSource();
                    if (instantSource == -1)
                    {
                        CurrentInstantSource = npc.whoAmI;
                    }
                    else
                    {
                        if (!Main.npc[instantSource].active)
                        {
                            if (npc.TryGetGlobalNPC(out SourceMarkNPC markNPC))
                            {
                                markNPC.InstantSource = -1;
                            }
                            CurrentInstantSource = -1;
                        }
                        else
                        {
                            CurrentInstantSource = rootSource;
                        }
                    }
                }
            }
            orig.Invoke(npc, i);
            CurrentRootSource = -1;
            CurrentLastSource = -1;
            CurrentInstantSource = -1;
        }

        internal static void UpdateProjHook(On_Projectile.orig_Update orig, Projectile proj, int i)
        {
            if (!proj.active)
            {
                CurrentRootSource = -1;
                CurrentLastSource = -1;
                orig.Invoke(proj, i);
                return;
            }

            int sourceRoot = proj.GetRootSource();
            if (sourceRoot != -1)
            {
                if (!Main.npc[sourceRoot].active)
                {
                    if (proj.TryGetGlobalProjectile(out SourceMarkProj markProj))
                    {
                        markProj.RootSource = -1;
                    }
                    CurrentRootSource = -1;
                }
                else
                {
                    CurrentRootSource = sourceRoot;
                }
            }
            else
            {
                CurrentRootSource = -1;
            }

            int sourceLast = proj.GetLastSource();
            if (sourceLast != -1)
            {
                if (!Main.npc[sourceLast].active)
                {
                    if (proj.TryGetGlobalProjectile(out SourceMarkProj markProj))
                    {
                        markProj.LastSource = -1;
                    }
                    CurrentLastSource = -1;
                }
                else
                {
                    CurrentLastSource = sourceLast;
                }
            }
            else
            {
                CurrentLastSource = -1;
            }

            orig.Invoke(proj, i);
            CurrentRootSource = -1;
            CurrentLastSource = -1;
        }

        internal static int NewNPCHook(On_NPC.orig_NewNPC orig, IEntitySource source, int X, int Y, int Type, int Start = 0, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f, float ai3 = 0f, int Target = 255)
        {
            int result = orig.Invoke(source, X, Y, Type, Start, ai0, ai1, ai2, ai3, Target);
            if (result >= 0 && result < 200)
            {
                if (Main.npc[result].TryGetGlobalNPC(out SourceMarkNPC markNPC))
                {
                    markNPC.RootSource = CurrentRootSource;
                    markNPC.LastSource = CurrentLastSource;
                    markNPC.InstantSource = CurrentInstantSource;
                }
            }
            return result;
        }
        internal static int NewProjectileHook(On_Projectile.orig_NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float orig, IEntitySource spawnSource, float X, float Y, float SpeedX, float SpeedY, int Type, int Damage, float KnockBack, int Owner, float ai0, float ai1, float ai2)
        {
            int result = orig.Invoke(spawnSource, X, Y, SpeedX, SpeedY, Type, Damage, KnockBack, Owner, ai0, ai1, ai2);
            if (result >= 0 && result < 1000)
            {
                if (Main.projectile[result].ModProjectile != null && Main.projectile[result].ModProjectile is BaseMagicProj) return result;
                if (Main.projectile[result].TryGetGlobalProjectile(out SourceMarkProj markProj))
                {
                    markProj.RootSource = CurrentRootSource;
                    markProj.LastSource = CurrentLastSource;
                }
                if (Main.projectile[result].TryGetGlobalProjectile(out DamageModifyProj modifyProj))
                {
                    if (CurrentLastSource != -1)
                    {
                        if (Main.npc[CurrentLastSource].HasBuff(ModContent.BuffType<SilencedDNDBuff>()))
                        {
                            modifyProj.DamageMult *= 0.5f;
                        }
                    }
                }
            }
            return result;
        }

    }

    public class SourceMarkNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public int RootSource = -1;
        public int LastSource = -1;
        public int InstantSource = -1;
        public int InstantTimer = 61;
        public override void PostAI(NPC npc)
        {
            if (npc.type != NPCID.MoonLordCore)
            {
                InstantTimer = 0;
            }
            else
            {
                InstantTimer--;
            }
        }
    }

    public class SourceMarkProj : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public int RootSource = -1;
        public int LastSource = -1;
    }

}
