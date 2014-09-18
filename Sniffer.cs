using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;

namespace pendulum
{
    public class Sniffer
    {

        private ICaptureDevice _device;

        public Sniffer(ICaptureDevice device)
        {
            _device = device;
        }

        public void Start()
        {
            _device.OnPacketArrival += OnPacketArrival;
            _device.Open(DeviceMode.Promiscuous, 1000);
            _device.StartCapture();
        }

        public void Stop()
        {
            _device.StopCapture();
            _device.Close();
        }

        private void OnPacketArrival(object sender, CaptureEventArgs args)
        {
            Packet packet = Packet.ParsePacket(args.Packet.LinkLayerType, args.Packet.Data);
            TcpPacket tcpPacket = (TcpPacket)packet.Extract(typeof (TcpPacket));

            if (tcpPacket == null) return;
            //Make sure we are only getting nexus stuff
            if ((tcpPacket.DestinationPort == 2001 || tcpPacket.SourcePort == 2001) ||
                (tcpPacket.DestinationPort == 2000 || tcpPacket.SourcePort == 2000))
            {
                if (tcpPacket.PayloadData != null && tcpPacket.PayloadData.Length > 0 && tcpPacket.PayloadData[0] == 0xAA)
                {
                    EventSink.InvokeNexusPacket(
                        new NexusPacketEventArgs(
                            tcpPacket.PayloadData,
                            (tcpPacket.DestinationPort == 2001 || tcpPacket.DestinationPort == 2000)
                            )
                        );

                }
                
            }
        }
    }
}
