using InstagramDownloaderTest.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace InstagramDownloaderTest.Service
{
     public class DialogService : IDialogService
    {
        /// <summary>
        /// Shows a message in the dialog.
        /// </summary>
        /// <param name="message">The message to show.</param>
        public void Show(string message)
        {
            MessageBox.Show(message);
        }

        /// <summary>
        /// Shows a message with a caption in the dialog.
        /// </summary>
        /// <param name="message">The message to show.</param>
        /// <param name="caption">The caption of the dialog.</param>
        public void Show(string message, string caption)
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK);
        }


        public MessageBoxResult Show(string message, string caption, MessageBoxButton buttons)
        {
            var d = MessageBox.Show(message, caption, buttons);
            return d;
        }
    }
}
