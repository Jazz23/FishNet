using FishNet.Connection;
using FishNet.Managing.Server;
using FishNet.Managing.Timing;
using FishNet.Object;
using UnityEngine;

namespace Stuff
{
    public class TestNb : NetworkBehaviour
    {
        public override void OnStartClient()
        {
        }

        public void SendTestRpc()
        {
            TestRpc(TimeManager.GetPreciseTick(TickType.LastPacketTick));
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void TestRpc(PreciseTick tick, NetworkConnection conn = null)
        {
            if (tick.Tick > conn!.LocalTick.LocalTick)
            {
                conn.Kick(KickReason.MalformedData, log: "Out of order tick.");
                return;
            }
            
            Debug.Log("Received RPC");
            var idfk = conn.PacketTick;
            var timeDelta = TimeManager.TimePassed(tick);
            var timeDelta2 = TimeManager.TimePassed(idfk.LastRemoteTick);
            var timeDelta4 = TimeManager.TimePassed(conn.LocalTick.LocalTick);


            var serverReceivedTick = TimeManager.GetPreciseTick(TickType.Tick);
            var serverReceivedTick2 = TimeManager.GetPreciseTick(TickType.LastPacketTick);
            var ticksMissed = serverReceivedTick.Tick - tick.Tick;
            var timeDelta3 = ticksMissed * TimeManager.TickDelta;
        }
    }
}