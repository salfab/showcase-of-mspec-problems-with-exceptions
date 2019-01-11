using System;

namespace MSpecExceptionsExample
{
    using MSpecExceptionsExample.Contracts;

    public class AwesomenessService
    {
        private readonly IDependencyToAwesomeness dependencyToAwesomeness;

        public AwesomenessService(IDependencyToAwesomeness dependencyToAwesomeness)
        {
            this.dependencyToAwesomeness = dependencyToAwesomeness;
        }

        public string MakeItAwesome(string nonAwesomeString)
        {
            if (nonAwesomeString == null)
            {
                throw new ArgumentNullException(nameof(nonAwesomeString));
            }

            // very important step 1
            this.dependencyToAwesomeness.SprinkleSomeMagic();

            // very important step 2
            this.dependencyToAwesomeness.SprinkleMoreMagic();
            
            // very important step 3
            var theAwesomenessIsCapital = this.dependencyToAwesomeness.AddTheFinalTouch();

            var seedOfAwesomeness = this.dependencyToAwesomeness.GenerateSeedOfAwesomeness();

            if (theAwesomenessIsCapital)
            {
                seedOfAwesomeness = seedOfAwesomeness.ToUpper();
            }

            if (!seedOfAwesomeness.Contains("AWESOME"))
            {
                throw new NotSupportedException("Making a string awesome is not supported yet if the magic seed is not AWESOME.");
            }

            var awesomeizedString = nonAwesomeString + seedOfAwesomeness;

            return awesomeizedString;
        }
    }
}
