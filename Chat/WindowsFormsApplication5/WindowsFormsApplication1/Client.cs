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

namespace WindowsFormsApplication1
{
    public partial class Client : Form
    {

        Socket userSocket;
        String nickName;

        byte[] byteData;

        public Client()
        {
            InitializeComponent();
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {

            try
            {
                if (this.NickNameTextBox.Text == null || this.NickNameTextBox.Text == " " || this.NickNameTextBox.Text == "")
                {
                    MessageBox.Show("Заполните поле \"Nick\"");
                }

                else
                {
                    nickName = this.NickNameTextBox.Text;
                    userSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    IPAddress ipAddress = IPAddress.Parse(this.ServerNameTextBox.Text);

                    IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 1000);


                    userSocket.BeginConnect(ipEndPoint, new AsyncCallback(ThreadForConnect), null);
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ThreadForConnect(IAsyncResult ar)
        {
            try
            {
                ChatMessage msgToSend = new ChatMessage();


                msgToSend.sender = this.nickName;
                msgToSend.receiver = "server";
                msgToSend.text = "New user " + nickName + " has joined";
                msgToSend.connect = true;
                msgToSend.disconnect = false;


                byte[] b = ChatMessage.Serialization(msgToSend);

                NickNameTextBox.Enabled = false;
                ServerNameTextBox.Enabled = false;
                Sendbutton.Enabled = true;
                ConnectButton.Enabled = false;
                
                userSocket.BeginSend(b, 0, b.Length, SocketFlags.None, new AsyncCallback(ThreadForSend), null);
            }
            catch (Exception ex)
            {
                
                MessageBox.Show(ex.Message);

                NickNameTextBox.Enabled = true;
                ServerNameTextBox.Enabled = true;
                Sendbutton.Enabled = false;
                ConnectButton.Enabled = true;
                
            }
        }

        public void ThreadForSend(IAsyncResult ar)
        {
            try
            {
                userSocket.EndSend(ar);

                this.MessagesListBox.Items.Add("sent");

                byteData = new byte[1024];
              
                userSocket.BeginReceive(byteData,
                                            0,
                                            byteData.Length,
                                            SocketFlags.None,
                                            new AsyncCallback(OnReceive),
                                            null);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public void OnReceive(IAsyncResult ar)
        {

            try
            {
                userSocket.EndReceive(ar);
                MessagesListBox.Items.Add("received");

                ChatMessage receivedMsg = ChatMessage.DeSerialization(byteData);
                //если получен ответ, что пользователь с таким именем уже существует
                if (receivedMsg.connection_failed == true)
                {
                    MessageBox.Show(receivedMsg.text);
                    
                    userSocket.Close();
                    MessageBox.Show("Соединенеи не установлено.\n Возможная причина: \n пользователь с таким именем уже существует\n ");
                    NickNameTextBox.Enabled = true;
                    ServerNameTextBox.Enabled = true;
                    Sendbutton.Enabled = false;
                    ConnectButton.Enabled = true;


                }
                    //если получено сообщение об подключении или отключении пользователя
                else if (receivedMsg.connect == true || receivedMsg.disconnect == true)
                {
                    this.UserslistBox.Items.Clear();

                    this.MessagesListBox.Items.Add(receivedMsg.text);

                    UserslistBox.Items.AddRange(receivedMsg.allUsers.Split('*'));
                   
                }

                    //если это обычное сообщение
                else
                {
                    MessagesListBox.Items.Add(receivedMsg.text);
                }

                userSocket.BeginReceive(byteData,
                                          0,
                                          byteData.Length,
                                          SocketFlags.None,
                                          new AsyncCallback(OnReceive),
                                          null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void Sendbutton_Click(object sender, EventArgs e)
        {

            try
            {
                if (UserslistBox.SelectedItem == null)
                {
                    MessageBox.Show("Вы не выбрали получателя!");
                }

                else if (UserslistBox.SelectedItem.ToString() == NickNameTextBox.Text)
                {
                    MessageBox.Show("Вы не можете отправить сообщение себе!");
                }

                else
                {
                    ChatMessage MsgForSend = new ChatMessage();

                    MsgForSend.sender = this.NickNameTextBox.Text;
                    MsgForSend.receiver = this.UserslistBox.SelectedItem.ToString();
                    MsgForSend.connect = false;
                    MsgForSend.disconnect = false;
                    MsgForSend.allUsers = " ";
                    MsgForSend.text = textBox1.Text;

                    byte[] sending = ChatMessage.Serialization(MsgForSend);

                    userSocket.BeginSend(sending, 0, sending.Length, SocketFlags.None, new AsyncCallback(ThreadForSend), null);
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Client_Load(object sender, EventArgs e)
        {
            this.Sendbutton.Enabled = false;
        }


        private void Client_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                ChatMessage MsgForClose = new ChatMessage();

                MsgForClose.sender = this.NickNameTextBox.Text;
                MsgForClose.receiver = "server";
                MsgForClose.connect = false;
                MsgForClose.disconnect = true;
                MsgForClose.allUsers = " ";
                MsgForClose.text = this.nickName + " disconnected from the server";

                byte[] closing = ChatMessage.Serialization(MsgForClose);

                userSocket.Send(closing, 0, closing.Length, SocketFlags.None);
                userSocket.Close();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
