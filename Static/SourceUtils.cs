using BG3MagicRework.Systems;
using Terraria;

namespace BG3MagicRework.Static
{
    public static class SourceUtils
    {
        internal static int _getRootSource(this NPC npc)
        {
            if (!npc.TryGetGlobalNPC<SourceMarkNPC>(out _)) return -1;
            return npc.GetGlobalNPC<SourceMarkNPC>().RootSource;
        }

        internal static int _getLastSource(this NPC npc)
        {
            if (!npc.TryGetGlobalNPC<SourceMarkNPC>(out _)) return -1;
            return npc.GetGlobalNPC<SourceMarkNPC>().LastSource;
        }

        internal static int _getInstantSource(this NPC npc)
        {
            if (!npc.TryGetGlobalNPC<SourceMarkNPC>(out _)) return -1;
            return npc.GetGlobalNPC<SourceMarkNPC>().InstantSource;
        }

        internal static int _getRootSource(this Projectile proj)
        {
            if (!proj.TryGetGlobalProjectile<SourceMarkProj>(out _)) return -1;
            return proj.GetGlobalProjectile<SourceMarkProj>().RootSource;
        }

        internal static int _getLastSource(this Projectile proj)
        {
            if (!proj.TryGetGlobalProjectile<SourceMarkProj>(out _)) return -1;
            return proj.GetGlobalProjectile<SourceMarkProj>().LastSource;
        }

        /// <summary>
        /// �����Դͷ��NPC.WhoAmI
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        public static int GetRootSource(this NPC npc)
        {
            int source = npc._getRootSource();
            if (source == -1)
            {
                return -1;
            }
            else
            {
                if (!Main.npc[source].active)
                {
                    return -1;
                }
                else
                {
                    return source;
                }
            }
        }

        /// <summary>
        /// �����һ��Դͷ��NPC.WhoAmI
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        public static int GetLastSource(this NPC npc)
        {
            int source = npc._getLastSource();
            if (source == -1)
            {
                return -1;
            }
            else
            {
                if (!Main.npc[source].active)
                {
                    return -1;
                }
                else
                {
                    return source;
                }
            }
        }

        /// <summary>
        /// ��õ�֡�ٻ�NPC����Դͷ
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        public static int GetInstantSource(this NPC npc)
        {
            int source = npc._getInstantSource();
            if (source == -1)
            {
                return -1;
            }
            else
            {
                if (!Main.npc[source].active)
                {
                    return -1;
                }
                else
                {
                    return source;
                }
            }
        }

        /// <summary>
        /// �����Դͷ��NPC.WhoAmI
        /// </summary>
        /// <param name="proj"></param>
        /// <returns></returns>
        public static int GetRootSource(this Projectile proj)
        {
            int source = proj._getRootSource();
            if (source == -1)
            {
                return -1;
            }
            else
            {
                if (!Main.npc[source].active)
                {
                    return -1;
                }
                else
                {
                    return source;
                }
            }
        }

        public static int GetLastSource(this Projectile proj)
        {
            int source = proj._getLastSource();
            if (source == -1)
            {
                return -1;
            }
            else
            {
                if (!Main.npc[source].active)
                {
                    return -1;
                }
                else
                {
                    return source;
                }
            }
        }
    }
}