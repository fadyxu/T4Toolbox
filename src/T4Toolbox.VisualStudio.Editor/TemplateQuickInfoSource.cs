// <copyright file="TemplateQuickInfoSource.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;

    internal sealed class TemplateQuickInfoSource : IAsyncQuickInfoSource //IQuickInfoSource//,

    {
        private readonly TemplateAnalyzer analyzer;

        public TemplateQuickInfoSource(ITextBuffer buffer)
        {
            Debug.Assert(buffer != null, "buffer");
            this.analyzer = TemplateAnalyzer.GetOrCreate(buffer);
        }

        public void AugmentQuickInfoSession(IAsyncQuickInfoSession session, IList<object> quickInfoContent, out ITrackingSpan applicableToSpan)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }

            if (quickInfoContent == null)
            {
                throw new ArgumentNullException("quickInfoContent");
            }

            TemplateAnalysis analysis = this.analyzer.CurrentAnalysis;
            SnapshotPoint? triggerPoint = session.GetTriggerPoint(analysis.TextSnapshot);
            if (triggerPoint != null && analysis.Template != null)
            {
                string description;
                Span applicableTo;
                if (analysis.Template.TryGetDescription(triggerPoint.Value.Position, out description, out applicableTo))
                {
                    quickInfoContent.Add(description);
                    applicableToSpan = analysis.TextSnapshot.CreateTrackingSpan(applicableTo, SpanTrackingMode.EdgeExclusive); 
                    return;                    
                }
            }

            applicableToSpan = null;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public Task<QuickInfoItem> GetQuickInfoItemAsync(IAsyncQuickInfoSession session, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}