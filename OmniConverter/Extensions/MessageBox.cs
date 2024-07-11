using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia.Models;

namespace OmniConverter
{
    public sealed class MessageBox
    {
        public enum MsgBoxResult
        {
            Unknown = -1,
            Ok,
            Yes,
            No,
            Cancel,
            Abort
        }

        private static async Task<string> iShow(Window? owner, string message, string title, ButtonEnum buttons, Icon icon, HyperLinkParams? hlparams = null)
        {
            List<ButtonDefinition> btnsDef;

            switch (buttons)
            {
                case ButtonEnum.OkCancel:
                    btnsDef = new List<ButtonDefinition>
                    {
                        new ButtonDefinition { Name = "Ok", },
                        new ButtonDefinition { Name = "Cancel", IsCancel = true, }
                    };
                    break;

                case ButtonEnum.YesNo:
                    btnsDef = new List<ButtonDefinition>
                    {
                        new ButtonDefinition { Name = "Yes", },
                        new ButtonDefinition { Name = "No", IsCancel = true, }
                    };
                    break;

                case ButtonEnum.Ok:
                default:
                    btnsDef = new List<ButtonDefinition>
                    {
                        new ButtonDefinition { Name = "Ok", }
                    };
                    break;
            }

            if (hlparams == null) { hlparams = new HyperLinkParams { Text = "", Action = null }; }

            MessageBoxCustomParams msgboxParams = new MessageBoxCustomParams
            {
                ButtonDefinitions = btnsDef,
                ContentTitle = title,
                ContentMessage = message,
                Icon = icon,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false,
                MaxWidth = 500,
                MaxHeight = 800,
                SizeToContent = SizeToContent.WidthAndHeight,
                ShowInCenter = true,
                Topmost = true,
                HyperLinkParams = hlparams
            };

            var _ = MessageBoxManager.GetMessageBoxCustom(msgboxParams);

            if (owner != null)
                return await _.ShowWindowDialogAsync(owner);

            return await _.ShowWindowAsync();
        }

        public static MsgBoxResult Show(string text, string title = "OmniConverter - Information", ButtonEnum buttons = ButtonEnum.Ok, Icon icon = Icon.Info)
        {
            return Show(null, text, title, buttons, icon, null);
        }

        public static MsgBoxResult Show(Window? owner, string text, string title = "OmniConverter - Information", ButtonEnum buttons = ButtonEnum.Ok, Icon icon = Icon.Info, HyperLinkParams? hlparams = null)
        {
            MsgBoxResult result = MsgBoxResult.Unknown;
            var source = new CancellationTokenSource();
            var isMainThread = owner != null;

            try
            {
                if (isMainThread) Dispatcher.UIThread.Post(() =>
                {
                    if (owner != null)
                        owner.IsEnabled = false;
                });

                string temp = string.Empty;

                Dispatcher.UIThread.Post(() =>
                {
                    var _ = iShow(owner, text, title, buttons, icon, hlparams).ContinueWith(answer =>
                    {
                        temp = answer.Result;
                        source.Cancel();
                    });
                });

                if (isMainThread) Dispatcher.UIThread.MainLoop(source.Token);
                else while (!source.IsCancellationRequested) { Thread.Sleep(1); }

                string[] enums = Enum.GetNames(typeof(MsgBoxResult));
                for (int i = 0; i < enums.Length; i++)
                {
                    if (temp != null && temp.Equals(enums[i]))
                        result = (MsgBoxResult)Enum.Parse(typeof(MsgBoxResult), enums[i]);
                }
            }
            catch (Exception ex)
            {
                Debug.PrintToConsole(Debug.LogType.Error, ex.ToString());
            }

            if (isMainThread) Dispatcher.UIThread.Post(() => owner.IsEnabled = true);

            return result;
        }
    }
}
