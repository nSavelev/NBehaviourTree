using System;

namespace NBehaviourTree.Runtime.Comparator
{
    public static class Comparator
    {
        public static bool Compare(CompareFunc func, int a, int b)
        {
            switch (func)
            {
                case CompareFunc.False:
                    return false;
                    break;
                case CompareFunc.Less:
                    return a < b;
                    break;
                case CompareFunc.LEqual:
                    return a <= b;
                    break;
                case CompareFunc.Equal:
                    return a == b;
                    break;
                case CompareFunc.GEqual:
                    return a >= b;
                    break;
                case CompareFunc.Greater:
                    return a > b;
                    break;
                case CompareFunc.True:
                    return true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(func), func, null);
            }
        }
        
        public static bool Compare(CompareFunc func, float a, float b)
        {
            switch (func)
            {
                case CompareFunc.False:
                    return false;
                    break;
                case CompareFunc.Less:
                    return a < b;
                    break;
                case CompareFunc.LEqual:
                    return a <= b;
                    break;
                case CompareFunc.Equal:
                    return a == b;
                    break;
                case CompareFunc.GEqual:
                    return a >= b;
                    break;
                case CompareFunc.Greater:
                    return a > b;
                    break;
                case CompareFunc.True:
                    return true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(func), func, null);
            }
        }
    }
}