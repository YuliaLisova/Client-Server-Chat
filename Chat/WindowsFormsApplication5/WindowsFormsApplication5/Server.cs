using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using ClassLibrary1;



namespace WindowsFormsApplication5
{
    public partial class Server : Form
    {
        Socket serverSocket;

        byte[] byteData = new byte[1024];

        Dictionary<string, Socket> ClientsList;



        public Server()
        {
            InitializeComponent();
            ClientsList = new Dictionary<string,Socket>();
        }

        private void Server_Load(object sender, EventArgs e)
        {
            try
            {
                serverSocket = new Socket(AddressFamily.InterNetwork,
                              SocketType.Stream,
                              ProtocolType.Tcp);


                string ServerName = Dns.GetHostName().ToString();
                IPHostEntry ipHostEntry = Dns.GetHostEntry(ServerName);
                IPEndPoint ipEndPoint = new IPEndPoint(ipHostEntry.AddressList.First
                        (ipAddress => ipAddress.AddressFamily == AddressFamily.InterNetwork), 1000);


                serverSocket.Bind(ipEndPoint);
                serverSocket.Listen(10);

                ServerIPtextBox.Text = serverSocket.LocalEndPoint.ToString();
                // ServerIPtextBox.Text = ServerName;
                serverSocket.BeginAccept(new AsyncCallback(ThreadForAccept), null);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ThreadForAccept(IAsyncResult ar)
        {

            try{
            Socket clientSocket = serverSocket.EndAccept(ar);
            serverSocket.BeginAccept(new AsyncCallback(ThreadForAccept), null);


            clientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None,
                    new AsyncCallback(ThreadForReceive), clientSocket);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ThreadForReceive(IAsyncResult ar)
        {
            try{
            this.Messages_listBox.Items.Add("received");

            Socket clientSocket = (Socket)ar.AsyncState;
            clientSocket.EndReceive(ar);

                // десериализуем полученное сообщение
            ChatMessage receivedMessage = ChatMessage.DeSerialization(byteData);

           

            string receivedText = receivedMessage.sender + " to "+receivedMessage.receiver
            + ": " + receivedMessage.text;

            this.Messages_listBox.Items.Add(receivedText);

            ChatMessage MsgToSend;

                //если сообщение говорит о подключении нового пользователя
            if (receivedMessage.connect == true)
            {
                //добавляеь нового пользователя в коллекцию на сервере
                try
                {
                    this.ClientsList.Add(receivedMessage.sender, clientSocket);//если пользователь с таким ником уже есть
                    //будет сгенерировано исключение

                    this.Clients_listBox1.Items.Add(receivedMessage.sender);
                    MsgToSend = new ChatMessage();

                    //формируем текс сообщения для отправки, содержащий новый список всех пользователей
                    String allUsers = "";
                    foreach (KeyValuePair<string, Socket> kvp in ClientsList)
                    {
                        allUsers += kvp.Key + "*";
                    }

                    MsgToSend.text = receivedText;
                    MsgToSend.allUsers = allUsers;
                    MsgToSend.sender = "server";
                    MsgToSend.receiver = "all";
                    MsgToSend.connect = true;
                    MsgToSend.disconnect = false;

                    byte[] bytesToSend = ChatMessage.Serialization(MsgToSend);

                    //отправляеь всем подключенным пользователям обновленный список подключенных
                    foreach (KeyValuePair<string, Socket> kvp in ClientsList)
                    {

                        Socket EachclientSocket = kvp.Value;

                        EachclientSocket.BeginSend(bytesToSend, 0, bytesToSend.Length, SocketFlags.None,
                                   new AsyncCallback(OnSend), EachclientSocket);
                    }
                }


                    //если пользователь с таким ником уже есть, отправляем новому клиенту сообщение об этом
                catch (Exception ex)
                {
                    MsgToSend = new ChatMessage();
                    MsgToSend.text = ex.Message;
                    this.Messages_listBox.Items.Add(ex.Message);
                    MsgToSend.allUsers = null;
                    MsgToSend.sender = "server";
                    MsgToSend.receiver = receivedMessage.sender;
                    MsgToSend.connect = false;
                    MsgToSend.disconnect = false;
                    MsgToSend.connection_failed = true;//флаг, говорящий о том, что этот ник занят

                    byte[] bytesToSend = ChatMessage.Serialization(MsgToSend);
                    clientSocket.BeginSend(bytesToSend, 0, bytesToSend.Length, SocketFlags.None,
                                   new AsyncCallback(OnSend), clientSocket);
                    clientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(ThreadForReceive), clientSocket);
                }
            }

                //если в сообщении говорится об отсоединении одного из пользователей
            else if (receivedMessage.disconnect == true)
            {
                //удаляем его из коллекции пользователей на сервере
                ClientsList.Remove(receivedMessage.sender);


                //формируем обновленный список подключенных и рассылаем его всем пользователям
                MsgToSend = new ChatMessage();
                String allUsers = "";

                Clients_listBox1.Items.Clear();

                foreach (KeyValuePair<string, Socket> kvp in ClientsList)
                {
                    allUsers += kvp.Key + "*";
                    Clients_listBox1.Items.Add(kvp.Key);
                }

                MsgToSend.text = receivedText;
                MsgToSend.allUsers = allUsers;
                MsgToSend.sender = "server";
                MsgToSend.receiver = "all";
                MsgToSend.connect = true;
                MsgToSend.disconnect = false;

                byte[] bytesToSend = ChatMessage.Serialization(MsgToSend);


                foreach (KeyValuePair<string, Socket> kvp in ClientsList)
                {

                    Socket EachclientSocket = kvp.Value;

                    EachclientSocket.BeginSend(bytesToSend, 0, bytesToSend.Length, SocketFlags.None,
                               new AsyncCallback(OnSend), EachclientSocket);
                }


            }

                //если это обычное сообщение
            else
            {
                //формируем текс сообщения для отправки
                MsgToSend = new ChatMessage();
                MsgToSend.text = receivedText;

                Socket socketReceiver;

                byte[] bytesToSend = ChatMessage.Serialization(MsgToSend);

                //находим в коллекции сокетов сокет, соответсвующих нику получателя
                //и отправляем ему полученное сообщение
                foreach (KeyValuePair<string, Socket> kvp in ClientsList)
                {
                    if (kvp.Key == receivedMessage.receiver)
                    {
                        socketReceiver = kvp.Value;
                        socketReceiver.BeginSend(bytesToSend, 0, bytesToSend.Length, SocketFlags.None,
                                   new AsyncCallback(OnSend), socketReceiver);

                        break;
                    }
                }


                clientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(ThreadForReceive), clientSocket);
            }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
               

        }


        public void OnSend(IAsyncResult ar)
        {
            try{
            Socket client = (Socket)ar.AsyncState;
            client.EndSend(ar);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
