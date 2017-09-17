using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pocoyo2
{
    struct TestStruct
    {
        bool boolProp1 { get; set; }
    }

    class TestClass
    {
        bool boolProp1 { get; set; }
    }

    abstract class TestClassAbstract
    {
        bool boolProp1 { get; set; }
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
        private Pocoyo.TestStruct qualifiedTestStructProp { get; set; }
        private Pocoyo.TestClass qualifiedTestClassProp { get; set; }
        private Pocoyo.TestEnum qualifiedTestEnumProp { get; set; }
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
    }

    class TestArrayProps
    {
        bool[] boolProp { get; set; }
        byte[] byteProp { get; set; }
        char[] charProp { get; set; }
        decimal[] decimalProp { get; set; }
        double[] doubleProp { get; set; }
        dynamic[] dynamicProp { get; set; }
        float[] floatProp { get; set; }
        int[] intProp { get; set; }
        int[,] intArrayXXProp { get; set; }
        int[] intArrayXProp { get; set; }
        int[][] intArrayXArrayXProp { get; set; }
        long[] longProp { get; set; }
        object[] objectProp { get; set; }
        sbyte[] sbyteProp { get; set; }
        short[] shortProp { get; set; }
        string[] stringProp { get; set; }
        uint[] uintProp { get; set; }
        ulong[] ulongProp { get; set; }
        ushort[] ushortProp { get; set; }
        TestStruct[] testStructProp { get; set; }
        TestClass[] testClassProp { get; set; }
        TestEnum[] testEnumProp { get; set; }
    }

    class TestListProps
    {
        List<bool> boolProp { get; set; }
        List<byte> byteProp { get; set; }
        List<char> charProp { get; set; }
        List<decimal> decimalProp { get; set; }
        List<double> doubleProp { get; set; }
        List<dynamic> dynamicProp { get; set; }
        List<float> floatProp { get; set; }
        List<int> intProp { get; set; }
        List<int[,]> intArrayXXProp { get; set; }
        List<int[]> intArrayXProp { get; set; }
        List<int[][]> intArrayXArrayXProp { get; set; }
        List<long> longProp { get; set; }
        List<object> objectProp { get; set; }
        List<sbyte> sbyteProp { get; set; }
        List<short> shortProp { get; set; }
        List<string> stringProp { get; set; }
        List<uint> uintProp { get; set; }
        List<ulong> ulongProp { get; set; }
        List<ushort> ushortProp { get; set; }
        List<TestStruct> testStructProp { get; set; }
        List<TestClass> testClassProp { get; set; }
        List<TestEnum> testEnumProp { get; set; }
    }

    class TestCollectionsGenericListProps
    {
        System.Collections.Generic.List<bool> boolProp { get; set; }
        System.Collections.Generic.List<byte> byteProp { get; set; }
        System.Collections.Generic.List<char> charProp { get; set; }
        System.Collections.Generic.List<decimal> decimalProp { get; set; }
        System.Collections.Generic.List<double> doubleProp { get; set; }
        System.Collections.Generic.List<dynamic> dynamicProp { get; set; }
        System.Collections.Generic.List<float> floatProp { get; set; }
        System.Collections.Generic.List<int> intProp { get; set; }
        System.Collections.Generic.List<int[,]> intArrayXXProp { get; set; }
        System.Collections.Generic.List<int[]> intArrayXProp { get; set; }
        System.Collections.Generic.List<int[][]> intArrayXArrayXProp { get; set; }
        System.Collections.Generic.List<long> longProp { get; set; }
        System.Collections.Generic.List<object> objectProp { get; set; }
        System.Collections.Generic.List<sbyte> sbyteProp { get; set; }
        System.Collections.Generic.List<short> shortProp { get; set; }
        System.Collections.Generic.List<string> stringProp { get; set; }
        System.Collections.Generic.List<uint> uintProp { get; set; }
        System.Collections.Generic.List<ulong> ulongProp { get; set; }
        System.Collections.Generic.List<ushort> ushortProp { get; set; }
        System.Collections.Generic.List<TestStruct> testStructProp { get; set; }
        System.Collections.Generic.List<TestClass> testClassProp { get; set; }
        System.Collections.Generic.List<TestEnum> testEnumProp { get; set; }
    }
}

