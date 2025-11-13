using FluentValidation.TestHelper;
using IAM_Service.Application.Users.Command;

namespace IAM_Service.Application.Tests.Users.Command
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdateUserCommandValidationTests
    {
        /// <summary>
        /// The validator
        /// </summary>
        private readonly UpdateUserCommandValidation _validator = new();

        /// <summary>
        /// Shoulds the fail when user identifier is invalid.
        /// </summary>
        [Fact]
        public void Should_Fail_When_UserId_Is_Invalid()
        {
            var command = new UpdateUserCommand { UserId = 0 };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.UserId);
        }

        /// <summary>
        /// Shoulds the pass when valid command.
        /// </summary>
        [Fact]
        public void Should_Pass_When_Valid_Command()
        {
            var command = new UpdateUserCommand
            {
                UserId = 1,
                FullName = "John Doe",
                PhoneNumber = "0123456789",
                Gender = "Male",
                Age = 25,
                Address = "HCM City",
                DateOfBirth = new DateOnly(2000, 1, 1),
            };

            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        /// <summary>
        /// Shoulds the fail invalid phone.
        /// </summary>
        [Fact]
        public void Should_Fail_Invalid_Phone()
        {
            var command = new UpdateUserCommand { UserId = 1, PhoneNumber = "abc123" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
        }

        /// <summary>
        /// Shoulds the skip validation when fields are null.
        /// </summary>
        [Fact]
        public void Should_Skip_Validation_When_Fields_Are_Null()
        {
            var command = new UpdateUserCommand { UserId = 1 };

            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(x => x.FullName);
            result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
            result.ShouldNotHaveValidationErrorFor(x => x.Gender);
        }

        /// <summary>
        /// Shoulds the fail invalid dob.
        /// </summary>
        [Fact]
        public void Should_Fail_Invalid_DOB()
        {
            var command = new UpdateUserCommand
            {
                UserId = 1,
                DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddDays(1)) // Future date
            };

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.DateOfBirth);
        }
    }
}