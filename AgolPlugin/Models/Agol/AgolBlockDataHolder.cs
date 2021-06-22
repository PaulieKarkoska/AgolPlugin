namespace AgolPlugin.Models.Agol
{
    public class AgolBlockDataHolder
    {
        public AgolBlockDataHolder(string globalId, int objectId, string serviceItemId, string serviceUrl)
        {
            //GlobalId = globalId;
            //ObjectId = objectId;
            //ServiceItemId = serviceItemId;
            //ServiceUrl = serviceUrl;
        }

        public string GlobalId { get; private set; }
        public int ObjectId { get; private set; }
        public string ServiceItemId { get; private set; }
        public string ServiceUrl { get; private set; }
    }
}