using System;

namespace Diploma.UI.Converters
{

    public static class ConvertersHost
    {

        static ConvertersHost()
        {
            TrueToVisible = new BoolToVisibilityConverter(true);
            FalseToVisible = new BoolToVisibilityConverter(false);
            NullToTrue = new NullToBoolConverter(true);
            NullToFalse = new NullToBoolConverter(false);
        }

        public static BoolToVisibilityConverter TrueToVisible
        {
            get;
            
            private set;
        }

        public static BoolToVisibilityConverter FalseToVisible
        {
            get;

            private set;
        }

        public static NullToBoolConverter NullToTrue
        {
            get;

            private set;
        }

        public static NullToBoolConverter NullToFalse
        {
            get;

            private set;
        }

    }

}