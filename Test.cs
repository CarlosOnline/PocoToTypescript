using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pocoyo
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TestClassAttribute : Attribute
    {
        public string Name { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class TestPropAttribute : Attribute
    {
        public string Name { get; set; }
    }

    struct TestStruct
    {
        bool boolProp1 { get; set; }
    }

    [TestClass]
    class TestClass
    {
        [TestProp]
        public bool boolProp1 { get; set; }
    }

    abstract class TestClassAbstract
    {
        public bool boolProp1 { get; set; }
    }

    interface TestInterface
    {
        bool boolProp1 { get; set; }
    }

    enum TestEnum
    {
        Value1,
        Value2,
        Value10 = 10,
        Value11,
        Value4 = 4,
        Value5
    }

    enum TestEnumLong : long
    {
        Value1,
        Value2,
        Value10 = 10,
        Value11,
        Value4 = 4,
        Value5
    }

    class TestPrivateProps
    {
        private bool boolProp { get; set; }
        private byte byteProp { get; set; }
        private char charProp { get; set; }
        private decimal decimalProp { get; set; }
        private double doubleProp { get; set; }
        private dynamic dynamicProp { get; set; }
        private float floatProp { get; set; }
        private int intProp { get; set; }
        private int[,] intArrayXXProp { get; set; }
        private int[] intArrayXProp { get; set; }
        private int[][] intArrayXArrayXProp { get; set; }
        private long longProp { get; set; }
        private object objectProp { get; set; }
        private sbyte sbyteProp { get; set; }
        private short shortProp { get; set; }
        private string stringProp { get; set; }
        private uint uintProp { get; set; }
        private ulong ulongProp { get; set; }
        private ushort ushortProp { get; set; }
        private TestStruct testStructProp { get; set; }
        private TestClass testClassProp { get; set; }
        private TestClassAbstract TestClassAbstractProp { get; set; }
        private TestInterface TestInterfaceProp { get; set; }
        private TestEnum testEnumProp { get; set; }
        private Pocoyo2.TestStruct qualifiedTestStructProp { get; set; }
        private Pocoyo2.TestClass qualifiedTestClassProp { get; set; }
        private Pocoyo2.TestEnum qualifiedTestEnumProp { get; set; }
    }

    class TestPublicProps
    {
        public bool boolProp { get; set; }
        public byte byteProp { get; set; }
        public char charProp { get; set; }
        public decimal decimalProp { get; set; }
        public double doubleProp { get; set; }
        public dynamic dynamicProp { get; set; }
        public float floatProp { get; set; }
        public int intProp { get; set; }
        public int[,] intArrayXXProp { get; set; }
        public int[] intArrayXProp { get; set; }
        public int[][] intArrayXArrayXProp { get; set; }
        public long longProp { get; set; }
        public object objectProp { get; set; }
        public sbyte sbyteProp { get; set; }
        public short shortProp { get; set; }
        public string stringProp { get; set; }
        public uint uintProp { get; set; }
        public ulong ulongProp { get; set; }
        public ushort ushortProp { get; set; }
        public TestStruct testStructProp { get; set; }
        public TestClass testClassProp { get; set; }
        public TestClassAbstract TestClassAbstractProp { get; set; }
        public TestInterface TestInterfaceProp { get; set; }
        public TestEnum testEnumProp { get; set; }
        public Pocoyo2.TestStruct qualifiedTestStructProp { get; set; }
        public Pocoyo2.TestClass qualifiedTestClassProp { get; set; }
        public Pocoyo2.TestEnum qualifiedTestEnumProp { get; set; }
    }

    class TestStaticPublicProps
    {
        public static bool boolProp { get; set; }
        public static byte byteProp { get; set; }
        public static char charProp { get; set; }
        public static decimal decimalProp { get; set; }
        public static double doubleProp { get; set; }
        public static dynamic dynamicProp { get; set; }
        public static float floatProp { get; set; }
        public static int intProp { get; set; }
        public static int[,] intArrayXXProp { get; set; }
        public static int[] intArrayXProp { get; set; }
        public static int[][] intArrayXArrayXProp { get; set; }
        public static long longProp { get; set; }
        public static object objectProp { get; set; }
        public static sbyte sbyteProp { get; set; }
        public static short shortProp { get; set; }
        public static string stringProp { get; set; }
        public static uint uintProp { get; set; }
        public static ulong ulongProp { get; set; }
        public static ushort ushortProp { get; set; }
        public static TestStruct testStructProp { get; set; }
        public static TestClass testClassProp { get; set; }
        public static TestClassAbstract TestClassAbstractProp { get; set; }
        public static TestInterface TestInterfaceProp { get; set; }
        public static TestEnum testEnumProp { get; set; }
    }

    class TestInternalProps
    {
        internal bool boolProp { get; set; }
        internal byte byteProp { get; set; }
        internal char charProp { get; set; }
        internal decimal decimalProp { get; set; }
        internal double doubleProp { get; set; }
        internal dynamic dynamicProp { get; set; }
        internal float floatProp { get; set; }
        internal int intProp { get; set; }
        internal int[,] intArrayXXProp { get; set; }
        internal int[] intArrayXProp { get; set; }
        internal int[][] intArrayXArrayXProp { get; set; }
        internal long longProp { get; set; }
        internal object objectProp { get; set; }
        internal sbyte sbyteProp { get; set; }
        internal short shortProp { get; set; }
        internal string stringProp { get; set; }
        internal uint uintProp { get; set; }
        internal ulong ulongProp { get; set; }
        internal ushort ushortProp { get; set; }
        internal TestStruct testStructProp { get; set; }
        internal TestClass testClassProp { get; set; }
        internal TestClassAbstract TestClassAbstractProp { get; set; }
        internal TestInterface TestInterfaceProp { get; set; }
        internal TestEnum testEnumProp { get; set; }
    }

    class TestNullableProps
    {
        public bool? boolProp { get; set; }
        public byte? byteProp { get; set; }
        public char? charProp { get; set; }
        public decimal? decimalProp { get; set; }
        public double? doubleProp { get; set; }
        //public dynamic? dynamicProp { get; set; }
        public float? floatProp { get; set; }
        public int? intProp { get; set; }
        public int?[,] intArrayXXProp { get; set; }
        public int?[] intArrayXProp { get; set; }
        public int?[][] intArrayXArrayXProp { get; set; }
        public long? longProp { get; set; }
        //public object? objectProp { get; set; }
        public sbyte? sbyteProp { get; set; }
        public short? shortProp { get; set; }
        //public string? stringProp { get; set; }
        public uint? uintProp { get; set; }
        public ulong? ulongProp { get; set; }
        public ushort? ushortProp { get; set; }
        public TestStruct? testStructProp { get; set; }
        public TestEnum? testEnumProp { get; set; }
        public Pocoyo2.TestStruct qualifiedTestStructProp { get; set; }
        public Pocoyo2.TestClass qualifiedTestClassProp { get; set; }
        public Pocoyo2.TestEnum qualifiedTestEnumProp { get; set; }
    }

    class TestArrayProps
    {
        public bool[] boolProp { get; set; }
        public byte[] byteProp { get; set; }
        public char[] charProp { get; set; }
        public decimal[] decimalProp { get; set; }
        public double[] doubleProp { get; set; }
        public dynamic[] dynamicProp { get; set; }
        public float[] floatProp { get; set; }
        public int[] intProp { get; set; }
        public int[,] intArrayXXProp { get; set; }
        public int[] intArrayXProp { get; set; }
        public int[][] intArrayXArrayXProp { get; set; }
        public long[] longProp { get; set; }
        public object[] objectProp { get; set; }
        public sbyte[] sbyteProp { get; set; }
        public short[] shortProp { get; set; }
        public string[] stringProp { get; set; }
        public uint[] uintProp { get; set; }
        public ulong[] ulongProp { get; set; }
        public ushort[] ushortProp { get; set; }
        public TestStruct[] testStructProp { get; set; }
        public TestClass[] testClassProp { get; set; }
        public TestEnum[] testEnumProp { get; set; }
        public Pocoyo2.TestStruct[] qualifiedTestStructProp { get; set; }
        public Pocoyo2.TestClass[] qualifiedTestClassProp { get; set; }
        public Pocoyo2.TestEnum[] qualifiedTestEnumProp { get; set; }
    }

    class TestListProps
    {
        public List<bool> boolProp { get; set; }
        public List<byte> byteProp { get; set; }
        public List<char> charProp { get; set; }
        public List<decimal> decimalProp { get; set; }
        public List<double> doubleProp { get; set; }
        public List<dynamic> dynamicProp { get; set; }
        public List<float> floatProp { get; set; }
        public List<int> intProp { get; set; }
        public List<int[,]> intArrayXXProp { get; set; }
        public List<int[]> intArrayXProp { get; set; }
        public List<int[][]> intArrayXArrayXProp { get; set; }
        public List<long> longProp { get; set; }
        public List<object> objectProp { get; set; }
        public List<sbyte> sbyteProp { get; set; }
        public List<short> shortProp { get; set; }
        public List<string> stringProp { get; set; }
        public List<uint> uintProp { get; set; }
        public List<ulong> ulongProp { get; set; }
        public List<ushort> ushortProp { get; set; }
        public List<TestStruct> testStructProp { get; set; }
        public List<TestClass> testClassProp { get; set; }
        public List<TestEnum> testEnumProp { get; set; }
        public List<Pocoyo2.TestStruct> qualifiedTestStructProp { get; set; }
        public List<Pocoyo2.TestClass> qualifiedTestClassProp { get; set; }
        public List<Pocoyo2.TestEnum> qualifiedTestEnumProp { get; set; }
    }

    class TestCollectionsGenericListProps
    {
        public System.Collections.Generic.List<bool> boolProp { get; set; }
        public System.Collections.Generic.List<byte> byteProp { get; set; }
        public System.Collections.Generic.List<char> charProp { get; set; }
        public System.Collections.Generic.List<decimal> decimalProp { get; set; }
        public System.Collections.Generic.List<double> doubleProp { get; set; }
        public System.Collections.Generic.List<dynamic> dynamicProp { get; set; }
        public System.Collections.Generic.List<float> floatProp { get; set; }
        public System.Collections.Generic.List<int> intProp { get; set; }
        public System.Collections.Generic.List<int[,]> intArrayXXProp { get; set; }
        public System.Collections.Generic.List<int[]> intArrayXProp { get; set; }
        public System.Collections.Generic.List<int[][]> intArrayXArrayXProp { get; set; }
        public System.Collections.Generic.List<long> longProp { get; set; }
        public System.Collections.Generic.List<object> objectProp { get; set; }
        public System.Collections.Generic.List<sbyte> sbyteProp { get; set; }
        public System.Collections.Generic.List<short> shortProp { get; set; }
        public System.Collections.Generic.List<string> stringProp { get; set; }
        public System.Collections.Generic.List<uint> uintProp { get; set; }
        public System.Collections.Generic.List<ulong> ulongProp { get; set; }
        public System.Collections.Generic.List<ushort> ushortProp { get; set; }
        public System.Collections.Generic.List<TestStruct> testStructProp { get; set; }
        public System.Collections.Generic.List<TestClass> testClassProp { get; set; }
        public System.Collections.Generic.List<TestEnum> testEnumProp { get; set; }
    }
}

