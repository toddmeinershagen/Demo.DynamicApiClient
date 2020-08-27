namespace Demo.PolicyApiClient.Functions
{
    public static partial class Function1
    {
        public interface ISpecification
        {
            bool IsSatisfiedBy(dynamic item);
            ISpecification And(ISpecification specification);
            ISpecification Or(ISpecification specification);
            ISpecification Not(ISpecification specification);
        }
    }
}
