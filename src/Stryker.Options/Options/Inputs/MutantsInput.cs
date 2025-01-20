using System.Collections.Generic;
using System.Linq;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Configuration;
using Stryker.Utilities;

namespace Stryker.Abstractions.Options.Inputs
{
    public class MutantsInput : Input<IEnumerable<string>>
    {
        public override IEnumerable<string> Default => new List<string>();

        protected override string Description => @"Allows to specify mutants that should be tested.";

        public IEnumerable<IMutantPattern> Validate()
        {
            if (SuppliedInput is { } && SuppliedInput.Any())
            {
                var filesToInclude = new List<IMutantPattern>();

                foreach (var pattern in SuppliedInput)
                {
                    filesToInclude.Add(MutantPattern.Parse(FilePathUtils.NormalizePathSeparators(pattern)));
                }

                return filesToInclude;
            }

            return [];
        }
    }
}
