namespace Demo.PolicyApiClient.Functions
{
    public static partial class Functions
    {
        public class NotSpecification : CompositeSpecification
        {
            ISpecification leftSpecification;
            ISpecification rightSpecification;

            public NotSpecification(ISpecification left, ISpecification right)
            {
                this.leftSpecification = left;
                this.rightSpecification = right;
            }

            public override bool IsSatisfiedBy(dynamic item)
            {
                return this.leftSpecification.IsSatisfiedBy(item)
                    && this.rightSpecification.IsSatisfiedBy(item) == false;
            }
        }
    }
}
