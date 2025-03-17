using GameObjects.Construct.Parts;

namespace GameObjects.Construct
{
    public class Construct
    {
        ConstructPartCore[] _allParts = new ConstructPartCore[0];

        public int MaxPartCount => _allParts.Length;
    }
}