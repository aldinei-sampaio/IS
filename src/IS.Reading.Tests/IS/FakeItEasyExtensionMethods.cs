using FakeItEasy.Configuration;

namespace IS
{
    internal static class FakeItEasyExtensionMethods
    {
        public static IAfterCallConfiguredWithOutAndRefParametersConfiguration<IReturnValueConfiguration<Task<T>>> ReturnsDefault<T>(this IReturnValueConfiguration<Task<T>> configuration)
            => configuration.Returns(Task.FromResult<T>(default));

        public static IAfterCallConfiguredWithOutAndRefParametersConfiguration<IReturnValueConfiguration<T>> ReturnsDefault<T>(this IReturnValueConfiguration<T> configuration)
            => configuration.Returns(default);
    }
}
