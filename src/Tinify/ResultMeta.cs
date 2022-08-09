using System;
using System.Net.Http.Headers;

namespace TinifyAPI
{
    public class ResultMeta
    {
        protected HttpResponseHeaders Meta { get; set; }

        internal ResultMeta(HttpResponseHeaders meta)
        {
            Meta = meta;
        }

        public uint? Width
        {
            get
            {
                if (!Meta.TryGetValues("Image-Width", out var values))
                {
                    return null;
                }

                foreach (var header in values)
                {
                    if (uint.TryParse(header, out var value))
                    {
                        return value;
                    }
                }
                return null;
            }
        }

        public uint? Height
        {
            get
            {
                if (!Meta.TryGetValues("Image-Height", out var values))
                {
                    return null;
                }

                foreach (var header in values)
                {
                    if (uint.TryParse(header, out var value))
                    {
                        return value;
                    }
                }
                return null;
            }
        }

        public Uri Location => Meta.Location;
    }
}
