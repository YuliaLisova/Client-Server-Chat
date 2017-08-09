using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO; 

namespace ClassLibrary1
{
     [Serializable]
    public class ChatMessage
    {
        public string sender;
        public string receiver;
        public string text;
        public string allUsers;
       public bool connect;
       public bool disconnect;
      public bool connection_failed;


        public static byte[] Serialization(ChatMessage obj)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, obj);
            byte[] msg = stream.ToArray();
            return msg;
        }

        public static ChatMessage DeSerialization(byte[] serializedAsBytes)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            stream.Write(serializedAsBytes, 0, serializedAsBytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return (ChatMessage)formatter.Deserialize(stream);
        }
    }
}
