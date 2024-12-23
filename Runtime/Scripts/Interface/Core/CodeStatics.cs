namespace Trackman
{
    public static class CodeStatics
    {
#if UNITY_INCLUDE_TESTS
        public static bool IncludeTests => true;
#else
        public static bool IncludeTests => false;
#endif
    }
}