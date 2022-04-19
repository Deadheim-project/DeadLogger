using HarmonyLib;
using UnityEngine;

namespace DeadLogger
{
    class Patches
    {
        [HarmonyPatch(typeof(Player), nameof(Player.PlacePiece))]
        public static class PlacePiece
        {
            private static void Postfix(Player __instance, Piece piece, bool __result)
            {
                if (__result)
                {
                    Util.PrepareLog("Construiu: ", __instance, piece.gameObject);
                }
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.RemovePiece))]
        public static class RemovePiece
        {
            public static Piece pieceRemoved;
            public static string portalTag = "";

            private static void Postfix(Player __instance, bool __result)
            {
                if (__result)
                {
                    string description = "Destruiu: ";

                    try
                    {
                        TeleportWorld teleport = pieceRemoved.gameObject.GetComponent<TeleportWorld>();
                        if (teleport) description = "Destruiu Portal " + portalTag + " : ";

                        PrivateArea area = pieceRemoved.gameObject.GetComponent<PrivateArea>();
                        Fireplace fireplace = pieceRemoved.gameObject.GetComponent<Fireplace>();
                        if (fireplace && area) description = "Destruiu Totem Combustivel " + fireplace.m_nview.GetZDO().GetFloat("fuel", -1f) + " : ";
                    }
                    catch
                    {

                    }


                    Util.PrepareLog(description, __instance, pieceRemoved.gameObject);
                }
            }

            private static void Finalizer()
            {
                pieceRemoved = null;
                portalTag = "";
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.CheckCanRemovePiece))]
        public static class PieceRemoved
        {
            private static void Postfix(Piece piece)
            {
                RemovePiece.pieceRemoved = piece;
                TeleportWorld teleport = piece.gameObject.GetComponent<TeleportWorld>();
                if (teleport) RemovePiece.portalTag = teleport.GetText() + " : ";

            }
        }

        [HarmonyPatch(typeof(WearNTear), "RPC_Damage")]
        public static class RPC_Damage
        {
            private static void Postfix(WearNTear __instance, ref HitData hit, ZNetView ___m_nview)
            {
                if (___m_nview is null) return;

                if (hit.GetAttacker() && hit.GetAttacker().IsPlayer())
                {
                    float totalDamage = hit.GetTotalDamage();

                    string description = "Destruiu Hit: ";
                    TeleportWorld teleport = __instance.gameObject.GetComponent<TeleportWorld>();
                    if (teleport) description = "Destruiu Hit Portal " + teleport.GetText() + " : ";

                    PrivateArea area = __instance.gameObject.GetComponent<PrivateArea>();
                    Fireplace fireplace = __instance.gameObject.GetComponent<Fireplace>();
                    if (fireplace && area) description = "Destruiu Totem Combustivel " + fireplace.m_nview.GetZDO().GetFloat("fuel", -1f) + " : ";


                    if (__instance.m_health <= totalDamage)
                    {
                        Util.PrepareLog(description, (Player)hit.GetAttacker(), __instance.gameObject);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Door), nameof(Door.Interact))]
        public static class DoorCanInteract
        {
            private static void Postfix(Door __instance, ref bool __result, Humanoid character)
            {
                if (!__result) return;

                if (!character.IsPlayer()) return;

                Util.PrepareLog("Interagiu Porta: ", (Player)character, __instance.gameObject);
            }
        }

        [HarmonyPatch(typeof(Container), nameof(Container.Interact))]
        public static class ContainerInteract
        {
            private static void Postfix(Container __instance, ref bool __result, Humanoid character)
            {
                if (!__result) return;

                if (!character.IsPlayer()) return;

                Util.PrepareLog("Interagiu Container: ", (Player)character, __instance.gameObject);
            }
        }

        [HarmonyPatch(typeof(PrivateArea), nameof(PrivateArea.Interact))]
        public static class PrivateAreaInteract
        {
            private static void Postfix(PrivateArea __instance, ref bool __result, Humanoid human)
            {
                if (!__result) return;

                if (!human.IsPlayer()) return;

                string description = "Interagiu Totem: ";

                PrivateArea area = __instance.gameObject.GetComponent<PrivateArea>();
                Fireplace fireplace = __instance.gameObject.GetComponent<Fireplace>();

                if (fireplace && area) description = "Interagiu Totem Combustivel " + fireplace.m_nview.GetZDO().GetFloat("fuel", -1f) + " : ";


                Util.PrepareLog(description, (Player)human, __instance.gameObject);
            }
        }

        [HarmonyPatch(typeof(TeleportWorld), nameof(TeleportWorld.Interact))]
        public static class TeleportInteract
        {
            private static void Postfix(TeleportWorld __instance, ref bool __result, Humanoid human)
            {
                if (!__result) return;

                if (!human.IsPlayer()) return;

                Util.PrepareLog("Interagiu Portal " + __instance.GetText() + " : ", (Player)human, __instance.gameObject);
            }
        }

        [HarmonyPatch(typeof(TeleportWorld), nameof(TeleportWorld.Teleport))]
        public static class TeleportTeleport
        {
            private static void Postfix(TeleportWorld __instance, Player player)
            {

                if (!__instance.TargetFound()) return;
                if (ZoneSystem.instance.GetGlobalKey("noportals")) return;
                if (!player.IsTeleportable()) return;
                if (!player.IsPlayer()) return;

                PlayerTeleportTo.PortalName = __instance.GetText();
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.TeleportTo))]
        public static class PlayerTeleportTo
        {
            public static string PortalName = "";
            private static void Postfix(Player __instance, ref bool __result, Vector3 pos)
            {
                if (!__result) return;

                Util.PrepareLog("Teleportou com portal" + PortalName + " para x " + (int)pos.x + "," + (int)pos.z + ": ", __instance, __instance.gameObject);
            }

            private static void Finalizer()
            {
                PortalName = "";
            }
        }
    }
}
