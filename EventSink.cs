using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pendulum
{
    public delegate void NexusPacketEventHandler(NexusPacketEventArgs e);

    public class NexusPacketEventArgs : EventArgs
    {
        private readonly bool _fromClient;
        private readonly byte[] _data;

        public bool FromClient { get { return _fromClient; } }
        public byte[] Data { get { return _data; } }

        public NexusPacketEventArgs(byte[] data, bool fromClient)
        {
            _fromClient = fromClient;
            _data = data;
        }
    }

    public static class EventSink
    {
        public static event NexusPacketEventHandler NexusPacket;

        public static void InvokeNexusPacket(NexusPacketEventArgs e)
        {
            if (NexusPacket != null)
            {
                NexusPacket(e);
            }
        }
    }
}
