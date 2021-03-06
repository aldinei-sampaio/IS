﻿using System;
using System.IO;
using System.Reflection;

namespace IS.Reading
{
    public static class EmbeddedResourceExtensions
    {
        public static Stream GetResourceStream(this object instance, string resourceName)
        {
            var asm = Assembly.GetExecutingAssembly();
            var stream = asm.GetManifestResourceStream(instance.GetType(), resourceName);
            if (stream == null)
                throw new Exception($"Recurso '{resourceName}' não encontrado.");
            return stream;
        }

        public static string GetResourceString(this object instance, string resourceName)
        {
            using var stream = GetResourceStream(instance, resourceName);
            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
