﻿using CefFlashBrowser.Models.Data;
using CefFlashBrowser.Utils;
using CefFlashBrowser.Views.Dialogs.JsDialogs;
using CefSharp;
using CefSharp.Wpf;

namespace CefFlashBrowser.FlashBrowser.Handlers
{
    public class JsDialogHandler : IJsDialogHandler
    {
        public bool OnBeforeUnloadDialog(IWebBrowser chromiumWebBrowser, IBrowser browser, string messageText, bool isReload, IJsDialogCallback callback)
        {
            if (GlobalData.Settings.DisableOnBeforeUnloadDialog)
            {
                callback.Continue(true);
                return true;
            }

            ((ChromiumWebBrowser)chromiumWebBrowser).Dispatcher.Invoke(() =>
            {
                var title = LanguageManager.GetString(isReload ? "title_askWhetherToReload" : "title_askWhetherToLeave");
                JsConfirmDialog.ShowDialog(messageText, title, result =>
                {
                    callback.Continue(result == true);
                });
            });
            return true;
        }

        public void OnDialogClosed(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            //throw new NotImplementedException();
        }

        public bool OnJSDialog(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, CefJsDialogType dialogType, string messageText, string defaultPromptText, IJsDialogCallback callback, ref bool suppressMessage)
        {
            var targetBrowser = (ChromiumWebBrowser)chromiumWebBrowser;

            switch (dialogType)
            {
                case CefJsDialogType.Alert:
                    {
                        targetBrowser.Dispatcher.Invoke(() =>
                        {
                            JsAlertDialog.ShowDialog(messageText, targetBrowser.Title);
                        });
                        suppressMessage = true;
                        return false;
                    }

                case CefJsDialogType.Confirm:
                    {
                        targetBrowser.Dispatcher.Invoke(() =>
                        {
                            JsConfirmDialog.ShowDialog(messageText, targetBrowser.Title, result =>
                            {
                                callback.Continue(result == true);
                            });
                        });
                        suppressMessage = false;
                        return true;
                    }

                case CefJsDialogType.Prompt:
                    {
                        targetBrowser.Dispatcher.Invoke(() =>
                        {
                            JsPromptDialog.ShowDialog(messageText, targetBrowser.Title, defaultPromptText, result =>
                            {
                                callback.Continue(result != null, result);
                            });
                        });
                        suppressMessage = false;
                        return true;
                    }
            }
            return false;
        }

        public void OnResetDialogState(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            //throw new NotImplementedException();
        }
    }
}
