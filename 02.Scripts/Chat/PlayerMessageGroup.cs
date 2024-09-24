using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace FireEscape.Chat
{
    public class PlayerMessageGroup : MessageGroup
    {
        public string CurrentState { get; set; }
        public ChildEmotion ChildEmotion { get; set; }
        public List<SurroundingObject> SurroundingObjects { get; set; }
        //public List<PossibleAction> PossibleActions { get; set; }

        public string SerializeToYAML()
        {
            var serializer = new SerializerBuilder().Build();
            return serializer.Serialize(this);
        }
    }
}
