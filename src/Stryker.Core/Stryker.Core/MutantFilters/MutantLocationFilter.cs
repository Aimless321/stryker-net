using System.Collections.Generic;
using System.Linq;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.ProjectComponents;

namespace Stryker.Core.MutantFilters;

public class MutantLocationFilter : IMutantFilter
{
    public MutantFilter Type => MutantFilter.MutantPattern;

    public string DisplayName => "Mutant location filter";

    private readonly IEnumerable<IMutantPattern> _includeMutants;


    public MutantLocationFilter(IStrykerOptions options)
    {
        _includeMutants = options.Mutants;
    }

    public IEnumerable<IMutant> FilterMutants(IEnumerable<IMutant> mutants, IReadOnlyFileLeaf file,
        IStrykerOptions options)
    {
        return mutants.Where(IsMutantIncluded);

        bool IsMutantIncluded(IMutant mutant)
        {
            // Check if the the mutant is included.
            return _includeMutants.Any(MatchesPattern);

            bool MatchesPattern(IMutantPattern pattern)
            {
                // if we do not have the original node, we cannot exclude the mutation according to its location
                if (mutant.Mutation.OriginalNode == null)
                {
                    return false;
                }
                // We check both the full and the relative path to allow for relative paths.
                return pattern.IsMatch(file.FullPath, mutant.Mutation.OriginalNode.GetLocation().GetMappedLineSpan()) ||
                       pattern.IsMatch(file.RelativePath, mutant.Mutation.OriginalNode.GetLocation().GetMappedLineSpan());
            }
        }
    }
}
