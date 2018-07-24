using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Eberhard.Core.Utilities
{
    public static class ApplicationCommands
    {
       
        public static RoutedUICommand Close
        {
            get
            {
                return new RoutedUICommand("Close the Application window",
                    "Close", typeof(ApplicationCommands));
            }
        }

        public static RoutedUICommand Minimize
        {
            get
            {
                return new RoutedUICommand("Minimize the Application window to toolbar",
                    "Minimize", typeof(ApplicationCommands));
            }
        }


        public static RoutedUICommand FullScreen
        {
            get
            {
                return new RoutedUICommand("Blow the window to a full screen over the tool bar",
                    "FullScreen", typeof(ApplicationCommands));
            }
        }
    }
}
