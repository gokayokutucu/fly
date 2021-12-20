using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace Fly.Persistence.Conventions
{
    public class LowerCaseConvention : IMemberMapConvention
    {
        private string _name = "LowerCaseConvention";

        #region Implementation of IConvention
        public string Name
        {
            get { return _name; }
            private set { _name = value; }
        }

        public void Apply(BsonMemberMap memberMap)
        {
            string elementName = memberMap.MemberName.Substring(0, 1).ToLower() + memberMap.MemberName.Substring(1);
            memberMap.SetElementName(elementName);
        }
        #endregion
    }

}