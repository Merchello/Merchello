namespace Merchello.Tests.Umbraco.TestHelpers.Fake
{
    using global::Umbraco.Core.Persistence.Mappers;
    using global::Umbraco.Core.Persistence.Querying;
    using global::Umbraco.Core.Persistence.SqlSyntax;

    using Moq;

    public class FakeQueryFactory : IQueryFactory
    {

        public FakeQueryFactory(ISqlSyntaxProvider sqlSyntax)
        {
            this.MappingResolver = new Mock<IMappingResolver>().Object;

            this.SqlSyntax = sqlSyntax;
        }

        public IQuery<T> Create<T>()
        {
            throw new System.NotImplementedException();
        }

        public IMappingResolver MappingResolver { get; }

        public ISqlSyntaxProvider SqlSyntax { get; }
    }
}