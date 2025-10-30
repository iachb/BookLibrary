using System;
using System.Collections.Generic;

namespace BookLibrary.Core.Exceptions
{
    public class DomainValidationException : Exception
    {
        public IReadOnlyCollection<string> Errors { get; }

        public DomainValidationException(string message) : base(message)
        {
            Errors = new List<string> { message };
        }

        public DomainValidationException(IEnumerable<string> errors, string? message = null) : base(message ?? "Domain validation failed")
        {
            Errors = new List<string>(errors);
        }
    }
}
