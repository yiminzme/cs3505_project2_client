﻿using NetworkController;
using SnakeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SnakeClient
{
    public class ClientSnakeNetworkController
    {
       
        /// <summary>
        /// The socket state that we use to communicate with the remote server.
        /// </summary>
        SocketState clientSocketState;

        /// <summary>
        /// Indicates whether or not we have completed all the neccisary steps required to send movement connections.
        /// </summary>
        bool initialized = false;

        public struct InitData
        {
            public InitData(int playerId, int WorldWidth, int WorldHeight)
            {
                PlayerId = playerId;
                WorldSize = new World.Dimensions(WorldWidth, WorldHeight);
            }

            public int PlayerId {get; private set; }
            public World.Dimensions WorldSize {get; private set; }
        }

        public delegate void handleInitData(InitData initData);
        public delegate void handleDataReceived(IList<string> data);



        public bool connectToServer(string hostname, string playerName, handleInitData handshakeCompletedHandler)
        {
            if (clientSocketState != null) { return false; }
            Socket s = Networking.ConnectToNetworkNode(hostname, Networking.DEFAULT_PORT, (ss) => { handleNetworkNodeConnected(ss, playerName, handshakeCompletedHandler); });
            return !ReferenceEquals(s, null);
        }


        private void handleNetworkNodeConnected(SocketState aSocketState, string playerName, handleInitData handshakeCompletedHandler)
        {
            clientSocketState = aSocketState;

            Networking.Send(aSocketState.theSocket, playerName + '\n');
            Networking.listenForData(aSocketState, (ss) => { worldSetupDataRecieved(ss, handshakeCompletedHandler); });
        }

        private void worldSetupDataRecieved(SocketState aSocketState, handleInitData handshakeCompletedHandler)
        {
            IList<String> setupData = Networking.getMessageStringsFromBufferSeperatedByCharacter(aSocketState, '\n');

            //Expects 3 Lines Of Startup Data, If It Isn't Recieved Continue Listening And Resets Buffer
            if(setupData.Count() < 3)
            {
                Networking.resetGrowableBufferWithMessagesSeperatedByCharacter(aSocketState, setupData, '\n');
                Networking.listenForData(aSocketState, (ss) => { worldSetupDataRecieved(ss, handshakeCompletedHandler); });
                return;
            }

            int playerId;
            int worldWidth;
            int worldHeight;

            Int32.TryParse(setupData[0], out playerId);
            Int32.TryParse(setupData[1], out worldWidth);
            Int32.TryParse(setupData[2], out worldHeight);

            handshakeCompletedHandler(new InitData(playerId, worldWidth, worldHeight));
            initialized = true;
        }

        public void startDataListenerLoop(handleDataReceived dataReceivedHandler)
        {
            Networking.listenForData(clientSocketState, (ss) => { receiveDataAndStartListeningForMoreData(ss, dataReceivedHandler); });
        }

        public void receiveDataAndStartListeningForMoreData(SocketState aSocketState, handleDataReceived dataReceivedHandler)
        {
            IList<string> data = Networking.getMessageStringsFromBufferSeperatedByCharacter(aSocketState, '\n');

            dataReceivedHandler(data);

            startDataListenerLoop(dataReceivedHandler);
        }

        /// <summary>
        /// Sends the specified direction input to the server.
        /// </summary>
        public void sendDirection(int direction)
        {
            if (!initialized)
            {
                return;
            }

            Networking.Send(clientSocketState.theSocket, "("+direction+")\n");
        }

    }

}