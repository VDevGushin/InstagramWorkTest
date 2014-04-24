using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramDownloaderTest.Service.Interfaces
{
    public interface IDialogService
    {
        /// <summary>
        /// Shows a message in the dialog.
        /// </summary>
        /// <param name="message">The message to show.</param>
        void Show(string message);

        /// <summary>
        /// Shows a message with a caption in the dialog.
        /// </summary>
        /// <param name="message">The message to show.</param>
        /// <param name="caption">The caption of the dialog.</param>
        void Show(string message, string caption);

        System.Windows.MessageBoxResult Show(string message, string caption, System.Windows.MessageBoxButton buttons);


    }
}
