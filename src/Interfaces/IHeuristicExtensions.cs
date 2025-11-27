using AngleSharp.Dom;

namespace savorfolio_backend.Interfaces;

public interface IHeuristicExtensions
{
    string GetBestMatch(IEnumerable<IElement> labelElements, string labelPattern);
    string MatchEnum<TEnum>(IDocument document)
        where TEnum : Enum;
}
