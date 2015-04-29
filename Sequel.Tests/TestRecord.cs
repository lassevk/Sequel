using JetBrains.Annotations;

namespace Sequel.Tests
{
    public class TestRecord
    {
        [CanBeNull]
        public string Key
        {
            get;
            set;
        }

        public int Value
        {
            get;
            set;
        }

        protected bool Equals(TestRecord other)
        {
            return string.Equals(Key, other.Key) && Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((TestRecord) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Key != null ? Key.GetHashCode() : 0)*397) ^ Value;
            }
        }
    }
}