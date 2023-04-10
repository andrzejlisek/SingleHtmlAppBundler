namespace SingleHtmlAppBundler
{
    class GlobalId
    {
        static int Id = 0;

        static Dictionary<string, int> IdKey = new Dictionary<string, int>();

        public static void Clear()
        {
            Id = 0;
            IdKey.Clear();
        }

        static string IdStr(int Id_)
        {
            return "_" + Id_ + "_";
        }

        public static string NewId(string Key, bool ForceNew)
        {
            if (ForceNew)
            {
                bool NotAllowed = true;
                Id++;
                if (!IdKey.ContainsKey(Key))
                {
                    IdKey.Add(Key, Id);
                }
                else
                {
                    IdKey[Key] = Id;
                }
                return IdStr(Id);
            }
            else
            {
                if (IdKey.ContainsKey(Key))
                {
                    return IdStr(IdKey[Key]);
                }
                else
                {
                    Id++;
                    IdKey.Add(Key, Id);
                    return IdStr(Id);
                }
            }
        }
    }
}