namespace Trackman
{
    public static class CodeStatics
    {
#if UNITY_INCLUDE_TESTS
        public static bool IncludeTests => true;
#else
        public static bool IncludeTests => false;
#endif

#if TESTS_LOGGING_VERBOSE
        public static bool VerboseTestsLogging => true;
#else
        public static bool VerboseTestsLogging => false;
#endif

        public static bool IsRunningTests { get; set; }

        public static bool SkipVerboseLogs => IsRunningTests && !VerboseTestsLogging;
    }
}