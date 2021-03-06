﻿using System.Collections.Generic;
using System.Linq;
using System.Windows.Automation;
using WinAppDriver.Extensions;
using WinAppDriver.Infrastructure;

namespace WinAppDriver.Server.CommandHandlers
{
    /// <summary>
    /// Provides handling for the switch to window command.
    /// </summary>
    internal class SwitchToWindowCommandHandler : CommandHandler
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="environment">The <see cref="CommandEnvironment"/> to use in executing the command.</param>
        /// <param name="parameters">The <see cref="Dictionary{string, object}"/> containing the command parameters.</param>
        /// <returns>The JSON serialized string representing the command response.</returns>
        public override Response Execute(CommandEnvironment environment, Dictionary<string, object> parameters)
        {
            if (!parameters.TryGetValue("handle", out object handle))
            {
                return Response.CreateMissingParametersResponse("handle");
            }

            var windowIdentifier = handle?.ToString();
            if (windowIdentifier != CommandEnvironment.GlobalWindowHandle)
            {
                var windows = new BreadthFirstSearch().Find(environment.Cache.AutomationElement, ControlType.Window, environment.GetCancellationToken())
                    .Where(w => w.Current.ControlType == ControlType.Window)
                    .ToList();

                var matchingWindow = windows.SingleOrDefault(w => w.Current.AutomationId == windowIdentifier);
                if (matchingWindow != null)
                {
                    // if any of those windows is modal and it is not the window that user is switching to, return an error
                    var modalWindow = windows.FirstOrDefault(w => (bool)w.GetAutomationElementPropertyValue(WindowPattern.IsModalProperty) == true);
                    if (modalWindow != null && modalWindow != matchingWindow)
                    {
                        return Response.CreateErrorResponse(WebDriverStatusCode.UnhandledError, "Window is not enabled.");
                    }

                    environment.SwitchToWindow(matchingWindow);                    
                    return Response.CreateSuccessResponse();
                }

                return Response.CreateErrorResponse(WebDriverStatusCode.NoSuchWindow, "No window found");
            }

            return Response.CreateSuccessResponse();
        }
    }
}
