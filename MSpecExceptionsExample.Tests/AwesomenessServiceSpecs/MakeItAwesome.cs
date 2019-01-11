namespace MSpecExceptionsExample.Tests.AwesomenessServiceSpecs
{
    using System;
    using System.Net.Mail;
    using System.Runtime.CompilerServices;
    using System.Transactions;

    using FluentAssertions;

    using Machine.Specifications;

    using Moq;

    using MSpecExceptionsExample.Contracts;

    using It = Machine.Specifications.It;

    public class MakeItAwesome
    {
        public class Given_instanciated_AwesomenessService
        {
            #region context
            private Establish context = () =>
                {
                    InjectedDependencyToAwesomeness = new Mock<IDependencyToAwesomeness>(MockBehavior.Strict);
                    Sut = new AwesomenessService(InjectedDependencyToAwesomeness.Object);
                };

            public static Mock<IDependencyToAwesomeness> InjectedDependencyToAwesomeness { get; private set; }

            public static AwesomenessService Sut { get; set; }

            #endregion
        }

        class when_MakeItAwesome_is_called_with_null_specified_nonAwesomeString : Given_instanciated_AwesomenessService
        {
            #region context

            public static Exception ThrownException { get; set; }

            Because of = () => ThrownException = Catch.Exception(() => Sut.MakeItAwesome(null));

            #endregion

            private It should_throw_ArgumentNullException = () => ThrownException.Should().NotBeNull("No exception was thrown").And.BeOfType<ArgumentNullException>("The wrong type of exception was thrown");
        }

        class when_MakeItAwesome_is_called_with_instantiated_SpecifiedNonAwesomeString : Given_instanciated_AwesomenessService
        {
            #region context
            public static string SpecifiedNonAwesomeString { get; set; }
            public static string Result { get; set; }

            // STEP 1: in order to be able to run should_throw_NotSupportedException successfully,            
            // I need to use the Because that catches the exception.
            //
            // Try to switch it, then go to comment identified with STEP 2
            
            public Because of = () => Result = Sut.MakeItAwesome(SpecifiedNonAwesomeString);
            //public Because of = () => ThrownException = Catch.Exception(() => Result =  Sut.MakeItAwesome(SpecifiedNonAwesomeString));
            public static Exception ThrownException { get; set; }


            Establish context = () =>
                {
                    SpecifiedNonAwesomeString = "not an awesome string";

                    InjectedDependencyToAwesomeness
                        .Setup(o => o.SprinkleSomeMagic())
                        .Verifiable();

                    InjectedDependencyToAwesomeness
                        .Setup(o => o.SprinkleMoreMagic())
                        .Verifiable();

                    InjectedDependencyToAwesomeness
                        .Setup(o => o.AddTheFinalTouch())
                        .Returns(() => ReturnedFinalTouch);


                    // STEP 2: Try commenting this setup here, and look at the test should_return_string_with_length_of_26.

                    InjectedDependencyToAwesomeness
                        .Setup(o => o.GenerateSeedOfAwesomeness())
                        .Returns(() => ReturnedSeedOfAwesomeness);
                };

            public static string ReturnedSeedOfAwesomeness { get; set; }

            public static bool ReturnedFinalTouch { get; set; }

            #endregion
        }

        class when_generated_seed_of_awesomeness_is_awful : when_MakeItAwesome_is_called_with_instantiated_SpecifiedNonAwesomeString
        {
            #region context
            
            private Establish context = () => ReturnedSeedOfAwesomeness = "awful";

            #endregion

            // Here, we have a problem. If I use a "Because" that doesn't catch exception, the test will obviously not work.
            // ThrownExceptionm will be null and the test runner will tell me that an exception occured. This is expected. We need to use the Catch.Exception for this context as well.
            // what annoys me is that the parent context is also inherited by contexts that will end up in a nominal scenario that will *not* throw an exception.

            // The reason it annoys me is that have all these setups in "when_MakeItAwesome_is_called_with_instantiated_SpecifiedNonAwesomeString", and that I don't want to duplicate that logic.
            // (this is after all the reason why MSpec is so great : to be able to recycle the arrange)

            // If I do however swith to a Because that catches exceptions, there will be some side effects.
            // These side effects are illustrated by the comments starting with // STEP 1 and // STEP 2:
            // If a developer forgets to setup a mock (MockExceptions is thrown) - or if for ANY other reason, an unexpected exception is thrown
            // That exception will be swallowed.
            //
            // Please see other comments to play around with the contexts, comment and reproduce the behavior I described.
            It should_throw_NotSupportedException = () => ThrownException.Should().NotBeNull().And.BeOfType<NotSupportedException>("If I forget a mock setup, this test is expected to fail ! That is good !");
        }

        class when_generated_seed_of_awesomeness_is_awesome : when_MakeItAwesome_is_called_with_instantiated_SpecifiedNonAwesomeString
        {
            #region context

            private Establish context = () => ReturnedSeedOfAwesomeness = "awesome"; 

            #endregion
        }

        class when_the_final_touch_makes_awesomeness_capital : when_generated_seed_of_awesomeness_is_awesome
        {
            #region context

            private Establish context = () => { ReturnedFinalTouch = true; };

            #endregion

            It should_call_SprinkleSomeMagic_on_InjectedDependencyToAwesomeness = () => InjectedDependencyToAwesomeness.Verify(o => o.SprinkleSomeMagic(), Times.Once);
            It should_call_SprinkleMoreMagic_on_InjectedDependencyToAwesomeness = () => InjectedDependencyToAwesomeness.Verify(o => o.SprinkleMoreMagic(), Times.Once);
            It should_call_AddTheFinalTouch_on_InjectedDependencyToAwesomeness = () => InjectedDependencyToAwesomeness.Verify(o => o.AddTheFinalTouch(), Times.Once);

            private It should_return_string_ending_with_AWESOME = () => Result.Should().EndWith("AWESOME");

            // When one mock setup is missing, The implementation will throw a MockException.
            // However, this test will be red with a NullReferenceException, thrown by the assertion because Result is null.
            // What I would expect instead is to have the runnerpointing me to the right direction, saying that a MockException was thrown.
            private It should_return_string_with_length_of_26 = () => Result.Length.Should().Be(28, "If I forget a mock setup, I don't want to have a NullReferenceException, I want a MockException with the appropriate stacktrace. Also, this message will -understandably- not appear in the test runner.");
        }
    }
}