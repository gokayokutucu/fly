namespace Fly.Domain.Common
{
    public abstract class PlanetUpdate
    {
        protected abstract string GetFieldName();
        protected abstract object GetFieldValue();
        public string FieldName => GetFieldName();
        public object FieldValue => GetFieldValue();
        protected abstract Type GetDataType();
        public Type DataType => GetDataType();
    }

    public class PlanetUpdate<T> : PlanetUpdate
    {
        public new string FieldName { get; }
        public new T FieldValue { get; }

        public PlanetUpdate(string fieldName, T fieldValue)
        {
            FieldName = fieldName;
            FieldValue = fieldValue;
        }

        protected override string GetFieldName()
        {
            return FieldName;
        }

        protected override object GetFieldValue()
        {
            return FieldValue;
        }

        protected override Type GetDataType()
        {
            return typeof(T);
        }
    }
}
