namespace Demo.PolicyApiClient.Functions
{
    public static partial class Functions
    {
        public abstract class CompositeSpecification : ISpecification
        {
            public abstract bool IsSatisfiedBy(dynamic item);

            public ISpecification And(ISpecification specification)
            {
                return new AndSpecification(this, specification);
            }
            public ISpecification Or(ISpecification specification)
            {
                return new OrSpecification(this, specification);
            }
            public ISpecification Not(ISpecification specification)
            {
                return new NotSpecification(this, specification);
            }
        }
    }
}
