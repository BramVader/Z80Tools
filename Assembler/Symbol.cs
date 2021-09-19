namespace Assembler
{

    public class Symbol
    {
        public object[] Value { get; set; }

        public bool IsPublic { get; set; }    // When declared with double colon (::)

        public string Name { get; set; }

        public bool Readonly { get; set; }

        public SymbolType Type { get; set; }

        public bool HasEqualValue(Symbol other)
        {
            static bool checkEqual(object a, object b)
            {
                if ((a == null) != (b == null))
                    return false;
                if (a == null)
                    return true;

                if (a is object[] arra)
                {
                    if (b is object[] arrb)
                    {
                        if (arra.Length != arrb.Length)
                            return false;
                        for (int n = 0; n < arra.Length; n++)
                            if (!checkEqual(arra[n], arrb[n]))
                                return false;
                        return true;
                    }
                    return false;
                }
                return a.Equals(b);
            }

            return checkEqual(Value, other.Value);
        }
    }
}
