// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;

//ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    public class ListDiagnosticSource : DiagnosticSource
    {
        public ListDiagnosticSource(List<Tuple<string, object>> diagnosticList)
        {
            DiagnosticList = diagnosticList;
        }

        public List<Tuple<string, object>> DiagnosticList { get; }

        public override void Write(string diagnosticName, object parameters)
            => DiagnosticList?.Add(new Tuple<string, object>(diagnosticName, parameters));

        public override bool IsEnabled(string diagnosticName) => true;
    }
}
