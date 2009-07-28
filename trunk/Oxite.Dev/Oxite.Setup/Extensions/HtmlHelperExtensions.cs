//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace Oxite.Modules.Setup.Extensions
{
    public static class HtmlHelperExtensions
    {
        #region WizardHeader
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="name"></param>
        /// <param name="getIsChecked"></param>
        /// <param name="labelInnerHtml"></param>
        /// <returns></returns>
        /// <example>
        /// <div class="wizardHeader">
        ///     <div class="wizardStep">Step 1</div>
        ///     <div class="wizardStepConnectorFinished"></div>
        ///     <div class="wizardCurrentStep">Step 2</div>
        ///     <div class="wizardStepConnector"></div>
        ///     <div class="wizardStep">Step 3</div>
        /// </div>
        /// </example>
        public static string WizardHeader(this HtmlHelper htmlHelper, List<string> steps, int currentStep)
        {
            StringBuilder output = new StringBuilder();

            TagBuilder wizardTag = new TagBuilder("div");

            wizardTag.Attributes["class"] = "wizardHeader";

            for (int i = 0; i < steps.Count; i++)
            {
                if (i > 0)
                {
                    TagBuilder connectorTag = new TagBuilder("div");

                    if (i <= currentStep)
                    {
                        connectorTag.Attributes["class"] = "wizardStepConnectorFinished";
                    }
                    else
                    {
                        connectorTag.Attributes["class"] = "wizardStepConnector";
                    }

                    output.Append(connectorTag.ToString());
                }

                TagBuilder builder = new TagBuilder("div");

                if (i == currentStep)
                {
                    builder.Attributes["class"] = "wizardCurrentStep";
                }
                else
                {
                    builder.Attributes["class"] = "wizardStep";
                }

                builder.InnerHtml = steps[i];

                output.Append(builder.ToString());
            }

            wizardTag.InnerHtml = output.ToString();

            return wizardTag.ToString();
        }
        #endregion
    }
}
