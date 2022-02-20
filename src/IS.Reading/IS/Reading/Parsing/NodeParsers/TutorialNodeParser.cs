namespace IS.Reading.Parsing.NodeParsers;

public class TutorialNodeParser : DialogNodeParserBase, ITutorialNodeParser
{
    public override string Name => "!";

    public override BalloonType BalloonType => BalloonType.Tutorial;

    public TutorialNodeParser(
        IElementParser elementParser,
        ITutorialChildNodeParser tutorialChildNodeParser,
        ISetNodeParser setNodeParser
    ) : base(elementParser, tutorialChildNodeParser, setNodeParser)
    {
    }
}
