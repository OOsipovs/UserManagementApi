using FluentAssertions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagementApi.Application.DTOs;

namespace UserManagementApi.Tests
{
    public class CreateUserRequestValidationTests
    {
        private static IList<ValidationResult> Validate(CreateUserRequest request)
        {
            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(request, context, results, validateAllProperties: true);
            return results;
        }

        private static CreateUserRequest ValidRequest() => new()
        {
            Username = "johndoe",
            Email = "john@example.com",
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        [Fact]
        public void Validation_AllFieldsValid_PassesWithNoErrors()
        {
            var errors = Validate(ValidRequest());
            errors.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Validation_UsernameEmpty_FailsValidation(string username)
        {
            var request = ValidRequest();
            request.Username = username;

            var errors = Validate(request);
            errors.Should().Contain(e => e.MemberNames.Contains(nameof(CreateUserRequest.Username)));
        }

        [Fact]
        public void Validation_UsernameExceedsMaxLength_FailsValidation()
        {
            var request = ValidRequest();
            request.Username = new string('a', 101);

            var errors = Validate(request);
            errors.Should().Contain(e => e.MemberNames.Contains(nameof(CreateUserRequest.Username)));
        }

        [Theory]
        [InlineData("notanemail")]
        [InlineData("missing@")]
        [InlineData("@nodomain.com")]
        public void Validation_InvalidEmailFormat_FailsValidation(string email)
        {
            var request = ValidRequest();
            request.Email = email;

            var errors = Validate(request);
            errors.Should().Contain(e => e.MemberNames.Contains(nameof(CreateUserRequest.Email)));
        }

        [Fact]
        public void Validation_EmailExceedsMaxLength_FailsValidation()
        {
            var request = ValidRequest();
            request.Email = new string('a', 195) + "@x.com"; // > 200 chars

            var errors = Validate(request);
            errors.Should().Contain(e => e.MemberNames.Contains(nameof(CreateUserRequest.Email)));
        }

        [Fact]
        public void Validation_FirstNameEmpty_FailsValidation()
        {
            var request = ValidRequest();
            request.FirstName = string.Empty;

            var errors = Validate(request);
            errors.Should().Contain(e => e.MemberNames.Contains(nameof(CreateUserRequest.FirstName)));
        }

        [Fact]
        public void Validation_LastNameExceedsMaxLength_FailsValidation()
        {
            var request = ValidRequest();
            request.LastName = new string('b', 101);

            var errors = Validate(request);
            errors.Should().Contain(e => e.MemberNames.Contains(nameof(CreateUserRequest.LastName)));
        }
    }
}
