using System;

namespace PartsHunter.Models
{
    public static class SystemForm
    {
        public static class ComponentRegisterForm
        {
            public static bool ValidatedCategory { get; set; }
            public static bool ValidatedDescription { get; set; }
            public static bool ValidatedDrawer { get; set; }
        }

        public static string Current_Button_Click { get; set; } = String.Empty;

        public static string Last_Button_Click { get; set; } = String.Empty;

        public static string Current_Selected_Box { get; set; } = String.Empty;

        public static string Current_Category { get; set; } = String.Empty;

        public static int SEARCH { get; set; } = 0;
        public static int REGISTER { get; set; } = 1;
        public static int CLEAR_ALL { get; set; } = 0;
        public static int CLEAR_KEEPING_FILLED_DRAWERS { get; set; } = 1;

        public static bool Pre_Load_Done { get; set; } = false;
        public static string BatcFileLocation { get; set; }

    }
}
