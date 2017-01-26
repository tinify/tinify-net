using System.Threading.Tasks;

namespace TinifyAPI
{
    public static class SourceTaskExtensions
    {
        public static async Task<Source> Preserve(this Task<Source> task, params string[] options)
        {
            var source = await task.ConfigureAwait(false);
            return source.Preserve(options);
        }

        public static async Task<Source> Resize(this Task<Source> task, object options)
        {
            var source = await task.ConfigureAwait(false);
            return source.Resize(options);
        }

        public static async Task<ResultMeta> Store(this Task<Source> task, object options)
        {
            var source = await task.ConfigureAwait(false);
            return await source.Store(options).ConfigureAwait(false);
        }

        public static async Task<Result> GetResult(this Task<Source> task)
        {
            var source = await task.ConfigureAwait(false);
            return await source.GetResult().ConfigureAwait(false);
        }

        public static async Task ToFile(this Task<Source> task, string path)
        {
            var source = await task.ConfigureAwait(false);
            await source.ToFile(path).ConfigureAwait(false);
        }

        public static async Task<byte[]> ToBuffer(this Task<Source> task)
        {
            var source = await task.ConfigureAwait(false);
            return await source.ToBuffer().ConfigureAwait(false);
        }
    }
}
