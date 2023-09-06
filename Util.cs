using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DeadLogger
{
    public class Util
    {
        public static void SendLog(string description, Vector3 pos, string nick, long peerId, long creatorId)
        {
            description += " Posição: " + (int)pos.x + "," + (int)pos.z;
            description += " CreatorId: " + creatorId;
            ZPackage pkg = new ZPackage();
            string msg = nick + "|";
            msg += peerId + "|";
            msg += description + "|";
            pkg.Write(msg);
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "DeadLogger_SaveLog", pkg);
        }

        public static void PrepareLog(string textToAppend, Player __instance, GameObject gameObject)
        {
            if (DeadLogger.PrefabsToLog.Value != "")
            {
                if (!DeadLogger.PrefabsToLog.Value.Contains(gameObject.name)) return;
            }


            List<ZNet.PlayerInfo> playerInfoList = new List<ZNet.PlayerInfo>();
            ZNet.instance.GetOtherPublicPlayers(playerInfoList);

            long peerId = __instance.m_nview.m_zdo.GetOwner();
            ZNet.PlayerInfo playerInfo = playerInfoList.FirstOrDefault(x => x.m_characterID.UserID == peerId);

            string playerName = playerInfo.m_name;

            if (__instance.GetPlayerID() == Player.m_localPlayer.GetPlayerID()) playerName = Game.instance.GetPlayerProfile().m_playerName;

            long creatorId = 0;
            Piece piece = gameObject.GetComponent<Piece>();
            if (piece) creatorId = piece.m_creator;

            SendLog(textToAppend + gameObject.name, __instance.transform.position, playerName, __instance.GetPlayerID(), creatorId);
        }
    }
}
