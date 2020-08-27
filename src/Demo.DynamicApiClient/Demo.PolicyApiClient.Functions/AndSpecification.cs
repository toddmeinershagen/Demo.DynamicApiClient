namespace Demo.PolicyApiClient.Functions
{
    public static partial class Functions
    {
        public class AndSpecification : CompositeSpecification
        {
            ISpecification leftSpecification;
            ISpecification rightSpecification;

            public AndSpecification(ISpecification left, ISpecification right)
            {
                this.leftSpecification = left;
                this.rightSpecification = right;
            }

            public override bool IsSatisfiedBy(dynamic item)
            {
                return this.leftSpecification.IsSatisfiedBy(item)
                    && this.rightSpecification.IsSatisfiedBy(item);
            }
        }
    }
}
