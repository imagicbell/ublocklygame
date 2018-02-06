namespace UToolbox
{
    using UnityEngine;
    using System.IO;
    
    /// <summary>
    /// 序列化、反序列化，存储数据
    /// </summary>
    public static class SerializeHelper
    {
        public static string ToJson<T>(this T obj) where T : class
        {
#if UNITY_EDITOR
            return JsonUtility.ToJson(obj, true);
#else
            return JsonUtility.ToJson(obj);
#endif
        }

        public static T FromJson<T>(this string json) where T : class
        {
            return (T) JsonUtility.FromJson(json, typeof(T));
        }

        public static void SaveJson<T>(this T obj, string path) where T : class
        {
            System.IO.File.WriteAllText(path, obj.ToJson<T>());
        }

        public static T LoadJson<T>(string path) where T : class
        {
            return System.IO.File.ReadAllText(path).FromJson<T>();
        }


        public static byte[] ToProtoBuff<T>(this T obj) where T : class
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                ProtoBuf.Serializer.Serialize<T>(ms, obj);
                return ms.ToArray();
            }
        }

        public static T FromProtoBuff<T>(this byte[] bytes) where T : class
        {
            if (bytes == null || bytes.Length == 0)
            {
                throw new System.ArgumentNullException("bytes");
            }
            T t = ProtoBuf.Serializer.Deserialize<T>(new System.IO.MemoryStream(bytes));
            return t;
        }

        public static void SaveProtoBuff<T>(this T obj, string path) where T : class
        {
            System.IO.File.WriteAllBytes(path, obj.ToProtoBuff<T>());
        }

        public static T LoadProtoBuff<T>(string path) where T : class
        {
            return System.IO.File.ReadAllBytes(path).FromProtoBuff<T>();
        }

#if UNITY_EDITOR

        //Example
        //*********json 数据结构***********
        [System.Serializable]
        class PlayerInfo
        {
            public string name;
            public int lives;
            public float health;
            public ChildInfo[] childs;
        }

        [System.Serializable]
        class ChildInfo
        {
            public string name;
            public int lives;
        }

        //*********protobuf 数据结构***********
        [ProtoBuf.ProtoContract]
        class Person
        {
            [ProtoBuf.ProtoMember(1)]
            public int Id { get; set; }

            [ProtoBuf.ProtoMember(2)]
            public string Name { get; set; }

            [ProtoBuf.ProtoMember(3)]
            public Address Address { get; set; }
        }

        [ProtoBuf.ProtoContract]
        class Address
        {
            [ProtoBuf.ProtoMember(1)]
            public string Line1 { get; set; }

            [ProtoBuf.ProtoMember(2)]
            public string Line2 { get; set; }
        }

        static void Example()
        {
            //use json
            PlayerInfo playerInfo = new PlayerInfo();
            playerInfo.name = "qINGYUN";
            playerInfo.lives = 25;

            playerInfo.SaveJson<PlayerInfo>("/UserData/..");
            playerInfo = LoadJson<PlayerInfo>("/UserData/..");

            //use protobuf
            Person person = new Person();
			person.Name = "zhenhua";

            person.SaveProtoBuff<Person>("/UserData/..");
            person = LoadProtoBuff<Person>("/UserData/..");
        }
#endif
    }
}