//-----------------------------------------------------------------------------
// Filename: SilverlightTCPSIPChannel.cs
//
// Description: TCP SIP channel for us with Silverlight client. The Silverlight TCP socket
// has a number of restrictions functionality and security wise hence the need for a custom
// channel implementation different from a standard TCP SIP channel.
// 
// History:
// 12 OCt 2008	Aaron Clauson	Created.
//
// License: 
// This software is licensed under the BSD License http://www.opensource.org/licenses/bsd-license.php
//
// Copyright (c) 2006-2008 Aaron Clauson (aaronc@blueface.ie), Blue Face Ltd, Dublin, Ireland (www.blueface.ie)
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that 
// the following conditions are met:
//
// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer. 
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following 
// disclaimer in the documentation and/or other materials provided with the distribution. Neither the name of Blue Face Ltd. 
// nor the names of its contributors may be used to endorse or promote products derived from this software without specific 
// prior written permission. 
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, 
// BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
// IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, 
// OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.
//-----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using SIPSorcery.Sys;

#if UNITTEST
using NUnit.Framework;
#endif

namespace SIPSorcery.SIP
{
    public class SilverlightTCPSIPChannel : SIPChannel
    {
        private const int MAX_SOCKET_BUFFER_SIZE = 4096;        // Max amount of data that can be received from the socket on a single read.

        private Socket m_socket;
        private byte[] m_socketBuffer = new byte[MAX_SOCKET_BUFFER_SIZE];
        private IPEndPoint m_remoteEndPoint;
        
        private bool m_isConnected = false;
        public bool IsConnected
        {
            get { return m_isConnected; }
        }

        public SilverlightTCPSIPChannel(IPEndPoint localEndPoint)
        {
            m_localSIPEndPoint = new SIPEndPoint(SIPProtocolsEnum.tcp, localEndPoint);
            m_isReliable = true;
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(IPEndPoint remoteEndPoint)
        {
            m_remoteEndPoint = remoteEndPoint;
            SocketAsyncEventArgs socketConnectionArgs = new SocketAsyncEventArgs();
            socketConnectionArgs.RemoteEndPoint = m_remoteEndPoint;
            socketConnectionArgs.Completed += SocketConnect_Completed;

            m_socket.ConnectAsync(socketConnectionArgs);
        }

        private void SocketConnect_Completed(object sender, SocketAsyncEventArgs e)
        {
            m_isConnected = (e.SocketError == SocketError.Success);

            if (m_isConnected)
            {
                SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs();
                receiveArgs.SetBuffer(m_socketBuffer, 0, MAX_SOCKET_BUFFER_SIZE);
                receiveArgs.Completed += SocketRead_Completed;
                m_socket.ReceiveAsync(receiveArgs);
            }
            else
            {
                throw new ApplicationException("Connection to " + m_remoteEndPoint + " failed.");
            }
        }

        private void SocketRead_Completed(object sender, SocketAsyncEventArgs e)
        {
            int bytesRead = e.BytesTransferred;
            if (bytesRead > 0)
            {
                byte[] buffer = new byte[bytesRead];
                Array.Copy(m_socketBuffer, buffer, bytesRead);

                if (SIPMessageReceived != null)
                {
                    SIPMessageReceived(this, new SIPEndPoint(SIPProtocolsEnum.tcp, m_remoteEndPoint), buffer);
                }

                SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs();
                receiveArgs.SetBuffer(m_socketBuffer, 0, MAX_SOCKET_BUFFER_SIZE);
                receiveArgs.Completed += SocketRead_Completed;
                m_socket.ReceiveAsync(receiveArgs);
            }
        }

        public void Send(string message)
        {
            byte[] sendBuffer = Encoding.UTF8.GetBytes(message);
            SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
            sendArgs.SetBuffer(sendBuffer, 0, sendBuffer.Length);
            m_socket.SendAsync(sendArgs);
        }

        public override void Send(IPEndPoint destinationEndPoint, string message)
        {
            Send(message);
        }

        public override void Send(IPEndPoint destinationEndPoint, byte[] buffer)
        {
            SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
            sendArgs.SetBuffer(buffer, 0, buffer.Length);
            m_socket.SendAsync(sendArgs);
        }

        public override void Send(IPEndPoint dstEndPoint, byte[] buffer, string serverCN) {
            throw new ApplicationException("This Send method is not available in the Silverlight SIP TCP channel, please use an alternative overload.");
        }

        public override void Close()
        {
            m_socket.Close();
        }
    }
}